using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private VisualElement _root;
    UIManager _manager;
    private Label _currentDayLabel;
    private Label _harvestedPlantCountLabel, _finishedDriedCountLabel, _scoreLabel, _moneyLabel;

    private Button _changeToMainRoomButton, _changeToDryingRoomButton,
        _advanceDayButton, _shopButton, _inventoryButton,
        _openMainmenuButton, _openChatButton, _openQuestLogButton;

    private GameState _gameState;

    private bool _isInitialized;
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
        _isInitialized = true;
    }

    private void OnEnable()
    {
        if (GameStateManagerSingleton.Instance.IsGameLoaded && _isInitialized)
        {

            if (!_gameState.OnboardingDoneData.HudOnboardingIsDone)
            {
                _gameState.OnboardingDoneData.HudOnboardingIsDone = true;
                StartOnboarding();
            }
        }
    }

    private void StartOnboarding()
    {
        UIEvents.ShowOnboardingView(new() {
            new(null, "Willkommen", "Herzlich willkommen! Du bist der Erbe eines sehr beliebten Growers: Deinem Großvater. Er hat dir seinen Growroom und einige Bücher vererbt."),
            new(null, "Willkommen", "Dir stehen bis zu vier Töpfe zur Verfügung, in denen du Cannabis anbauen kannst!"),
            new(null, "Willkommen", "Dir sollte in ein paar Tagen Lara Gruen schreiben und dich einweisen, bis dahin kannst du dich schonmal umschauen."),
            new(null, "Willkommen", "Übrigens: Alle Fenster kannst du einfach mit Esc schließen! (außer dieses hier :) )"),
            new(_advanceDayButton, "Nächster Tage", "Hier kannst du den nächsten Tag starten"),
            new(_currentDayLabel, "Aktueller Tag", "Hier siehst du welcher Tag heute ist"),
            new(_harvestedPlantCountLabel.parent, "Geerntete Pflanzen", "Hier siehst du wieviele ungetrocknete Pflanzen du hast"),
            new(_finishedDriedCountLabel.parent, "Getrocknete Pflanzen", "Hier siehst du wieviele getrocknete Pflanzen du hast"),
            new(_scoreLabel.parent, "Punkte", "Hier siehst wieviele Punkte du gesammelt hast"),
            new(_moneyLabel.parent, "Geld", "Hier siehst du wieviel Geld du hast"),
            new(_openMainmenuButton, "Hauptmenü", "Hier kannst du das Hauptmenü öffnen, um zu speichern, zu laden oder das Spiel zu beenden."),
            new(_openChatButton, "Chat", "Hier kannst du mit Freunden chatten"),
            new(_openQuestLogButton, "Questlog", "Hier siehst du alle aktuellen Quests"),
            new(_shopButton, "Shop", "Hier kannst du Dinge kaufen"),
            new(_inventoryButton, "Inventar", "Hier kannst du dein Inventar anschauen"),
        });
    }

    private void SetupButtons()
    {
        _advanceDayButton = _root.Q<Button>("advance-day-button");
        _advanceDayButton.clicked += GameStateManagerSingleton.Instance.AdvanceDay;
        _advanceDayButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        
        _inventoryButton = _root.Q<Button>("inventar-button");
        _inventoryButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _inventoryButton.clicked += () => UIEvents.ShowInventar.Invoke();

        _shopButton = _root.Q<Button>("shop-button");
        _shopButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _shopButton.clicked += () => UIEvents.ShowShop.Invoke();

        

        _changeToMainRoomButton = _root.Q<Button>("change-to-main-room-button");
        _changeToMainRoomButton.clicked += _manager.ChangeToMainRoom;
        _changeToMainRoomButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _changeToDryingRoomButton = _root.Q<Button>("change-to-drying-room-button");
        _changeToDryingRoomButton.clicked += _manager.ChangeToDryingRoom;
        _changeToDryingRoomButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _openMainmenuButton = _root.Q<Button>("open-mainmenu-button");
        _openMainmenuButton.clicked += () => UIEvents.ShowMainMenuView.Invoke();
        _openMainmenuButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _openChatButton = _root.Q<Button>("open-chat-button");
        _openChatButton.clicked += () => UIEvents.ShowChatView.Invoke();
        _openChatButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _openQuestLogButton = _root.Q<Button>("open-questlog-button");
        _openQuestLogButton.clicked += () => UIEvents.ShowQuestLog.Invoke();
        _openQuestLogButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }

    private void SetupLabels()
    {
        _currentDayLabel = _root.Q<Label>("current-day-label");
        _harvestedPlantCountLabel = _root.Q<Label>("harvested-plants-count-label");
        _finishedDriedCountLabel = _root.Q<Label>("dried-plants-count-label");
        _scoreLabel = _root.Q<Label>("score-label");
        _moneyLabel = _root.Q<Label>("money-label");

    }

    private void UpdateHUD()
    {
        _currentDayLabel.text = $"Tag {GameStateManagerSingleton.Instance.GameState.CurrentDay}";
        _harvestedPlantCountLabel.text = _gameState.HarvestedPlantCount.ToString();
        _finishedDriedCountLabel.text = _gameState.CompletedDriedPlantsCount.ToString();
        _scoreLabel.text = _gameState.CurrentScore.ToString();
        _moneyLabel.text = _gameState.Money.ToString();

        if (!_manager.IsReadyToChangeRoom)
        {
            _changeToMainRoomButton.SetEnabled(false);
            _changeToDryingRoomButton.SetEnabled(false);
        }
        else
        {
            UpdateButtonState(_changeToMainRoomButton, _manager.DryingRoomIsActive);
            UpdateButtonState(_changeToDryingRoomButton, _manager.MainRoomIsActive);
        }
    }
    void UpdateButtonState(Button button, bool canChange)
    {
        button.style.display = canChange ? DisplayStyle.Flex : DisplayStyle.None;
        if (canChange)
            button.SetEnabled(true);
    }


}
