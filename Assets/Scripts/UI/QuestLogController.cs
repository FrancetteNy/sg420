using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestLogController : MonoBehaviour
{

    private VisualElement _root;
    private VisualElement _questBox;
    private Label _questTitle;
    private Label _questDescription;

    private List<QuestObjectiveElement> _objectiveLists;
    private List<QuestWithObjectiveIndex> _activeQuestsList;
    private ListView _questListView;   

    private GameState _gameState;


    
    private class QuestObjectiveElement : VisualElement
    {
        public Label ObjectiveTitle;
        public Label ObjectiveDescription;
        public QuestObjectiveElement(string objectiveTitle, string objectiveDescription) : base()
        {
            ObjectiveTitle = new Label(objectiveTitle);
            ObjectiveDescription = new Label(objectiveDescription);
            ObjectiveTitle.AddToClassList("h2");
            ObjectiveDescription.AddToClassList("h3");
            ObjectiveDescription.AddToClassList("objective-description");
            this.AddToClassList("submenu-window");
            this.AddToClassList("objective");
            this.Add(ObjectiveTitle);
            this.Add(ObjectiveDescription);
        }
    }
    public void Initialize(VisualElement root)
    {
        _root = root;

        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideQuestLog.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _objectiveLists = new();
        SetupQuestBox();
        _gameState = GameStateManagerSingleton.Instance.GameState;
        _activeQuestsList = _gameState.ActiveQuestsList.List;
        SetUpListView();

    }

    public void RefreshView()
    {
        if (_activeQuestsList == null || _activeQuestsList.Count == 0)
        {
            _questBox.style.display = DisplayStyle.None;
            return;
        }
        if (_activeQuestsList.Count <= _questListView.selectedIndex || _questListView.selectedIndex < 0)
            return;
        LoadEntry(_activeQuestsList[_questListView.selectedIndex]);
    }

    private void SetupQuestBox()
    {
        var contentContainer = _root.Q<VisualElement>("entry-scroll-view").Q<VisualElement>("unity-content-container");
        _questBox = contentContainer.Q<VisualElement>("quest-box");
        _questTitle = contentContainer.Q<Label>("quest-title");
        _questDescription = contentContainer.Q<Label>("quest-description");
        _questBox.style.display = DisplayStyle.None;
    }

    void LoadEntry(QuestWithObjectiveIndex quest)
    {
        _questBox.style.display = DisplayStyle.Flex;
        _questTitle.text = quest.Quest.Questname;
        _questDescription.text = quest.Quest.Questdescription;

        //Handle already existing objective elements
        for (int i = 0; i < _objectiveLists.Count; i++)
        {
            var element = _objectiveLists[i];

            if (i < quest.Quest.Objectives.Count)
            {
                UpdateObjectiveElement(element, quest.Quest.Objectives[i], quest.ObjectiveIndex, i);
            }
            else
            {
                element.style.display = DisplayStyle.None;
            }
        }
        //Create new ones if necessary
        for (int i = _objectiveLists.Count; i < quest.Quest.Objectives.Count; i++)
        {
            var objective = quest.Quest.Objectives[i];
            var element = new QuestObjectiveElement(objective.ObjectiveName, objective.ObjectiveDescription);
            SetObjectiveState(element, quest.ObjectiveIndex, i);
            _questBox.Add(element);
            _objectiveLists.Add(element);
        }
    }

    private void UpdateObjectiveElement(QuestObjectiveElement element, Objective objective, int currentObjectiveIndex, int index)
    {
        element.style.display = DisplayStyle.Flex;
        element.ObjectiveTitle.text = objective.ObjectiveName;
        element.ObjectiveDescription.text = objective.ObjectiveDescription;
        SetObjectiveState(element, currentObjectiveIndex, index);
    }

    private void SetObjectiveState(QuestObjectiveElement element, int currentObjectiveIndex, int index)
    {
        element.EnableInClassList("objective--done", currentObjectiveIndex < index);
        element.EnableInClassList("objective--active", currentObjectiveIndex == index);
        element.EnableInClassList("objective--future", currentObjectiveIndex > index);
    }

    void SetUpListView()
    {
        _questListView = _root.Query<ListView>("quest-listview");
        _questListView.itemsSource = _activeQuestsList;
        _questListView.makeItem = MakeListViewItem;

        _questListView.bindItem = (VisualElement element, int index) =>
        {
            QuestWithObjectiveIndex data = _activeQuestsList[index];
            Label button = element as Label;
            button.text = data.Quest.Questname;
        };
        _questListView.selectionChanged += OnSelectionChanged;
        if (_activeQuestsList.Count > 0)
        {
            _questListView.selectedIndex = 0;
        }
    }

    private void OnSelectionChanged(IEnumerable<object> enumerable)
    {
        if (enumerable.FirstOrDefault() is QuestWithObjectiveIndex quest)
        {
            LoadEntry(quest);
        }
    }

    private VisualElement MakeListViewItem()
    {
        var result = new Label();
        result.AddToClassList("unity-button");
        return result;
    }
}

