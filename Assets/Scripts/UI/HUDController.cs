using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private VisualElement _root;

    private Label _currentDayLabel;
    private Label _ernteLabel;
    private Label _scoreLabel;
    private Label _getrocknetLabel; 
    public void Initialize(VisualElement root)
    {
        _root = root;
        GameState.DayChanged += OnDayChanged;
        GameState.ErnteAction += OnErnteAction;
        GameState.ScoreChanged += OnScoreChanged;
        GameState.GetrocknetChanged += OnGetrocknetChanged;
        //GameState.OnInventoryChanged += OnInventoryChanged;  // Abo für Inventaränderungen

        SetupButtons();
        SetupLabels();
        UpdateHUD();
    }
    private void SetupButtons()
    {
        Button advanceDayButton = _root.Q<Button>("advance-day-button");
        advanceDayButton.clicked += GameStateManagerSingleton.Instance.AdvanceDay;
        advanceDayButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        var openMainmenuButton = _root.Q<Button>("open-mainmenu-button");
        openMainmenuButton.clicked += () => UIEvents.ShowMainMenuView.Invoke();
        openMainmenuButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        var openChatButton = _root.Q<Button>("open-chat-button");
        openChatButton.clicked += () => UIEvents.ShowChatView.Invoke();
        openChatButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        var openQuestLogButton = _root.Q<Button>("open-questlog-button");
        openQuestLogButton.clicked += () => UIEvents.ShowQuestLog.Invoke();
        openQuestLogButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }
    private void SetupLabels()
    {
        _currentDayLabel = _root.Q<Label>("current-day-label");
        _ernteLabel = _root.Q<Label>("ernte-count-label");
        _scoreLabel = _root.Q<Label>("score-label");
        _getrocknetLabel = _root.Q<Label>("getrocknet-count-label");
    }

    private void UpdateHUD()
    {
        OnDayChanged();
        OnErnteAction();
        OnScoreChanged();
        OnGetrocknetChanged();
    }

    private void OnErnteAction()
    {
        _ernteLabel.text = $"{GameStateManagerSingleton.Instance.GameState.TreesCount}";
    }

    private void OnScoreChanged()
    {
        _scoreLabel.text = $"{GameStateManagerSingleton.Instance.GameState.Score}";
    }
    private void OnGetrocknetChanged()
    {
        _getrocknetLabel.text = $"{GameStateManagerSingleton.Instance.GameState.Getrocknet}";
    }
    private void OnDayChanged()
    {
        _currentDayLabel.text = $"Tag {GameStateManagerSingleton.Instance.GameState.CurrentDay}";
    }
}