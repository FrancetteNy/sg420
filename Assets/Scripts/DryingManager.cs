using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using static Age;
using static AgeDrying;

public class DryingManager : MonoBehaviour
{
    public ObservableCollection<GameObject> Plants = new();
    private HighlightController _highlightController;
    void Start()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        foreach (Transform plantPosition in transform)
        {
            Plants.Add(plantPosition.gameObject);
        }
        foreach (var (plantData, plantObject) in GameStateManagerSingleton.Instance.GameState.PlantDriedDataList.List.Zip(Plants, (data, obj) => (data, obj)))
        {
            Drying_Controller dryingController = plantObject.GetComponent<Drying_Controller>();
            dryingController.PlantDriedData = plantData;
            UpdatePlantModel(dryingController);
        }
       // GameState.DayChanged += OnDayChanged;
        UpdateHighlight();
    }

    private void OnDayChanged()
    {
        foreach (var plant in Plants)
        {
            AgePlant(plant);
        }
    }
    private void SaveDataOfPlants()
    {
        var plantDataList = GameStateManagerSingleton.Instance.GameState.PlantDriedDataList.List;

        for (int i = 0; i < plantDataList.Count; i++)
        {
            plantDataList[i] = Plants[i].GetComponent<Drying_Controller>().PlantDriedData;
        }

        GameStateManagerSingleton.Instance.Save();
    }
    private void UpdatePlantModel(Drying_Controller plant)
    {
        if(plant.PlantDriedData.Age.Stage == DryingStage.Drying)
            plant.PlantObject.SetActive(true);
    }
    private void AgePlant(GameObject plant)
    {
        PlantDriedData plantDriedData = plant.GetComponent<Drying_Controller>().PlantDriedData;

        if(plantDriedData.Age.Stage != DryingStage.Drying)
            return;

        plantDriedData.Age.AgeNumber += 1;

        if (plantDriedData.Age.AgeNumber > DriedManagerConstants.MaximumAgePerDryingStage[plantDriedData.Age.Stage])
        {
            plantDriedData.Age.Stage = plantDriedData.Age.GetNextStage();
            plantDriedData.Age.AgeNumber = 0;
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < Plants.Count; i++)
        {
            ConstructPlantHighlightAndClickFunction(i);
        }
    }

    private void CollectPlant(Drying_Controller plant)
    {
        //NotificationManagerSingleton.Instance.AddNotification(new NotificationData(plantStrain, $"Total Score : {totalScore}", 500, () => print("Clicked")));

        plant.PlantObject.SetActive(false);
        GameStateManagerSingleton.Instance.AdvanceGetrocknet(1);
        // Change Value for every Strain
        GameStateManagerSingleton.Instance.AdvanceScore(DriedManagerConstants.ScorePerDryingStage[plant.PlantDriedData.Age.Stage]);
        plant.PlantDriedData.Age.ResetAge();
        SaveDataOfPlants();

        string plantStrain = plant.PlantDriedData.Strain.ToString();
        int totalScore = GameStateManagerSingleton.Instance.GameState.Getrocknet;
        NotificationManagerSingleton.Instance.AddNotification(new NotificationData(plantStrain, $"Total Score : {totalScore}", 6));
    }

    private void ConstructPlantHighlightAndClickFunction(int index)
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(Plants[index]);
        Drying_Controller plant = Plants[index].GetComponent<Drying_Controller>();

        highlightBuilder.WithClickAction((data) =>
        {
            if (plant.PlantDriedData.Age.Stage == DryingStage.Ready)
            {
                CollectPlant(plant);
            }
            else if (plant.PlantDriedData.Age.Stage == DryingStage.Drying)
            {
                ModalController.Instance.ShowModal("Warnung", "Die Trocknungsphase wirklich abschließen? Die Trocknung kann nicht rückgängig gemacht werden.", () => CollectPlant(plant));
            }
            else if (GameStateManagerSingleton.Instance.GameState.TreesCount > 0 && plant.PlantDriedData.Age.Stage == DryingStage.Others)
            {
                plant.PlantDriedData.Age.Stage = plant.PlantDriedData.Age.GetNextStage();
                plant.PlantObject.SetActive(true);
                GameStateManagerSingleton.Instance.AdvanceTreesCount(-1);
                SaveDataOfPlants();
            }
            else
            {
                NotificationManagerSingleton.Instance.AddNotification(new NotificationData("Warning", "You Donot have any plant", 6));
            }
        });
        highlightBuilder.Apply();
    }
}

public static class DriedManagerConstants
{
    public static Dictionary<DryingStage, int> MaximumAgePerDryingStage = new Dictionary<DryingStage, int> {
            {DryingStage.Others, 0 },
            {DryingStage.Drying, 2 },
            {DryingStage.Ready, 0 },
        };
    public static Dictionary<DryingStage, int> ScorePerDryingStage = new Dictionary<DryingStage, int> {
            {DryingStage.Others, 0 },
            {DryingStage.Drying, 5 },
            {DryingStage.Ready, 10 },
        };
}