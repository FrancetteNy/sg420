using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using static DryingProcess.AgeDrying;
using static DryingProcess;


public class DryingManager : MonoBehaviour
{
    public List<Drying_Controller> Plants = new List<Drying_Controller>();
    private HighlightController _highlightController;
    void Start()
    {
       

        foreach (var (plantData, plantObject) in GameStateManagerSingleton.Instance.GameState.PlantDriedDataList.List.Zip(Plants, (data, obj) => (data, obj)))
        {
            Drying_Controller dryingController = plantObject.GetComponent<Drying_Controller>();  
            dryingController.DriedPlantData = plantData;
            UpdatePlantModel(dryingController);
        }
        GameState.DayChanged += OnDayChanged;
    }
    private void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        foreach (Transform plantPosition in transform)
        {
            Plants.Add(plantPosition.GetComponent<Drying_Controller>());
        }
        InitializeHighlight();
    }
    private void OnDayChanged()
    {
        Plants.ForEach(AgePlant);
    }
    private void SaveDataOfPlant(Drying_Controller plant)
    {
        var plantDataList = GameStateManagerSingleton.Instance.GameState.PlantDriedDataList.List;
        int plantIndex = Plants.IndexOf(plant);

        if (plantIndex >= plantDataList.Count)
            plantDataList.Add(plant.DriedPlantData);
        else
            plantDataList[plantIndex] = plant.DriedPlantData; 
    }
    private void UpdatePlantModel(Drying_Controller plant)
    {
        if(plant.DriedPlantData.Age.Stage != AgeDrying.DryingStage.Empty)
        {
            plant.PlantObject.SetActive(true);
            plant.StageChanged?.Invoke();
        }
    }
    private void AgePlant(Drying_Controller plant)
    {
        DriedData plantDriedData = plant.DriedPlantData;

        if (plant.DriedPlantData.Age.Stage == AgeDrying.DryingStage.Empty)
            return;

        plantDriedData.Age.AgeNumber += 1;

        if (plantDriedData.Age.AgeNumber > DriedManagerConstants.MaximumAgePerDryingStage[plantDriedData.Age.Stage])
        {
            plantDriedData.Age.Stage = plantDriedData.Age.GetNextStage();
            plantDriedData.Age.AgeNumber = 0;
            plant.StageChanged?.Invoke();
        }
    }

    private void InitializeHighlight()
    {
        for (int i = 0; i < Plants.Count; i++)
        {
            ConstructPlantHighlightAndClickFunction(i);
        }
    }

    private void CollectPlant(Drying_Controller plant)
    {
        var dryingController = plant;  

        dryingController.PlantObject.SetActive(false);
        GameStateManagerSingleton.Instance.UpdateGetrocknet(1);

        // Update Score für die Pflanze
        GameStateManagerSingleton.Instance.UpdateScore(DriedManagerConstants.ScorePerDryingStage[dryingController.DriedPlantData.Age.Stage]);

        dryingController.DriedPlantData.Age.ResetPlantData();
        dryingController.StageChanged?.Invoke();
        SaveDataOfPlant(dryingController); 

        string plantStrain = $"{dryingController.DriedPlantData.Strain} geerntet";
        int totalScore = GameStateManagerSingleton.Instance.GameState.Getrocknet;
        NotificationManagerSingleton.Instance.AddNotification(new NotificationData(plantStrain, $"Total Score : {totalScore}", 6));
    }

    private void ConstructPlantHighlightAndClickFunction(int index)
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(Plants[index].gameObject);
        Drying_Controller plant = Plants[index];

        highlightBuilder.WithClickAction((data) =>
        {
            if (plant.DriedPlantData.Age.Stage != AgeDrying.DryingStage.Empty)
            {
                ModalController.Instance.ShowModal("Warnung", "Die Trocknungsphase wirklich abschließen? Die Trocknung kann nicht rückgängig gemacht werden.", () => CollectPlant(plant));               
            }
            else if (GameStateManagerSingleton.Instance.GameState.TreesCount > 0)
            {
                plant.DriedPlantData.Age.Stage = plant.DriedPlantData.Age.GetNextStage();
                plant.PlantObject.SetActive(true);
                GameStateManagerSingleton.Instance.UpdateTreesCount(-1);
                SaveDataOfPlant(plant);
            }
            else
            {
                NotificationManagerSingleton.Instance.AddNotification(new NotificationData(
                   "Warnung",
                   "Keine Pflanze zum Trocknen verfügbar. Bitte ernten Sie zuerst!", 
                   6
               ));
            }
        });
        highlightBuilder.Apply();
    }
}

public static class DriedManagerConstants
{
    public static Dictionary<AgeDrying.DryingStage, int> MaximumAgePerDryingStage = new Dictionary<AgeDrying.DryingStage, int> {
            {AgeDrying.DryingStage.Empty, 0 },
            {AgeDrying.DryingStage.DryingStart, 2 },
            {AgeDrying.DryingStage.DryingMid, 3 },
            {AgeDrying.DryingStage.DryingEnd, 4 },
            {AgeDrying.DryingStage.Ready, 0 },
        };
    public static Dictionary<AgeDrying.DryingStage, int> ScorePerDryingStage = new Dictionary<AgeDrying.DryingStage, int> {
            {AgeDrying.DryingStage.Empty, 0 },
            {AgeDrying.DryingStage.DryingStart, 5 },
            {AgeDrying.DryingStage.DryingMid, 10 },
            {AgeDrying.DryingStage.DryingEnd, 15 },
            {AgeDrying.DryingStage.Ready, 20 },
        };
}