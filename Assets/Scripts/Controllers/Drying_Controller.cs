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
        Color32 newColor = new Color32(160, 160, 160, 255); // Standardfarbe (grau)

        DryingStage stage = PlantDriedData.Age.Stage;

        if (stage == DryingStage.DryingMid)
        {
            // Trocknungsphase Mitte: Gelb-Braun
            newColor = new Color32(154, 134, 47, 255); // #9A862F
        }
        else if (stage == DryingStage.DryingEnd)
        {
            // Späte Trocknungsphase: Dunkles Braun
            newColor = new Color32(106, 51, 0, 255); // #6A3300
        }
        else if (stage == DryingStage.Ready)
        {
            // Fertig getrocknet: Dunkelbraun, fast rötlich
            newColor = new Color32(76, 35, 8, 255); // #4C2308
        }

        _rendererMaterial.material.color = newColor;
        _rendererMaterial.material.SetColor("_EmissionColor", newColor);
    }


}