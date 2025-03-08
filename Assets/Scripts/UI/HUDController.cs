using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private VisualElement _root;

    private Label _currentDayLabel;
    private Label _harvestedPlantCountLabel;
    private Label _finishedDriedCountLabel;
    private Label _scoreLabel;
    private GameState _gameState;
    public void Initialize(VisualElement root)
    {
        _root = root;
        _gameState = GameStateManagerSingleton.Instance.GameState;
        GameState.DayChanged += UpdateHUD;
        GameState.UpdateHUD += UpdateHUD;
        SetupButtons();
        SetupLabels();
        UpdateHUD();
    }
    private void SetupButtons()
    {
        var advanceDayButton = _root.Q<Button>("advance-day-button");
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
        _harvestedPlantCountLabel = _root.Q<Label>("harvested-plants-count-label");
        _finishedDriedCountLabel = _root.Q<Label>("dried-plants-count-label");
        _scoreLabel = _root.Q<Label>("score-label");

    }

    private void UpdateHUD()
    {
        _currentDayLabel.text = $"Tag {GameStateManagerSingleton.Instance.GameState.CurrentDay}";
        _harvestedPlantCountLabel.text = _gameState.HarvestedPlantCount.ToString();
        _finishedDriedCountLabel.text = _gameState.CompletedDriedPlantsCount.ToString();
        _scoreLabel.text = _gameState.CurrentScore.ToString();
    }



}