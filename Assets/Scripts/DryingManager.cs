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
            //if (controller.DriedPlantData.DryingAge.Stage == DryingStage.Empty)
            //{
                
                PlantData harvestedData = _gameState.HarvestedPlantDataList.List[^1];
                _gameState.HarvestedPlantDataList.List.Remove(harvestedData); //TODO Open selection menu
                
                DriedPlantData driedPlantData = _gameState.CurrentlyDryingPlants.List[index].Initialize(harvestedData);
                driedPlantData.DryingAge.Stage = driedPlantData.DryingAge.GetNextStage();
                controller.DriedPlantData = driedPlantData;
                //return;
            //}
            });
        highlightBuilder.Apply();
    }

    private void CollectPlant(DryingController controller)
    {

        // Update Score für die Pflanze
        //GameStateManagerSingleton.Instance.UpdateScore(DriedManagerConstants.ScorePerDryingStage[dryingController.DriedPlantData.Age.Stage]);


        string plantStrain = $"getrocknetes {controller.DriedPlantData.OldPlantData.Strain} geerntet";
        //int totalScore = GameStateManagerSingleton.Instance.GameState.Getrocknet;
        UIEvents.AddNotification(new NotificationData(plantStrain, "", 6));

        _gameState.CurrentlyDryingPlants.List.Remove(controller.DriedPlantData);
        _gameState.CompletedDriedPlantDataList.List.Add(controller.DriedPlantData);
        controller.DriedPlantData = new();
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