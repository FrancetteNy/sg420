using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Unity.VisualScripting;
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
        //Collect the initial Plants
        foreach (var plantController in gameObject.GetComponentsInChildren<PlantController>())
        {
            GameObject go = plantController.gameObject;
            Plants.Add(go);
            SetLayerOfAllChildren(go);
            UpdateModel(go.transform.Find("plant"), plantController.Age);
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
            UpdatePlant(plant, plantController);
        }
    }

    private void UpdatePlant(GameObject plant, PlantController plantController)
    {
        var correctAmountOfWater = false;
        var correctAmountOfNutrients = false;
        if (plantController.Soil.StoredWater < PlantManagerConstants.MinWater)
        {
            Debug.Log($"Not enough water {plant}");
        }
        else if (plantController.Soil.StoredWater > PlantManagerConstants.MaxWater)
        {
            Debug.Log($"Too much water {plant}");
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
        }
        else if (plantController.Soil.StoredNutrients > PlantManagerConstants.MaxNutrients)
        {
            Debug.Log($"Too much Nutrients {plant}");
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
            AgePlant(plantController);
            var plantModel = plant.transform.Find("plant");
            plantModel.localScale += new Vector3(0.1f,0.1f,0.1f);
        }
        UpdateModel(plant.transform.Find("plant"), plantController.Age);
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

    public void AgePlant(PlantController plantController)
    {

        plantController.Age.AgeNumber += 1;

        if (plantController.Age.AgeNumber > PlantManagerConstants.MaximumAgePerGrowthStage[plantController.Age.Stage])
        {
            plantController.Age.Stage = plantController.Age.GetNextStage();
            plantController.Age.AgeNumber = 0;
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
}