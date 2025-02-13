using Newtonsoft.Json;
using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatViewController : MonoBehaviour
{
    #region UI Elements
    private VisualElement _root;
    private ListView _chatMemberList;
    private VisualTreeAsset _chatMemberAsset;
    private ListView _chatList;
    private CenteredScrollView _centeredScrollView;
    #endregion

    #region Data References
    private ChatData _gamestateChatData;
    private Dictionary<string, NPC> _chatDatas;
    private List<NPCData> _allNPCs = new();
    #endregion

    #region Conversation State
    private string _currentNPC;
    private bool _answerRequired;
    private readonly Dictionary<string, NPCConversationState> _npcConversationStates = new();
    private readonly List<ChatHistoryItem> _chat = new();
    #endregion

    #region Input System
    private InputSystem_Actions _actions;
    private readonly ScrollValues _scrollValues = new();
    #endregion


    #region Unity Lifecycle
    private void Awake() => _actions = new InputSystem_Actions();
    private void OnEnable() => _actions.Enable();
    private void OnDisable() => _actions.Disable();

    private void Update() => HandleScrollInput();

    private void OnDestroy()
    {
        GameState.DayChanged -= CheckForChatUnlocks;
        _centeredScrollView.Textchosen -= ChooseAnswer;
    }
    #endregion

    #region Initialization
    public void Initialize(VisualElement root)
    {
        _root = root;
        _gamestateChatData = GameStateManagerSingleton.Instance.GameState.ChatData;

        SetupDependencies();
        InitializeUIComponents();
        RegisterEventHandlers();
    }

    private void SetupDependencies()
    {
        LoadChatData();
        EnumerateKnownCharacters();
    }

    private void InitializeUIComponents()
    {
        SetupCenteredScrollView();
        SetupChatList();
        SetupChatMembersList();
        SetupControlButtons();
    }

    private void RegisterEventHandlers()
    {
        GameState.DayChanged += CheckForChatUnlocks;
        _actions.UI.Submit.performed += _ => ChooseAnswer(_centeredScrollView.SelectedIndex);
    }
    #endregion

    #region Data Management
    private void LoadChatData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Chat/dialogues");
        if (textAsset == null)
        {
            Debug.LogError("Chat dialogues file not found in Resources/Chat/dialogues");
            return;
        }

        _chatDatas = JsonConvert.DeserializeObject<Dictionary<string, NPC>>(textAsset.text);
        if (_chatDatas == null)
            Debug.LogError("Failed to load Chatdata");
    }

    private void EnumerateKnownCharacters()
    {
        _allNPCs.Clear();
        var knownNPCs = _gamestateChatData.KnownNPCs.List;
        _allNPCs.AddRange(Resources.LoadAll<NPCData>("NPCs").Where(n => knownNPCs.Contains(n.CharacterName)));
    }
    #endregion

    #region UI Setup
    private void SetupChatList()
    {
        _chatList = _root.Q<ListView>("chat-list");
        if (_chatList == null)
        {
            Debug.LogError("Chat list not found in the UI.");
            return;
        }

        _chatList.canStartDrag += _ => false;
        _chatList.makeItem = CreateChatListItem;
        _chatList.bindItem = BindChatListItem;
        _chatList.itemsSource = _chat;
    }

    private VisualElement CreateChatListItem()
    {
        var item = new VisualElement().WithClass("chat-list__item-container");
        return item;
    }

    private void BindChatListItem(VisualElement item, int index)
    {
        var dialogItem = _chat[index];
        UpdateItemContainerStyles(item, dialogItem.ItemType);
        UpdateItemLabels(item, dialogItem.Texts, dialogItem.ItemType);
    }

    private void SetupChatMembersList()
    {
        _chatMemberList = _root.Q<ListView>("chat-member-list");
        if (_chatMemberList == null)
        {
            Debug.LogError("Chat member list not found in the UI.");
            return;
        }

        _chatMemberAsset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/ChatMember");
        if (_chatMemberAsset == null)
            Debug.LogError("Failed to load ChatMember asset");

        ConfigureChatMembersList();
        InitializeChatMembersSelection();
    }

    private void ConfigureChatMembersList()
    {
        _chatMemberList.canStartDrag += _ => false;
        _chatMemberList.makeItem = CreateChatMemberListItem;
        _chatMemberList.bindItem = (item, index) =>
            (item.userData as ChatListEntryController)?.SetCharacterData(_allNPCs[index]);
        _chatMemberList.fixedItemHeight = 55;
        _chatMemberList.itemsSource = _allNPCs;
        _chatMemberList.selectionChanged += HandleNPCSelectionChange;
    }

    private VisualElement CreateChatMemberListItem()
    {
        var item = (_chatMemberAsset?.Instantiate() as VisualElement) ?? new Label("Error");
        item.userData = new ChatListEntryController();
        ((ChatListEntryController)item.userData).SetVisualElement(item);
        return item;
    }

    private void InitializeChatMembersSelection()
    {
        if (_allNPCs.Count > 0)
            _chatMemberList.SetSelection(0);
    }

    private void SetupCenteredScrollView()
    {
        _centeredScrollView = _root.Q<CenteredScrollView>();
        _centeredScrollView.style.display = DisplayStyle.None;
        _centeredScrollView.Textchosen += ChooseAnswer;
    }

    private void SetupControlButtons()
    {
        var closeButton = _root.Q<Button>("close-button");
        if (closeButton == null)
        {
            Debug.LogError("Close button not found in the UI.");
            return;
        }

        closeButton.clicked += () =>
        {
            UIEvents.HideChatView.Invoke();
            SoundManagerSingleton.Instance.PlaySound("Click");
        };
    }
    #endregion

    #region Conversation Management
    private void HandleNPCSelectionChange(IEnumerable<object> selectedItems)
    {
        if (selectedItems.FirstOrDefault() is NPCData data)
        {
            HandleNPCSelection(data);
        }
    }

    private void HandleNPCSelection(NPCData data)
    {
        _currentNPC = data.CharacterName;
        if (!_npcConversationStates.TryGetValue(data.CharacterName, out var state))
        {
            state = InitializeNewConversationState(data.CharacterName);
        }
        var chat = state.CurrentChat;
        HideAnswerDialog();
        _chat.Clear();
        LoadConversationHistory(state);
        RefreshChatList();

        if (state.CurrentChatID != null)
            ContinueConversation(state, chat);
    }


    private NPCConversationState InitializeNewConversationState(string npcName)
    {
        var state = new NPCConversationState();
        _npcConversationStates[npcName] = state;
        _npcConversationStates[npcName].NPCName = npcName;
        var availableChats = GetAvailableChatsForNPC(npcName);

        if (availableChats.Count > 0)
        {
            state.CurrentChatID = availableChats[UnityEngine.Random.Range(0, availableChats.Count)].ID;
        }
        else
        {
            state.CurrentChatID = null;
            AddSystemMessage($"{npcName} ist offline", state);
        }
        state.CurrentChat = GetCurrentChat(state, npcName);
        return state;
    }

    private List<Chat> GetAvailableChatsForNPC(string npcName)
    {
        return _chatDatas[npcName].Chats
            .FindAll(chat => _gamestateChatData.ChatIDsAvailable.List.Contains(chat.ID));
    }

    private void LoadConversationHistory(NPCConversationState state)
    {
        _chat.AddRange(state.History);
    }

    private void ContinueConversation(NPCConversationState state, Chat chat)
    {
        if (IsChatComplete(state, chat))
        {
            FinalizeChat(chat, state);
            return;
        }
        ProcessCurrentDialogue(state, chat);
    }

    private Chat GetCurrentChat(NPCConversationState state, string npcName)
    {
        return _chatDatas[npcName].Chats.Find(chat => chat.ID == state.CurrentChatID);
    }

    private void ProcessCurrentDialogue(NPCConversationState state, Chat chat)
    {
        var dialogue = chat.Dialogues[state.CurrentDialogueIndex];

        if (state.CurrentQuestionIndex < dialogue.Question.Count)
        {
            ProcessNPCQuestion(state, dialogue);
            StartCoroutine(ContinueAfterDelay(state, chat));
        }
        else if (dialogue.Answers?.Count > 0)
        {
            if (state == _npcConversationStates[_currentNPC])
            {
                ShowPlayerAnswers(dialogue.Answers);
            }
        }
        else
        {
            FinalizeChat(chat, state);
        }
    }

    private void ProcessNPCQuestion(NPCConversationState state, Dialogue dialogue)
    {
        if (state.CurrentQuestionIndex == 0)
        {
            HandleFirstQuestion(dialogue);
        }

        AddNPCResponse(dialogue.Question[state.CurrentQuestionIndex], state);
        state.CurrentQuestionIndex++;
    }

    private void HandleFirstQuestion(Dialogue dialogue)
    {
        foreach (var unlock in dialogue.Unlocks)
        {
            ProcessUnlock(unlock);
        }
    }

    private void AddNPCResponse(string message, NPCConversationState state)
    {
        var itemType = ItemType.NPC;

        if (state.CurrentQuestionIndex == 0)
        {
            AddNewChatItem(message, itemType, state);
        }
        else
        {
            AppendToLastChatItem(message, state);
        }
    }

    private void ShowPlayerAnswers(List<Answer> answers)
    {
        _answerRequired = true;
        _centeredScrollView.SetTexts(answers.Select(ans => ConstructPlayerAnswer(ans)).ToList());
        _centeredScrollView.style.display = DisplayStyle.Flex;
    }

    private (string, bool) ConstructPlayerAnswer(Answer answer)
    {
        var result = (answer.Text, true);
        foreach (var requirement in answer.Requirements)
        {
            var reqData = _gamestateChatData.MetRequirements.List.Find(metReq => metReq.RequirementName == requirement.Key);
            if (reqData == null || reqData.RequirementValue < requirement.Value)
            {
                result.Item2 = false;
                result.Item1 += $"[{requirement.Key}: {requirement.Value}]";
            }
        }
        return result;
    }

    private void FinalizeChat(Chat chat, NPCConversationState state)
    {
        if (_gamestateChatData.DoneChatIDs.List.Contains(chat.ID))
            return;
        AddSystemMessage($"{state.NPCName} ist offline", state);
        _gamestateChatData.DoneChatIDs.List.Add(chat.ID);
        _gamestateChatData.ChatIDsAvailable.List.Remove(chat.ID);
    }
    #endregion

    #region Input Handling
    private void HandleScrollInput()
    {
        if (!_answerRequired)
            return;

        Vector2 input = _actions.UI.Navigate.ReadValue<Vector2>();
        int direction = GetScrollDirection(input);

        if (direction == 0)
        {
            ResetScrollState();
            return;
        }

        UpdateScrollNavigation(direction);
    }

    private int GetScrollDirection(Vector2 input)
    {
        if (input.y < -0.2f)
            return 1;
        if (input.y > 0.2f)
            return -1;
        return 0;
    }

    private void ResetScrollState()
    {
        _scrollValues.CurrentDirection = 0;
        _scrollValues.Timer = 0f;
        _scrollValues.StepCount = 0;
    }

    private void UpdateScrollNavigation(int direction)
    {
        if (direction != _scrollValues.CurrentDirection)
        {
            UpdateScrollDirection(direction);
            return;
        }

        _scrollValues.Timer += Time.deltaTime;
        if (_scrollValues.Timer >= GetCurrentDelay())
        {
            ExecuteScrollStep(direction);
            ResetScrollTimer();
        }
    }

    private float GetCurrentDelay()
    {
        return _scrollValues.StepCount switch
        {
            0 => _scrollValues.InitialDelay,
            < ScrollValues.StepsBeforeFast => _scrollValues.RepeatDelay,
            _ => _scrollValues.FastRepeatDelay
        };
    }

    private void UpdateScrollDirection(int direction)
    {
        _scrollValues.CurrentDirection = direction;
        _scrollValues.Timer = 0f;
        _scrollValues.StepCount = 0;
        ExecuteScrollStep(direction);
    }

    private void ExecuteScrollStep(int direction)
    {
        if (direction == 1)
        {
            _centeredScrollView.NextIndex();
        }
        else if (direction == -1)
        {
            _centeredScrollView.PreviousIndex();
        }
    }

    private void ResetScrollTimer()
    {
        _scrollValues.Timer = 0f;
        _scrollValues.StepCount++;
    }
    #endregion

    #region Chat Operations
    private void ChooseAnswer(int answerIndex)
    {
        if (!_answerRequired || !_centeredScrollView.IsIndexEnabled)
            return;

        var state = _npcConversationStates[_currentNPC];
        var chat = state.CurrentChat;
        var answer = chat.Dialogues[state.CurrentDialogueIndex].Answers[answerIndex];

        ProcessAnswerEffects(answer);
        RecordPlayerResponse(answer.Text, state);
        UpdateConversationState(state, answer);

        _centeredScrollView.style.display = DisplayStyle.None;
        _answerRequired = false;
        StartCoroutine(ContinueAfterDelay(state, chat));
    }

    private void ProcessAnswerEffects(Answer answer)
    {
        foreach (var progress in answer.Progress)
        {
            UpdateProgressRequirements(progress);
        }
        foreach (var unlock in answer.Unlocks)
        {
            ProcessUnlock(unlock);
        }
    }

    private void UpdateProgressRequirements(ProgressChange progress)
    {
        var requirements = _gamestateChatData.MetRequirements.List;
        var existing = requirements.Find(r => r.RequirementName == progress.Topic);

        if (existing != null)
        {
            existing.RequirementValue += progress.Change;
        }
        else
        {
            requirements.Add(new MetRequirement(progress.Topic, progress.Change));
        }
    }

    private void RecordPlayerResponse(string message, NPCConversationState state)
    {
        var lastItem = _chat.LastOrDefault();

        if (lastItem?.ItemType == ItemType.Player)
        {
            lastItem.Texts.Add(message);
        }
        else
        {
            AddNewChatItem(message, ItemType.Player, state);
        }

        RefreshChatList();
    }

    private void UpdateConversationState(NPCConversationState state, Answer answer)
    {
        state.CurrentDialogueIndex = answer.NextQuestion ?? -1;
        state.CurrentQuestionIndex = 0;
    }
    #endregion

    #region UI Helpers
    private void UpdateItemContainerStyles(VisualElement container, ItemType type)
    {
        container.EnableInClassList("chat-list__item-container--npc", type == ItemType.NPC);
        container.EnableInClassList("chat-list__item-container--player", type == ItemType.Player);
        container.EnableInClassList("chat-list__item-container--system", type == ItemType.System);
    }

    private void UpdateItemLabels(VisualElement container, List<string> texts, ItemType type)
    {
        for (int i = container.childCount; i < texts.Count; i++)
        {
            container.Add(new Label().WithClass("chat-list__item"));
        }
        int childIndex = 0;
        foreach (Label label in container.Children())
        {
            bool hasText = childIndex < texts.Count;
            label.style.display = hasText ? DisplayStyle.Flex : DisplayStyle.None;

            if (hasText)
            {
                UpdateLabelStyle(label, texts, childIndex, type);
                label.text = texts[childIndex];
            }

            childIndex++;
        }
    }

    private void UpdateLabelStyle(Label label, List<string> texts, int index, ItemType type)
    {
        label.EnableInClassList("chat-list__item--multiple", texts.Count > 1 && 0 < index && index < texts.Count - 1);
        label.EnableInClassList("chat-list__item--first", texts.Count > 1 && index == 0);
        label.EnableInClassList("chat-list__item--last", texts.Count > 1 && index == texts.Count - 1);
        label.EnableInClassList("chat-list__item--npc", type == ItemType.NPC);
        label.EnableInClassList("chat-list__item--player", type == ItemType.Player);
        label.EnableInClassList("chat-list__item--system", type == ItemType.System);
    }

    private void AddNewChatItem(string message, ItemType type, NPCConversationState state)
    {
        var item = new ChatHistoryItem(new List<string> { message }, type);
        state.History.Add(item);
        if (state == _npcConversationStates[_currentNPC])
        {
            _chat.Add(item);
            RefreshChatList();
        }
    }

    private void AppendToLastChatItem(string message, NPCConversationState state)
    {
        state.History.Last().Texts.Add(message);
        RefreshChatList();
    }

    private void AddSystemMessage(string message, NPCConversationState state)
    {
        AddNewChatItem(message, ItemType.System, state);
    }

    private void RefreshChatList()
    {
        _chatList.RefreshItems();
    }

    private bool IsChatComplete(NPCConversationState state, Chat chat)
    {
        return state.CurrentDialogueIndex == -1 || state.CurrentDialogueIndex >= chat.Dialogues.Count;
    }

    private void HideAnswerDialog()
    {
        _centeredScrollView.style.display = DisplayStyle.None;
    }

    #endregion

    #region Coroutines
    private IEnumerator ContinueAfterDelay(NPCConversationState state, Chat chat)
    {
        while (!enabled)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        ContinueConversation(state, chat);
    }
    #endregion

    #region Event Handlers
    private void CheckForChatUnlocks()
    {
        _npcConversationStates.Clear();
        _chat.Clear();
        RefreshChatList();
        List<(string, string)> chatIDs = new List<(string, string)>();
        foreach (var (npcName, npcData) in _chatDatas)
        {
            if (!_gamestateChatData.KnownNPCs.List.Contains(npcName))
                continue;
            foreach (var chat in npcData.Chats)
            {
                if (!_gamestateChatData.ChatIDsAvailable.List.Contains(chat.ID) && !_gamestateChatData.DoneChatIDs.List.Contains(chat.ID))
                {
                    var requirementsMet = true;
                    foreach (var unlock in chat.UnlocksNeeded)
                    {
                        if (!_gamestateChatData.ChatUnlocks.List.Contains(unlock))
                        {
                            requirementsMet = false;
                            break;
                        }
                    }
                    if (!requirementsMet)
                    {
                        continue;
                    }
                    foreach (var (requirementName, requirementValue) in chat.Requirements)
                    {
                        var found = _gamestateChatData.MetRequirements.List.Find((req) => req.RequirementName == requirementName && req.RequirementValue <= requirementValue);
                        if (found == null)
                        {
                            requirementsMet = false;
                            break;
                        }
                    }
                    if (requirementsMet)
                        chatIDs.Add((chat.ID, npcName));
                }
            }
        }
        NPCData nPCData = _chatMemberList.selectedItem as NPCData;
        //don't even roll something or change a chance when there is nothing to unlock
        if (chatIDs.Count == 0)
        {
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
        UIEvents.AddNotification(new(name, "Eine neue Nachricht!", -1, () =>
        {
            _currentNPC = name;
            UIEvents.ShowChatView.Invoke();
            var npcIndex = _allNPCs.FindIndex(npc => npc.CharacterName == name);
            _chatMemberList.SetSelection(npcIndex);
        }));
    }


    private void ProcessUnlock(string unlock)
    {
        if (!_gamestateChatData.ChatUnlocks.List.Contains(unlock))
        {
            _gamestateChatData.ChatUnlocks.List.Add(unlock);
            var split = unlock.Split(':');
            if (split.Length == 2)
            {
                switch (split[0])
                {
                    case "NPCUnlocked":
                        UnlockNPC(split[1]);

                        break;
                    case "QuestUnlocked":
                        print($"unlocked Quest {split[1]}");
                        break;
                    case "EnzyklopädieUnlocked":
                        print($"unlocked enzyklopädie eintrage {split[1]}");
                        break;
                    case "MechanicUnlocked":
                        print($"unlocked mechanic {split[1]}");
                        break;
                    default:
                        print($"unkown unlock {split[0]}: {split[1]}");
                        break;
                }
            }
        }
    }

    private void UnlockNPC(string npcName)
    {
        if (!_gamestateChatData.KnownNPCs.List.Contains(npcName))
        {
            _gamestateChatData.KnownNPCs.List.Add(npcName);
            NPCData[] npcs = Resources.LoadAll<NPCData>("NPCs");
            foreach (var npc in npcs)
            {
                if (npc.CharacterName == npcName)
                {
                    _allNPCs.Add(npc);
                    _chatMemberList.RefreshItems();
                    break;
                }
            }
        }
    }
    #endregion

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
    public Dictionary<string, int> Requirements;
    public int? NextQuestion;
    public List<ProgressChange> Progress;
    public List<string> Unlocks;
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
    public string ID;
    public Dictionary<string, int> Requirements;
    public List<string> UnlocksNeeded;
    public List<Dialogue> Dialogues;
}

[Serializable]
public class NPC
{
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
    public const int StepsBeforeFast = 3;        // Number of repeats before switching to fast

    public float Timer = 0f;
    public int StepCount = 0;
    // currentDirection: 1 means "NextIndex", -1 means "PreviousIndex", 0 means no active input
    public int CurrentDirection = 0;
}
public class NPCConversationState
{
    public string NPCName;
    public string CurrentChatID;
    public int CurrentDialogueIndex;
    public int CurrentQuestionIndex;
    public readonly List<ChatHistoryItem> History = new();
    public Chat CurrentChat;
}


public static class VisualElementExtensions
{
    public static T WithClass<T>(this T element, string className) where T : VisualElement
    {
        element.AddToClassList(className);
        return element;
    }
}