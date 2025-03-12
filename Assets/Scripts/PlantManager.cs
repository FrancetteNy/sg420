using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using static Age;

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
        foreach (var (plantData, position) in GameStateManagerSingleton.Instance.GameState.PlantDataList.List.Zip(plantPositions, (a, b) => (a, b)))
        {
            var inScene = Instantiate<GameObject>(plantPrefab, position);
            var controller = inScene.GetComponent<PlantController>();
            controller.PlantData = plantData;
            Plants.Add(inScene);
            SetLayerOfAllChildren(inScene);
            ManagePlantStageModel(inScene);
            UpdateModel(GetCurrentPlantModel(inScene).transform, plantData.Age);
        }
        
        //Make sure that everything is up to date if anything changes
        Plants.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) => UpdateHighlightAndDetailView());
        UpdateHighlightAndDetailView();


        GameState.DayChanged += OnDayChanged;
    }


    private void OnDestroy()
    {
        GameState.DayChanged -= OnDayChanged;
    }
    private void OnDayChanged()
    {
        foreach (var plant in Plants)
        {
            PlantController plantController = plant.GetComponent<PlantController>();
            UpdatePlant(plant, plantController.PlantData);
        }
    }

    private void UpdatePlant(GameObject plant, PlantData plantData)
    {
        var correctAmountOfWater = false;
        var correctAmountOfNutrients = false;
        if (plantData.Soil.StoredWater < PlantManagerConstants.MinWater)
        {
            plantData.Quality -= PlantManagerConstants.QualityPunishmentUnderWatering;
            Debug.Log($"Not enough water {plant}. Current {plantData.Soil.StoredWater}");
        }
        else if (plantData.Soil.StoredWater > PlantManagerConstants.MaxWater)
        {
            plantData.Quality -= PlantManagerConstants.QualityPunishmentOverWatering;
            Debug.Log($"Too much water {plant}. Current: {plantData.Soil.StoredWater}");
        }
        else
        {
            Debug.Log($"Enougth water {plant}. Current: {plantData.Soil.StoredWater}");
            if (plantData.Age.Stage == Age.GrowthStage.FLOWERING && plantData.Age.AgeNumber >= PlantManagerConstants.MaxFloweringAge)
            {
                Debug.Log($"Plant too old to grow {plant}");
            }
            else
            {
                correctAmountOfWater = true;
            }
        }
        if (plantData.Soil.StoredNutrients < PlantManagerConstants.MinNutrients)
        {
            plantData.Quality -= PlantManagerConstants.QualityPunishmentNotEnoughNutrients;
            Debug.Log($"Not enough Nutrients {plant}");
        }
        else if (plantData.Soil.StoredNutrients > PlantManagerConstants.MaxNutrients)
        {
            plantData.Quality -= PlantManagerConstants.QualityPunishmentTooMuchNutrients;
            Debug.Log($"Too much Nutrients {plant}");
        }
        else
        {
            if (plantData.Age.Stage == Age.GrowthStage.FLOWERING && plantData.Age.AgeNumber >= PlantManagerConstants.MaxFloweringAge)
            {
                Debug.Log($"Plant too old to grow {plant}");
            }
            else
            {
                correctAmountOfNutrients = true;
            }
        }
        plantData.Quality = Math.Max(plantData.Quality, 0);
        plantData.Soil.StoredWater = Math.Max(plantData.Soil.StoredWater - PlantManagerConstants.MinWater, 0);
        plantData.Soil.StoredNutrients = Math.Max(plantData.Soil.StoredNutrients - PlantManagerConstants.MinNutrients, 0);
        if (correctAmountOfNutrients && correctAmountOfWater)
        {
            AgePlant(plant);
            var plantModel = GetCurrentPlantModel(plant).transform;
            plantModel.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }
        else
        {
            PlantGenerator plantGenerator = plant.GetComponentInChildren<PlantGenerator>();
            plantGenerator.RemodelCannabisPlant(correctAmountOfNutrients, correctAmountOfWater);
        }
        UpdateModel(GetCurrentPlantModel(plant).transform, plantData.Age);
    }

    private void UpdateModel(Transform transform, Age age)
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
            case GrowthStage.FADED:
                vectorValue = 1.0f;
                break;
            default:
                vectorValue = 0.0f;
                break;
        }
        transform.localScale = new Vector3(vectorValue, vectorValue, vectorValue);
    }

    private float CalculateVectorValue(float min, float max, int numberOfSteps, int currentStep)
    {
        return min + (max - min) / numberOfSteps * currentStep;
    }

    public void AgePlant(GameObject plant)
    {
        PlantData plantController = plant.GetComponent<PlantController>().PlantData;
        if (plantController.Age.Stage == GrowthStage.EMPTY)
        {
            ManagePlantStageModel(plant);
            return;
        }
        plantController.Age.AgeNumber += 1;

        if (plantController.Age.AgeNumber > PlantManagerConstants.MaximumAgePerGrowthStage[plantController.Age.Stage])
        {
            plantController.Age.Stage = plantController.Age.GetNextStage();
            plantController.Age.AgeNumber = 0;
        }
        PlantGenerator plantGenerator = plant.GetComponentInChildren<PlantGenerator>();
        Debug.Log("Hallo :)");
        plantGenerator.GenerateCannabisPlant();
        ManagePlantStageModel(plant);
    }

    public void ManagePlantStageModel(GameObject plant)
    {
        var currentStage = plant.GetComponent<PlantController>().PlantData.Age.Stage;
        plant.transform.Find("Plant1").gameObject.SetActive(currentStage == GrowthStage.GERMINATION);
        plant.transform.Find("Plant2").gameObject.SetActive(currentStage == GrowthStage.SEEDLING);
        plant.transform.Find("Plant3").gameObject.SetActive(currentStage == GrowthStage.VEGETATIVEGROWTH);
        plant.transform.Find("plant").gameObject.SetActive(currentStage == GrowthStage.FLOWERING || currentStage == GrowthStage.FADED);
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
        if (gameObject.name != "pot") {
            gameObject.layer = _plantLayer;
        }
        
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

    private void ConstructPlantHighlightAndClickFunction(int index)
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(Plants[index]);
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
            {GrowthStage.GERMINATION, 1 },
            {GrowthStage.SEEDLING, 3 },
            {GrowthStage.VEGETATIVEGROWTH, 8 },
            {GrowthStage.FLOWERING, 5 },
        };
    public const int QualityPunishmentUnderWatering = 3;
    public const int QualityPunishmentOverWatering = 3;
    public const int QualityPunishmentTooMuchNutrients = 1;
    public const int QualityPunishmentNotEnoughNutrients = 1;

}