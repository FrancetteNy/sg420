using System.Collections;
using System;
using UnityEngine;
using static AgeDrying;

public class Drying_Controller : MonoBehaviour
{
    public DriedData PlantDriedData;
    public GameObject PlantObject;
    public Action StageChanged;
    Renderer _rendererMaterial;
    Outline _outline;
    void Start()
    {
        PlantObject = transform.Find("Plant").gameObject;
        _outline = GetComponent<Outline>();
        StageChanged += OnStageChanged;
        _rendererMaterial = PlantObject.GetComponent<Renderer>();
    }

    private void OnStageChanged()
    {
        OutlineUpdate();
        ColorMaterialUpdate();
    }
    private void OutlineUpdate()
    {
        if (PlantDriedData.Age.Stage == DryingStage.Ready)
        {
            _outline.OutlineColor = Color.green;
        }
        else
        {
            _outline.OutlineColor = Color.yellow;
        }
    }
    private void ColorMaterialUpdate()
    {
        Color32 newColor = new Color32(160, 160, 160, 255);
        DryingStage stage = PlantDriedData.Age.Stage;

        if (stage == DryingStage.DryingMid)
        {
            newColor = new Color32(154, 134, 50, 255);
        }
        else if(stage == DryingStage.DryingEnd)
        {
            newColor = new Color32(102, 51, 0, 255);
        }
        else if(stage == DryingStage.Ready)
        {
            newColor = new Color32(30, 15, 8, 255);
        }

        _rendererMaterial.material.color = newColor;
        _rendererMaterial.material.SetColor("_EmissionColor", newColor);
    }
}