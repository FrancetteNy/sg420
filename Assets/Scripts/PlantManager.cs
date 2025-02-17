using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Age;
using static DryingProcess.AgeDrying;

public class PlantManager : MonoBehaviour
{
    public ObservableCollection<GameObject> Plants = new();
    private HighlightController _highlightController;
    private int _plantLayer;
    void Start()
    {
        _plantLayer = LayerMask.NameToLayer("Plant");
        _highlightController = FindAnyObjectByType<HighlightController>();
        //Create the initial Plants and set the data from the GameStateManager
        var plantContainer = GameObject.Find("Plants");
        var plantPositions = new List<Transform>();
        foreach (Transform plantPosition in plantContainer.transform)
        {
            plantPositions.Add(plantPosition);
        }
        var plantPrefab = Resources.Load<GameObject>("prefabs/plant_with_pot");
        foreach (var (plantData, position) in GameStateManagerSingleton.Instance.GameState.PlantDataList.List.Zip(plantPositions, (a,b) => (a,b)))
        {
            var inScene = Instantiate(plantPrefab, position);
            var controller = inScene.GetComponent<PlantController>();
            controller.PlantData = plantData;
            Plants.Add(inScene);
            SetLayerOfAllChildren(inScene);
            ManagePlantStageModel(inScene);
            UpdateModel(inScene, plantData.Age);
        }
        //Make sure that everything is up to date if anything changes
        Plants.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) => UpdateHighlightAndDetailView());
        UpdateHighlightAndDetailView();
        UIEvents.ShowDetailView += (_) => _highlightController.enabled = false;

        GameState.DayChanged += OnDayChanged;
    }

    private void OnDayChanged()
    {
        foreach(var plant in Plants)
        {
            PlantController plantController = plant.GetComponent<PlantController>();
            UpdatePlant(plant, plantController.PlantData);
        }
    }

    private void UpdatePlant(GameObject plant, PlantData plantController)
    {
        var correctAmountOfWater = false;
        var correctAmountOfNutrients = false;

        if (plantController.Soil.StoredWater < PlantManagerConstants.MinWater)
        {
            Debug.Log($"Not enough water {plant}");
            GameState.EncyclopediaEntryUnlocked("BewÃ¤ssern");
        }
        else if (plantController.Soil.StoredWater > PlantManagerConstants.MaxWater)
        {
            Debug.Log($"Too much water {plant}");
            GameState.EncyclopediaEntryUnlocked("BewÃ¤ssern");
        }
        else
        {
            if (plantController.Age.Stage == Age.GrowthStage.FLOWERING && plantController.Age.AgeNumber >= PlantManagerConstants.MaxFloweringAge)
            {
                Debug.Log($"Plant too old to grow {plant}");
            }
            else
            {
                correctAmountOfWater = true;
            }
        }
        if (plantController.Soil.StoredNutrients < PlantManagerConstants.MinNutrients)
        {
            Debug.Log($"Not enough Nutrients {plant}");
            GameState.EncyclopediaEntryUnlocked("NÃ¤hrstoffe");
        }
        else if (plantController.Soil.StoredNutrients > PlantManagerConstants.MaxNutrients)
        {
            Debug.Log($"Too much Nutrients {plant}");
            GameState.EncyclopediaEntryUnlocked("NÃ¤hrstoffe");
        }
        else
        {
            if (plantController.Age.Stage == Age.GrowthStage.FLOWERING && plantController.Age.AgeNumber >= PlantManagerConstants.MaxFloweringAge)
            {
                Debug.Log($"Plant too old to grow {plant}");
            }
            else
            {
                correctAmountOfNutrients = true;
            }
        }
        plantController.Soil.StoredWater = Math.Max(plantController.Soil.StoredWater - PlantManagerConstants.MinWater, 0);
        plantController.Soil.StoredNutrients = Math.Max(plantController.Soil.StoredNutrients - PlantManagerConstants.MinNutrients, 0);
        if (correctAmountOfNutrients && correctAmountOfWater)
        {
            AgePlant(plant);
            var plantModel = GetCurrentPlantModel(plant).transform;
            plantModel.localScale += new Vector3(0.1f,0.1f,0.1f);
        }
        UpdateModel(plant, plantController.Age);
    }

    private void UpdateModel(GameObject plant, Age age)
    {
        float vectorValue;
        switch (age.Stage)
        {
            case GrowthStage.GERMINATION:
                vectorValue = 0.1f * age.AgeNumber;
                break;
            case GrowthStage.SEEDLING:
                vectorValue = CalculateVectorValue(
                    PlantManagerConstants.MaxGerminationValue,
                    PlantManagerConstants.MaxSeedlingValue,
                    PlantManagerConstants.MaximumAgePerGrowthStage[GrowthStage.SEEDLING],
                    age.AgeNumber);
                break;
            case GrowthStage.VEGETATIVEGROWTH:
                vectorValue = CalculateVectorValue(
                    PlantManagerConstants.MaxSeedlingValue,
                    PlantManagerConstants.MaxVegetativeGrowthValue,
                    PlantManagerConstants.MaximumAgePerGrowthStage[GrowthStage.VEGETATIVEGROWTH],
                    age.AgeNumber);
                break;
            case GrowthStage.FLOWERING:
                vectorValue = CalculateVectorValue(
                    PlantManagerConstants.MaxVegetativeGrowthValue,
                    PlantManagerConstants.MaxFloweringValue,
                    PlantManagerConstants.MaximumAgePerGrowthStage[GrowthStage.FLOWERING],
                    age.AgeNumber);
                break;
            default:
                vectorValue = 0.0f;
                break;
            case GrowthStage.FADED:
                vectorValue = 1.0f;
                break;

        }
        ManagePlantStageModel(plant);
        GetCurrentPlantModel(plant).transform.localScale = new Vector3(vectorValue, vectorValue, vectorValue);
    }

    private float CalculateVectorValue(float min, float max, int numberOfSteps, int currentStep)
    {
        return min + (max - min) / numberOfSteps * currentStep;
    }

    public void AgePlant(GameObject plant)
    {
        PlantController plantController = plant.GetComponent<PlantController>();
        PlantData plantData = plantController.PlantData;

        plantData.Age.AgeNumber += 1;

        if (plantData.Age.AgeNumber > PlantManagerConstants.MaximumAgePerGrowthStage[plantData.Age.Stage])
        {
            plantData.Age.Stage = plantData.Age.GetNextStage();
            ManagePlantStageModel(plant);
            plantData.Age.AgeNumber = 0;
            plantController.StageChanged.Invoke();
        }
    }

    private void ManagePlantStageModel(GameObject plant)
    {
        plant.transform.Find("Plant1").gameObject.SetActive(false);
        plant.transform.Find("Plant2").gameObject.SetActive(false);
        plant.transform.Find("Plant3").gameObject.SetActive(false);
        plant.transform.Find("plant").gameObject.SetActive(false);

        // Activate the current model
        GetCurrentPlantModel(plant).SetActive(true);
    }

    public GameObject GetCurrentPlantModel(GameObject plant)
    {
        switch (plant.GetComponent<PlantController>().PlantData.Age.Stage)
        {
            case GrowthStage.GERMINATION:
                return plant.transform.Find("Plant1").gameObject;
            case GrowthStage.SEEDLING:
                return plant.transform.Find("Plant2").gameObject;
            case GrowthStage.VEGETATIVEGROWTH:
                return plant.transform.Find("Plant3").gameObject;
            case GrowthStage.FLOWERING:
            case GrowthStage.FADED:
                return plant.transform.Find("plant").gameObject;
            default:
                return plant.transform.Find("Plant1").gameObject;
        }
    }

    private void SetLayerOfAllChildren(GameObject gameObject)
    {
        gameObject.layer = _plantLayer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayerOfAllChildren(child.gameObject);
        }
    }

    private void UpdateHighlightAndDetailView()
    {
        //DetailViewController.PlantsChanged.Invoke();
        for (int i = 0; i < Plants.Count; i++)
        {
            ConstructPlantHighlightAndClickFunction(i);
        }
    }

    public void ErntePlant(PlantController plant)
    {
        PlantData plantData = plant.PlantData;

        if (plantData.Age.Stage == GrowthStage.EMPTY)
        {
            ModalController.Instance.ShowModal("Warnung", "Die Pflanze ist noch nicht gewachsen und kann nicht geerntet werden.", () => { });
            return;
        }

        if (plantData.Age.Stage == GrowthStage.FLOWERING)
        {
            ErnteAction(plant);
        }
        else if (plantData.Age.Stage == GrowthStage.FADED)
        {
            ModalController.Instance.ShowModal("Warnung", "Die Pflanze ist verwelkt und past prime. Die Ernte kann nicht rückgängig gemacht werden!", () => ErnteAction(plant));
        }
        else
        {
            ModalController.Instance.ShowModal("Warnung", "Die Pflanze ist nicht in einem optimalen Zustand für die Ernte! Die Ernte kann nicht rückgängig gemacht werden.", () => ErnteAction(plant));
        }
    }
    private void ErnteAction(PlantController plant)
    {
        GetCurrentPlantModel(plant.gameObject).SetActive(false);

        plant.StageChanged?.Invoke();

        GameStateManagerSingleton.Instance.UpdateScore(PlantManagerConstants.ScorePerGrowthStage[plant.PlantData.Age.Stage]);

        if (plant.PlantData.Age.Stage != GrowthStage.FADED)
        {
            GameStateManagerSingleton.Instance.UpdateTreesCount(1);

            string plantStrain = $"{plant.PlantData.Strain} geerntet";
            int totalScore = GameStateManagerSingleton.Instance.GameState.TreesCount;
            NotificationManagerSingleton.Instance.AddNotification(new NotificationData(plantStrain, $"Total Score: {totalScore}", 6));
        }
        else
        {
            NotificationManagerSingleton.Instance.AddNotification(new NotificationData("Verwelkte Pflanze", "Diese Pflanze ist verwelkt und kann nicht mehr geerntet werden.", 6));
        }

        plant.PlantData.Age.ResetAge();
    }

    private void ConstructPlantHighlightAndClickFunction(int index)
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(Plants[index]);
        PlantController plant = Plants[index].GetComponent<PlantController>();

        highlightBuilder.WithClickAction((data) =>
        {
            data.Outline.enabled = false;
            UIEvents.ShowDetailView.Invoke(index);
        });
        highlightBuilder.Apply();

    }
}

