using System;
using System.Collections;
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
        GameState.InventorzChanged += OnInventorzChanged;
        GameState.ScoreChanged += OnScoreChanged;
        GameState.GetrocknetChanged += OnGetrocknetChanged;
        //GameState.OnInventoryChanged += OnInventoryChanged;  // Abo für Inventaränderungen

        SetupButtons();
        SetupLabels();
        UpdateHUD();
    }
    private void SetupButtons()
    {
        Button advanceDayButton = _root.Q<Button>("update-day-button");
        advanceDayButton.clicked += GameStateManagerSingleton.Instance.AdvanceDay;
        advanceDayButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        var saveGameButton = _root.Q<Button>("save-game-button");
        saveGameButton.clicked += GameStateManagerSingleton.Instance.Save;
        saveGameButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

    }
    private void SetupLabels()
    {
        _currentDayLabel = _root.Q<Label>("current-day-label");
        _ernteLabel = _root.Q<Label>("Ernte");
        _scoreLabel = _root.Q<Label>("Score");
        _getrocknetLabel = _root.Q<Label>("Getrocknet");
    }

    private void UpdateHUD()
    {
        OnDayChanged();
        OnInventorzChanged();
        OnScoreChanged();
        OnGetrocknetChanged();
    }

    private void OnInventorzChanged()
    {
        _ernteLabel.text = $"Ernte : {GameStateManagerSingleton.Instance.GameState.TreesCount}";
    }

    private void OnScoreChanged()
    {
        _scoreLabel.text = $"Score : {GameStateManagerSingleton.Instance.GameState.Score}";
    }
    private void OnGetrocknetChanged()
    {
        _getrocknetLabel.text = $"Getrocknet : {GameStateManagerSingleton.Instance.GameState.Getrocknet}";
    }
    private void OnDayChanged()
    {
        _currentDayLabel.text = $"Tag {GameStateManagerSingleton.Instance.GameState.CurrentDay}";
    }
}