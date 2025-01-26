using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatViewController : MonoBehaviour
{
    private VisualElement _root;
    private ListView _chatMemberList;
    private VisualTreeAsset _chatMemberAsset;
    private List<NPCData> _allNPCs;

    private ListView _chatList;
    private List<string> _chat = new();
    public void Initialize(VisualElement root)
    { 
        _root = root;
        SetupVisualTreeAssets();
        SetupChatList();
        SetupChatMembers();
    }

    private void SetupChatList()
    {
        _chatList = _root.Q<ListView>("chat-list");
        _chatList.makeItem = () =>
        {
            //var newListEntry = _chatMemberAsset.Instantiate();
            //var newListEntryLogic = new ChatListEntryController();

            //newListEntry.userData = newListEntryLogic;

            //newListEntryLogic.SetVisualElement(newListEntry);

            //return newListEntry;
            return new Label();
        };

        _chatList.bindItem = (item, index) =>
        {
            (item as Label).text = _chat[index];
            //(item.userData as ChatListEntryController)?.SetCharacterData(_allNPCs[index]);
        };

        //_chatMemberList.fixedItemHeight = 55;

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
        _chatMemberList.makeItem = () =>
        {
            var newListEntry = _chatMemberAsset.Instantiate();
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
        _chatMemberList.selectionChanged += (selectedItems) => { 
            NPCData data = selectedItems.First() as NPCData;
            if (data.CharacterName == "Test1")
            {
                _chat.Clear();
                _chat.AddRange(new List<string>(){"First", "Second" });
            }
            else
            {
                _chat.Clear();
                _chat.AddRange(new List<string> { "WOW", "AMAZING!" });
            }
            _chatList.RefreshItems();
        };
        _chatMemberList.SetSelection(0);
    }

    private void EnumerateAllCharacters()
    {
        _allNPCs = new List<NPCData>();
        _allNPCs.AddRange(Resources.LoadAll<NPCData>("NPCs"));
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
        _nameLabel.text = characterData.CharacterName;
        _avatar.style.backgroundImage = characterData.Avatar;
    }

}