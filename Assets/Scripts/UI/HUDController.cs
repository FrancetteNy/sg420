using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private VisualElement _root;
    UIManager _manager;
    private Label _currentDayLabel;
    private Label _harvestedPlantCountLabel;
    private Label _finishedDriedCountLabel;
    private Label _scoreLabel;

    private Button _changeToMainRoomButton;
    private Button _changeToDryingRoomButton;

    private GameState _gameState;
    public void Initialize(VisualElement root)
    {
        _root = root;
        _gameState = GameStateManagerSingleton.Instance.GameState;
        GameState.DayChanged += UpdateHUD;
        GameState.UpdateHUD += UpdateHUD;
        _manager = FindAnyObjectByType<UIManager>();
        SetupButtons();
        SetupLabels();
        UpdateHUD();
    }
    private void SetupButtons()
    {
        var advanceDayButton = _root.Q<Button>("advance-day-button");
        advanceDayButton.clicked += GameStateManagerSingleton.Instance.AdvanceDay;
        advanceDayButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        
        var inventarButton = _root.Q<Button>("inventar-button");
        inventarButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        inventarButton.clicked += () => UIEvents.ShowInventar.Invoke();

        var shopButton = _root.Q<Button>("shop-button");
        shopButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        shopButton.clicked += () => UIEvents.ShowShop.Invoke();

        

        _changeToMainRoomButton = _root.Q<Button>("change-to-main-room-button");
        _changeToMainRoomButton.clicked += _manager.ChangeToMainRoom;
        _changeToMainRoomButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _changeToDryingRoomButton = _root.Q<Button>("change-to-drying-room-button");
        _changeToDryingRoomButton.clicked += _manager.ChangeToDryingRoom;
        _changeToDryingRoomButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
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
        if (_manager.IsReadyToChangeRoom && _manager.DryingRoomIsActive)
        {
            _changeToMainRoomButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            _changeToMainRoomButton.style.display = DisplayStyle.None;
        }
        if(_manager.IsReadyToChangeRoom && _manager.MainRoomIsActive)
        {
            _changeToDryingRoomButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            _changeToDryingRoomButton.style.display = DisplayStyle.None;
        }
    }



}