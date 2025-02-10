using Newtonsoft.Json;
using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatViewController : MonoBehaviour
{
    private VisualElement _root;
    private ListView _chatMemberList;
    private VisualTreeAsset _chatMemberAsset;
    private List<NPCData> _allNPCs;
    private CenteredScrollView _centeredScrollView;

    private ListView _chatList;
    private List<ChatHistoryItem> _chat = new();
    private ChatData _gamestateChatData;
    private Dictionary<string, NPC> _chatDatas;
    private string _currentNPC;

    // Conversation state variables
    private Dictionary<string, NPCConversationState> _npcConversationStates = new();

    private InputSystem_Actions _actions;
    private ScrollValues _scrollValues = new();

    private bool _answerRequired = false;
    public void Initialize(VisualElement root)
    {
        _root = root;
        _gamestateChatData = GameStateManagerSingleton.Instance.GameState.ChatData;
        SetupVisualTreeAssets();
        LoadChatData();
        SetupChatList();
        SetupChatMembers();
        SetupButtons();
        SetupCenteredScrollView();
        SetupInputSystemActions();
        GameState.DayChanged += CheckForChatUnlocks;
    }

    private void CheckForChatUnlocks()
    {
        _npcConversationStates.Clear();
        _chat.Clear();
        RefreshChatList();
        List<(int, string)> chatIDs = new List<(int, string)>();
        foreach (var (npcName, npcData) in _chatDatas)
        {
            foreach (var chat in npcData.Chats)
            {
                if (!_gamestateChatData.ChatIDsAvailable.List.Contains(chat.ID) && !_gamestateChatData.DoneChatIDs.List.Contains(chat.ID))
                {
                    foreach(var unlock in chat.UnlocksNeeded)
                    {
                        if (!_gamestateChatData.ChatUnlocks.List.Contains(unlock))
                            continue;
                    }
                    foreach(var (requirementName, requirementValue) in chat.Requirements)
                    {
                        var found = _gamestateChatData.MetRequirements.List.Find((req) => req.RequirementName == requirementName && req.RequirementValue <= requirementValue);
                        if (found == null)
                            continue;
                    }
                    chatIDs.Add((chat.ID, npcName));
                }
            }
        }
        NPCData nPCData = _chatMemberList.selectedItem as NPCData;
        //don't even roll something or change a chance when there is nothing to unlock
        if (chatIDs.Count == 0){
            HandleNPCSelection(nPCData);
            return;
        }
        //roll if something actually gets unlocked, if not add to pitychance, because we are not evil :)
        if (UnityEngine.Random.Range(0f, 1f) > _gamestateChatData.NextChanceToUnlockSomething)
        {
            _gamestateChatData.NextChanceToUnlockSomething += 0.3f;
            HandleNPCSelection(nPCData);
            return;
        }
        var (unlockedID, name) = chatIDs[UnityEngine.Random.Range(0, chatIDs.Count)];
        _gamestateChatData.ChatIDsAvailable.List.Add(unlockedID);
        _gamestateChatData.NextChanceToUnlockSomething = 0.3f;
        HandleNPCSelection(nPCData);
        UIEvents.AddNotification(new(name, "Eine neue Nachricht!",-1,  () => {
            _currentNPC = name;
            UIEvents.ShowChatView.Invoke();
            var npcIndex = _allNPCs.FindIndex(npc => npc.CharacterName == name);
            _chatMemberList.SetSelection(npcIndex);
        }));
    }

    private void Awake()
    {
        _actions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        _actions.Enable();
    }
    private void OnDisable()
    {
        _actions.Disable();
    }
    private void SetupInputSystemActions()
    {
        _actions.UI.Submit.performed += (_) => ChooseAnswer(_centeredScrollView.SelectedIndex);
    }
    private void Update()
    {
        HandleUIInputs();
    }

    private void HandleUIInputs()
    {
        if (!_answerRequired)
            return;
        Vector2 uiNavigateVector = _actions.UI.Navigate.ReadValue<Vector2>();

        int newDirection = 0;
        if (uiNavigateVector.y < -0.2f)
            newDirection = 1;
        else if (uiNavigateVector.y > 0.2f)
            newDirection = -1;

        // If no input, reset the auto-repeat state
        if (newDirection == 0)
        {
            _scrollValues.CurrentDirection = 0;
            _scrollValues.Timer = 0f;
            _scrollValues.StepCount = 0;
            return;
        }

        // If direction changed (or starting fresh), immediately do a step and reset counters
        if (newDirection != _scrollValues.CurrentDirection)
        {
            _scrollValues.CurrentDirection = newDirection;
            _scrollValues.Timer = 0f;
            _scrollValues.StepCount = 0;
            ExecuteStep(_scrollValues.CurrentDirection);
            return;
        }

        // Otherwise, continue accumulating time
        _scrollValues.Timer += Time.deltaTime;
        // Use a different delay based on how many repeats have already been done:
        float currentDelay = (_scrollValues.StepCount == 0) ? _scrollValues.InitialDelay
                           : (_scrollValues.StepCount < _scrollValues.StepsBeforeFast ? _scrollValues.RepeatDelay : _scrollValues.FastRepeatDelay);

        // When timer exceeds the delay, perform another step
        if (_scrollValues.Timer >= currentDelay)
        {
            ExecuteStep(_scrollValues.CurrentDirection);
            _scrollValues.Timer = 0f;
            _scrollValues.StepCount++;
        }
    }

    private void ExecuteStep(int direction)
    {
        if (direction == 1)
            _centeredScrollView.NextIndex();
        else if (direction == -1)
            _centeredScrollView.PreviousIndex();
    }
    private void SetupCenteredScrollView()
    {
        _centeredScrollView = _root.Q<CenteredScrollView>();
        _centeredScrollView.style.display = DisplayStyle.None;
        _centeredScrollView.Textchosen += ChooseAnswer;
    }


    private void LoadChatData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Chat/dialogues");
        if (textAsset == null)
        {
            Debug.LogError("Chat dialogues file not found in Resources/Chat/dialogues");
            return;
        }
        string jsonString = textAsset.text;
        _chatDatas = JsonConvert.DeserializeObject<Dictionary<string, NPC>>(jsonString);
        if (_chatDatas == null)
            Debug.LogError("Failed to load Chatdata");
    }

    private void SetupButtons()
    {
        var closeButton = _root.Q<Button>("close-button");
        if (closeButton != null)
        {
            closeButton.clicked += () =>
            {
                UIEvents.HideChatView.Invoke();
                SoundManagerSingleton.Instance.PlaySound("Click");
            };
        }
        else
        {
            Debug.LogError("Close button not found in the UI.");
        }
    }

    private void SetupChatList()
    {
        _chatList = _root.Q<ListView>("chat-list");
        if (_chatList == null)
        {
            Debug.LogError("Chat list not found in the UI.");
            return;
        }
        _chatList.canStartDrag += (_) => false;
        _chatList.makeItem = () =>
        {
            var result = new VisualElement();
            result.AddToClassList("chat-list__item-container");
            var label = new Label();
            label.AddToClassList("chat-list__item");
            result.Add(label);
            return result;
        };
        _chatList.bindItem = (item, index) =>
        {
            var dialogItem = _chat[index];
            item.EnableInClassList("chat-list__item-container--npc", dialogItem.ItemType == ItemType.NPC);
            item.EnableInClassList("chat-list__item-container--player", dialogItem.ItemType == ItemType.Player);
            item.EnableInClassList("chat-list__item-container--system", dialogItem.ItemType == ItemType.System);
            for (int i = item.childCount; i < dialogItem.Texts.Count; i++)
            {
                var label = new Label();
                label.AddToClassList("chat-list__item");
                item.Add(label);
            }
            int childIndex = 0;
            foreach (Label label in item.Children())
            {
                if (childIndex >= dialogItem.Texts.Count)
                {
                    label.style.display = DisplayStyle.None;
                }
                else
                {
                    label.style.display = DisplayStyle.Flex;
                    label.text = dialogItem.Texts[childIndex];
                    label.EnableInClassList("chat-list__item--multiple", dialogItem.Texts.Count > 1 && childIndex > 0 && childIndex < dialogItem.Texts.Count - 1);
                    label.EnableInClassList("chat-list__item--first", dialogItem.Texts.Count > 1 && childIndex == 0);
                    label.EnableInClassList("chat-list__item--last", dialogItem.Texts.Count > 1 && childIndex == dialogItem.Texts.Count - 1);
                }
                label.EnableInClassList("chat-list__item--npc", dialogItem.ItemType == ItemType.NPC);
                label.EnableInClassList("chat-list__item--player", dialogItem.ItemType == ItemType.Player);
                label.EnableInClassList("chat-list__item--system", dialogItem.ItemType == ItemType.System);
                childIndex++;
            }
        };
        _chatList.itemsSource = _chat;
    }

    private void SetupVisualTreeAssets()
    {
        _chatMemberAsset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/ChatMember");
        if (_chatMemberAsset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/ChatMember");
        }
    }

    private void SetupChatMembers()
    {

        EnumerateAllCharacters();
        _chatMemberList = _root.Q<ListView>("chat-member-list");
        if (_chatMemberList == null)
        {
            Debug.LogError("Chat member list not found in the UI.");
            return;
        }
        _chatMemberList.canStartDrag += (_) => false;
        _chatMemberList.makeItem = () =>
        {
            var newListEntry = _chatMemberAsset?.Instantiate();
            if (newListEntry == null)
            {
                Debug.LogError("Failed to instantiate chat member asset.");
                return new Label("Error");
            }
            var newListEntryLogic = new ChatListEntryController();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };

        _chatMemberList.bindItem = (item, index) =>
        {
            (item.userData as ChatListEntryController)?.SetCharacterData(_allNPCs[index]);
        };

        _chatMemberList.fixedItemHeight = 55;
        _chatMemberList.itemsSource = _allNPCs;

        _chatMemberList.selectionChanged += (selectedItems) =>
        {
            if (selectedItems.FirstOrDefault() is not NPCData data){
                return;
            }
            HandleNPCSelection(data);
        };

        // Set initial selection (using a proper selection list)
        if (_allNPCs.Count > 0)
        {
            _chatMemberList.SetSelection(0);
        }
    }

    private void HandleNPCSelection(NPCData data)
    {
        _chat.Clear();
        _currentNPC = data.CharacterName;
        // Reset conversation state when switching NPCs
        if (!_npcConversationStates.TryGetValue(_currentNPC, out var state))
        {
            state = new NPCConversationState();
            _npcConversationStates[_currentNPC] = state;
            var _chatsOfThisNPC = _chatDatas[_currentNPC].Chats;
            var availableChatsOfThisNPC = _chatsOfThisNPC.FindAll((chat) => _gamestateChatData.ChatIDsAvailable.List.Contains(chat.ID));
            if (availableChatsOfThisNPC.Count == 0)
            {
                state.CurrentChatID = -1;
                AddOfflineMessage();
                return;
            }
            var chosenChatID = availableChatsOfThisNPC[UnityEngine.Random.Range(0, availableChatsOfThisNPC.Count)].ID;
            state.CurrentChatID = chosenChatID;
        }
        _chat.AddRange(state.History);
        RefreshChatList();
        if (state.CurrentChatID == -1)
            return;
        else
            ContinueConversation();

    }
    private void ContinueConversation()
    {
        var state = _npcConversationStates[_currentNPC];
        var chat = _chatDatas[_currentNPC].Chats[state.CurrentChatID];
        if (state.CurrentDialogueIndex == -1 || state.CurrentDialogueIndex >= chat.Dialogues.Count){
            FinishChat(chat);
            return;
        }

        var dialogue = chat.Dialogues[state.CurrentDialogueIndex];
        if (state.CurrentQuestionIndex < dialogue.Question.Count)
        {
            if(state.CurrentQuestionIndex == 0)
            {
                foreach (var unlock in dialogue.Unlocks)
                {
                    if (!_gamestateChatData.ChatUnlocks.List.Contains(unlock))
                    {
                        _gamestateChatData.ChatUnlocks.List.Add(unlock);
                        var split = unlock.Split(':');
                        if (split.Length > 1 && split[0] == "NPCUnlocked")
                        {
                            var unlockedNPC = split[1];
                            if (!_gamestateChatData.KnownNPCs.List.Contains(unlockedNPC))
                            { 
                                _gamestateChatData.KnownNPCs.List.Add(unlockedNPC);
                                //EnumerateAllCharacters();
                                NPCData[] npcs = Resources.LoadAll<NPCData>("NPCs");
                                foreach (var npc in npcs)
                                {
                                    if (npc.CharacterName == unlockedNPC)
                                    {
                                        _allNPCs.Add(npc);
                                        _chatMemberList.RefreshItems();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            AddNPCMessage(dialogue.Question[state.CurrentQuestionIndex]);
            state.CurrentQuestionIndex++;
            StartCoroutine(ContinueAfterDelay());
        }
        else if (dialogue.Answers != null && dialogue.Answers.Count > 0)
        {
            ShowPlayerAnswers(dialogue.Answers);
        }
        else
        {
            FinishChat(chat);
        }
    }

    private void FinishChat(Chat chat)
    {
        AddOfflineMessage();
        _gamestateChatData.DoneChatIDs.List.Add(chat.ID);
        _gamestateChatData.ChatIDsAvailable.List.Remove(chat.ID);
    }

    private void AddOfflineMessage()
    {
        var state = _npcConversationStates[_currentNPC];
        var item = new ChatHistoryItem(new() { $"{_currentNPC} ist offline" }, ItemType.System);
        _chat.Add(item);
        state.History.Add(item);
        RefreshChatList();
    }

    private IEnumerator ContinueAfterDelay()
    {
        while (!enabled)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        ContinueConversation();
    }

    private void AddNPCMessage(string message)
    {
        var state = _npcConversationStates[_currentNPC];
        if (state.CurrentQuestionIndex == 0)
        {
            var item = new ChatHistoryItem(new() { message }, ItemType.NPC);
            _chat.Add(item);
            state.History.Add(item);
        }
        else
        {
            _chat.Last().Texts.Add(message);
        }
        RefreshChatList();
    }

    private void ShowPlayerAnswers(List<Answer> answers)
    {
        _answerRequired = true;
        _centeredScrollView.SetTexts(answers.Select(ans => ans.Text).ToList());
        _centeredScrollView.style.display = DisplayStyle.Flex;
    }
    private void ChooseAnswer(int answerIndex)
    {
        if (!_answerRequired)
            return;
        var state = _npcConversationStates[_currentNPC];
        var currentChat = _chatDatas[_currentNPC].Chats[state.CurrentChatID];
        var currentDialogue = currentChat.Dialogues[state.CurrentDialogueIndex];
        var chosenAnswer = currentDialogue.Answers[answerIndex];
        var metRequirements = _gamestateChatData.MetRequirements.List;
        foreach (var p in chosenAnswer.Progress)
        {
            var foundIndex = metRequirements.FindIndex((metReq) => metReq.RequirementName == p.Topic);
            if (foundIndex >= 0)
            {
                metRequirements[foundIndex].RequirementValue += p.Change;
            }
            else
            {
                metRequirements.Add(new(p.Topic, p.Change));
            }
        }
        var message = chosenAnswer.Text;
        if (state.History.LastOrDefault() is not ChatHistoryItem lastItem || lastItem.ItemType != ItemType.Player)
        {
            var item = new ChatHistoryItem(new() { message }, ItemType.Player);
            _chat.Add(item);
            state.History.Add(item);
        }
        else
        {
            lastItem.Texts.Add(message);
        }
        RefreshChatList();
        _centeredScrollView.style.display = DisplayStyle.None;
        _answerRequired = false;
        state.CurrentDialogueIndex = chosenAnswer.NextQuestion.GetValueOrDefault(-1);
        state.CurrentQuestionIndex = 0;
        StartCoroutine(ContinueAfterDelay());
    }

    private void RefreshChatList()
    {
        _chatList.RefreshItems();
    }


    private void EnumerateAllCharacters()
    {
        _allNPCs = new List<NPCData>();
        NPCData[] npcs = Resources.LoadAll<NPCData>("NPCs");
        foreach (var npc in npcs)
        {
            if (_gamestateChatData.KnownNPCs.List.Contains(npc.CharacterName))
            {
                _allNPCs.Add(npc);
            }
        }
    }
}

public class ChatListEntryController
{
    private Label _nameLabel;
    private VisualElement _listEntry;
    private VisualElement _avatar;

    public void SetVisualElement(VisualElement newListEntry)
    {
        _listEntry = newListEntry;
        _nameLabel = _listEntry.Q<Label>("chat-member-name");
        _avatar = _listEntry.Q<VisualElement>("chat-member-avatar");
    }

    public void SetCharacterData(NPCData characterData)
    {
        if (characterData == null)
            return;
        if (_nameLabel != null)
            _nameLabel.text = characterData.CharacterName;
        if (_avatar != null)
            _avatar.style.backgroundImage = characterData.Avatar;
    }
}

[Serializable]
public class Dialogue
{
    public int ID;
    public List<string> Question;
    public List<Answer> Answers;
    public List<string> Unlocks;
}

[Serializable]
public class Answer
{
    public string Text;
    public int? NextQuestion;
    public List<ProgressChange> Progress;
}

[Serializable]
public class ProgressChange
{
    public string Topic;
    public int Change;
}

[Serializable]
public class Chat
{
    public int ID;
    public Dictionary<string, int> Requirements;
    public List<string> UnlocksNeeded;
    public List<Dialogue> Dialogues;
}

[Serializable]
public class NPC
{
    public List<string> Topics;
    public List<Chat> Chats;
}
public enum ItemType
{
    NPC,
    Player,
    System
}
public class ChatHistoryItem
{
    public List<string> Texts;
    public ItemType ItemType;
    public ChatHistoryItem(List<string> texts, ItemType itemType)
    {
        Texts = texts;
        ItemType = itemType;
    }
}
public class ScrollValues
{
    // Configure these in the Inspector (or set them directly)
    public float InitialDelay = 0.5f;      // Delay before auto-repeat begins
    public float RepeatDelay = 0.2f;       // Delay between repeats initially
    public float FastRepeatDelay = 0.05f;  // Delay after N steps for rapid repeat
    public int StepsBeforeFast = 3;        // Number of repeats before switching to fast

    public float Timer = 0f;
    public int StepCount = 0;
    // currentDirection: 1 means "NextIndex", -1 means "PreviousIndex", 0 means no active input
    public int CurrentDirection = 0;
}
public class NPCConversationState
{
    public int CurrentChatID;
    public int CurrentDialogueIndex;
    public int CurrentQuestionIndex;
    public readonly List<ChatHistoryItem> History = new();
}