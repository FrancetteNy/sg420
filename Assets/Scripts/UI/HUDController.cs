using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private VisualElement _root;

    private Label _currentDayLabel;
    public void Initialize(VisualElement root)
    {
        _root = root;
        GameState.DayChanged += OnDayChanged;
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

    }

    private void UpdateHUD()
    {
        OnDayChanged();
    }


    private void OnDayChanged()
    {
        _currentDayLabel.text = $"Tag {GameStateManagerSingleton.Instance.GameState.CurrentDay}";
    }
}