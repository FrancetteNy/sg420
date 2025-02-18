using System.Collections;
using System;
using UnityEngine;
using static AgeDrying;

public class Drying_Controller : MonoBehaviour
{
    public DriedData DriedPlantData;
    public GameObject PlantObject;
    public Action StageChanged;
    Renderer _rendererMaterial;
    void Start()
    {
        PlantObject = transform.Find("Plant").gameObject;
        StageChanged += OnStageChanged;
        _rendererMaterial = PlantObject.GetComponent<Renderer>();
    }

    private void OnStageChanged()
    {       
        ColorMaterialUpdate();
    } 
    private void ColorMaterialUpdate()
    {
        Color32 newColor = new Color32(160, 160, 160, 255); // Grauton

        DryingStage stage = DriedPlantData.Age.Stage;

        if (stage == DryingStage.DryingMid)
        {
            newColor = new Color32((byte)(0.8f * 255), (byte)(0.6f * 255), (byte)(0.3f * 255), 255); // #CC9933
        }
        else if (stage == DryingStage.DryingEnd)
        {
            newColor = new Color32((byte)(0.55f * 255), (byte)(0.25f * 255), 0, 255); // #8C4000
        }
        else if (stage == DryingStage.Ready)
        {
            newColor = new Color32((byte)(0.4f * 255), (byte)(0.2f * 255), (byte)(0.1f * 255), 255); // #662600
        }

        _rendererMaterial.material.color = newColor;
        _rendererMaterial.material.SetColor("_EmissionColor", newColor);
    }

}