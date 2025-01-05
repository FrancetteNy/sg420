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
    }
    private void SetupButtons()
    {
        var advanceDayButton = _root.Q<Button>("advance-day-button");
        advanceDayButton.clicked += AdvanceDay;
        advanceDayButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }

    private void SetupLabels()
    {
        _currentDayLabel = _root.Q<Label>("current-day-label");

    }


    private void AdvanceDay()
    {
        GameStateManagerSingleton.Instance.GameState.AdvanceDay();
    }

    private void OnDayChanged()
    {
        _currentDayLabel.text = $"Tag {GameStateManagerSingleton.Instance.GameState.CurrentDay}";
    }
}