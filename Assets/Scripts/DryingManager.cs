using System;
using System.Collections.Generic;
using UnityEngine;

public class DryingManager : MonoBehaviour
{
    public GameObject[] PlantPoints;
    List<DryingController> _dryingControllers;
    HighlightController _highlightController;
    GameState _gameState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = GameStateManagerSingleton.Instance.GameState;
        InitializeDryingControllers();
        InitializeHighlight();
        GameState.DayChanged += OnDayChanged;
    }

    private void OnDayChanged()
    {
        foreach (var controller in _dryingControllers)
        { 
            AgePlant(controller);
        }
    }

    private void AgePlant(DryingController controller)
    {
        DriedPlantData driedPlantdata = controller.DriedPlantData;

        if (driedPlantdata.DryingAge.Stage == DryingStage.Empty)
            return;

        driedPlantdata.DryingAge.AgeNumber += 1;

        if (driedPlantdata.DryingAge.AgeNumber > DriedManagerConstants.MaximumAgePerStage[driedPlantdata.DryingAge.Stage])
        {
            driedPlantdata.DryingAge.Stage = driedPlantdata.DryingAge.GetNextStage();
            driedPlantdata.DryingAge.AgeNumber = 0;
            controller.UpdatePlantObject();
        }
    }

    private void InitializeDryingControllers()
    {
        _dryingControllers = new List<DryingController>();
        for (int i = 0; i < PlantPoints.Length; i++)
        {
            GameObject plantPoint = PlantPoints[i];
            DryingController controller = plantPoint.GetComponent<DryingController>();
            _dryingControllers.Add(controller);
            controller.DriedPlantData = _gameState.CurrentlyDryingPlants.List[i];
        }
    }

    private void InitializeHighlight()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        for (int i = 0; i < _dryingControllers.Count; i++)
        {
            ConstructPlantHighlightAndClickFunction(i);
        }
    }

    private void ConstructPlantHighlightAndClickFunction(int index)
    {
        GameObject plantPoint = PlantPoints[index];
        DryingController controller = _dryingControllers[index];
        var highlightBuilder = _highlightController.BeginHighlightObject(plantPoint);
        highlightBuilder.WithClickAction((data) =>
        {
            if (controller.DriedPlantData.DryingAge.Stage != DryingStage.Empty)
            {
                UIEvents.ShowModalView("Warnung", "Die Trocknungsphase wirklich abschließen? Die Trocknung kann nicht rückgängig gemacht werden.", () => CollectPlant(controller));
                return;
            }
            if (_gameState.HarvestedPlantCount == 0)
            {
                UIEvents.AddNotification(new("Keine geernteten Pflanzen verfügbar", "", 2));
                return;
            }
            PlantData harvestedData = _gameState.HarvestedPlantDataList.List[^1];
            _gameState.HarvestedPlantDataList.List.Remove(harvestedData); //TODO Open selection menu
                
            DriedPlantData driedPlantData = _gameState.CurrentlyDryingPlants.List[index].Initialize(harvestedData);
            driedPlantData.DryingAge.Stage = driedPlantData.DryingAge.GetNextStage();
            controller.DriedPlantData = driedPlantData;
            GameState.UpdateHUD?.Invoke();
            });
        highlightBuilder.Apply();
    }

    private void CollectPlant(DryingController controller)
    {
        int scoreToAdd = DriedManagerConstants.ScorePerStage[controller.DriedPlantData.DryingAge.Stage];
        _gameState.CurrentScore += scoreToAdd;


        string plantStrain = $"getrocknetes {controller.DriedPlantData.OldPlantData.Strain} geerntet";
        string scoreString = $"Das hat dir {scoreToAdd} Punkte gegeben. Du hast jetzt {_gameState.CurrentScore} Punkte.";
        UIEvents.AddNotification(new NotificationData(plantStrain, scoreString, 3));

        _gameState.CurrentlyDryingPlants.List.Remove(controller.DriedPlantData);
        _gameState.CompletedDriedPlantDataList.List.Add(controller.DriedPlantData);
        controller.DriedPlantData = new();
        GameState.UpdateHUD?.Invoke();
    }
}


public static class DriedManagerConstants
{
    public static Dictionary<DryingStage, int> MaximumAgePerStage = new Dictionary<DryingStage, int> {
            {DryingStage.Empty, 0 },
            {DryingStage.DryingStart, 1 },
            {DryingStage.DryingMid, 1 },
            {DryingStage.DryingEnd, 1 },
            {DryingStage.DryingFinished, 0 },
        };
    public static Dictionary<DryingStage, int> ScorePerStage = new Dictionary<DryingStage, int> {
            {DryingStage.Empty, 0 },
            {DryingStage.DryingStart, 5 },
            {DryingStage.DryingMid, 10 },
            {DryingStage.DryingEnd, 15 },
            {DryingStage.DryingFinished, 20 },
        };
}