using System.Collections.Generic;
using UnityEngine;
using static Age;

public class DryingController : MonoBehaviour
{
    public DriedPlantData DriedPlantData
    {
        get
        {
            return this._driedPlantData;
        }
        set
        {
            this._driedPlantData = value;
            UpdatePlantObjectVisibility(value);
        }
    }
    Dictionary<GrowthStage, GameObject> _plantObjectsForGrowthStage;
    Dictionary<GrowthStage, Renderer> _rendererForGrowthStage;
    [SerializeField] DriedPlantData _driedPlantData;

    private void UpdatePlantObjectVisibility(DriedPlantData plantData)
    {
        bool plantDataIsValid = plantData != null && plantData.DryingAge != null;
        GrowthStage plantStage = plantData.OldPlantData.Age.Stage;
        _plantObjectsForGrowthStage[GrowthStage.GERMINATION].gameObject.SetActive(plantDataIsValid && plantStage == GrowthStage.GERMINATION);
        _plantObjectsForGrowthStage[GrowthStage.SEEDLING].gameObject.SetActive(plantDataIsValid && plantStage == GrowthStage.SEEDLING);
        _plantObjectsForGrowthStage[GrowthStage.VEGETATIVEGROWTH].gameObject.SetActive(plantDataIsValid && plantStage == GrowthStage.VEGETATIVEGROWTH);
        _plantObjectsForGrowthStage[GrowthStage.FLOWERING].gameObject.SetActive(plantDataIsValid && (plantStage == GrowthStage.FLOWERING || plantStage == GrowthStage.FADED));
        if (plantDataIsValid)
        {
            UpdatePlantObject();
        }
    }
    private void Awake()
    {
        _plantObjectsForGrowthStage = new() {
        {GrowthStage.EMPTY, null},
        {GrowthStage.GERMINATION, transform.Find("Plant1").gameObject},
        {GrowthStage.SEEDLING, transform.Find("Plant2").gameObject},
        {GrowthStage.VEGETATIVEGROWTH, transform.Find("Plant3").gameObject},
        {GrowthStage.FLOWERING, transform.Find("Plant").gameObject},
        {GrowthStage.FADED, transform.Find("Plant").gameObject},
        };
        _rendererForGrowthStage = new()
        {
        {GrowthStage.EMPTY, null},
        {GrowthStage.SEEDLING, _plantObjectsForGrowthStage[GrowthStage.SEEDLING].GetComponentInChildren<Renderer>()},
        {GrowthStage.GERMINATION, _plantObjectsForGrowthStage[GrowthStage.GERMINATION].GetComponentInChildren<Renderer>()},
        {GrowthStage.VEGETATIVEGROWTH, _plantObjectsForGrowthStage[GrowthStage.VEGETATIVEGROWTH].GetComponentInChildren < Renderer >()},
        {GrowthStage.FLOWERING, _plantObjectsForGrowthStage[GrowthStage.FLOWERING].GetComponent < Renderer >()},
        {GrowthStage.FADED, _plantObjectsForGrowthStage[GrowthStage.FADED].GetComponent < Renderer >()},
        };
    }

    Renderer GetCurrentRenderer()
    {
        return _rendererForGrowthStage[_driedPlantData.OldPlantData.Age.Stage];
    }

    internal void UpdatePlantObject()
    {
        var renderer = GetCurrentRenderer();
        if (renderer == null)
        {
            return;
        }

        Color32 newColor = new Color32(255, 255, 255, 255);

        DryingStage stage = DriedPlantData.DryingAge.Stage;

        if (stage == DryingStage.DryingMid)
        {
            newColor = new Color32((byte)(0.8f * 255), (byte)(0.6f * 255), (byte)(0.3f * 255), 255); // #CC9933
        }
        else if (stage == DryingStage.DryingEnd)
        {
            newColor = new Color32((byte)(0.55f * 255), (byte)(0.25f * 255), 0, 255); // #8C4000
        }
        else if (stage == DryingStage.DryingFinished)
        {
            newColor = new Color32((byte)(0.4f * 255), (byte)(0.2f * 255), (byte)(0.1f * 255), 255); // #662600
        }
        foreach (var material in renderer.materials)
        {
            material.color = newColor;
            material.SetColor("_EmissionColor", newColor);
        }
    }
}