public static class PlantManagerConstants
{
    public const float MinWater = 10f;
    public const float MaxWater = 30f;
    public const float MinNutrients = 5f;
    public const float MaxNutrients = 20f;

    public const int MaxFloweringAge = 5;

    public const float MaxGerminationValue = 0.1f;
    public const float MaxSeedlingValue = 0.2f;
    public const float MaxVegetativeGrowthValue = 0.6f;
    public const float MaxFloweringValue = 1.0f;
    public static Dictionary<GrowthStage, int> MaximumAgePerGrowthStage = new Dictionary<GrowthStage, int> {
            {GrowthStage.EMPTY, 0 },
            {GrowthStage.GERMINATION, 1 },
            {GrowthStage.SEEDLING, 3 },
            {GrowthStage.VEGETATIVEGROWTH, 8 },
            {GrowthStage.FLOWERING, 5 },
            {GrowthStage.FADED, 0 },
        };
    public static Dictionary<GrowthStage, int> ScorePerGrowthStage = new Dictionary<GrowthStage, int> {
            {GrowthStage.EMPTY, 0 },
            {GrowthStage.GERMINATION, 2 },
            {GrowthStage.SEEDLING, 4 },
            {GrowthStage.VEGETATIVEGROWTH, 6 },
            {GrowthStage.FLOWERING, 10 },
            {GrowthStage.FADED, 3 },
        };
}