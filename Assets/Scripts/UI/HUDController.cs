using System;
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

        var saveGameButton = _root.Q<Button>("save-game-button");
        saveGameButton.clicked += GameStateManagerSingleton.Instance.Save;
        saveGameButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        
        var inventarButton = _root.Q<Button>("inventar-button");
        inventarButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        inventarButton.clicked += () => UIEvents.ShowInventar.Invoke();
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