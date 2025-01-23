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
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        SetupButtons();
        SetupLabels();
        UpdateHUD();
    }
    public void Initialize(VisualElement root)
    {
        _root = root;
        GameState.DayChanged += OnDayChanged;
        GameState.InventorzChanged += OnInventorzChanged;
        GameState.ScoreChanged += OnScoreChanged;
        GameState.GetrocknetChanged += OnGetrocknetChanged;
        //GameState.OnInventoryChanged += OnInventoryChanged;  // Abo für Inventaränderungen
    }
    private void SetupButtons()
    {
        var advanceDayButton = _root.Q<Button>("advance-day-button");
        advanceDayButton.clicked += GameStateManagerSingleton.Instance.AdvanceDay;
        advanceDayButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        var saveGameButton = _root.Q<Button>("save-game-button");
        saveGameButton.clicked += GameStateManagerSingleton.Instance.Save;
        saveGameButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        // Button zum Ernten hinzufügen
        var harvestButton = _root.Q<Button>("harvest-button");
        //harvestButton.clicked += HarvestSelectedPlant;

        // for test
        Button _erntenBtn = _root.Q<Button>("Ernten");
        _erntenBtn.clicked += ErntenBtnClicked;
        _erntenBtn.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }

    private void ErntenBtnClicked()
    {
        GameState.ErnteAction?.Invoke();
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

    // Wenn sich das Inventar ändert, z. B. wenn eine Pflanze geerntet wird
    //private void OnInventoryChanged(int plantID, InventoryChangeType changeType)
    //{
    //    if (changeType == InventoryChangeType.Harvested)
    //    {
    //        // Update der UI nach der Ernte
    //        Debug.Log($"Pflanze {plantID} wurde geerntet!");
    //        UpdateHUD();  // Update der HUD-Anzeige nach der Ernte
    //    }
    //}

    // Ernte-Logik ausführen, wenn der Ernte-Button geklickt wird
    //private void HarvestSelectedPlant()
    //{
    //    int selectedPlantID = GetSelectedPlantID();  // Methode zum Abrufen der aktuell ausgewählten Pflanze
    //    if (selectedPlantID >= 0)
    //    {
    //        GameStateManagerSingleton.Instance.GameState.HarvestPlant(selectedPlantID);
    //    }
    //}
}