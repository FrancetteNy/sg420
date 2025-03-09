using System;
using UnityEngine;

public class DryingController : MonoBehaviour
{
    public DriedPlantData DriedPlantData { 
        get {
            return this._driedPlantData; 
        } 
        set {
            this._driedPlantData = value;
            this._plantObject.SetActive(value != null && value.DryingAge != null && value.DryingAge.Stage != DryingStage.Empty);
        } 
    }
    GameObject _plantObject;
    Renderer _rendererMaterial;
    DriedPlantData _driedPlantData;
    private void Start()
    {
        _plantObject = transform.Find("Plant").gameObject;
        _rendererMaterial = _plantObject.GetComponent<Renderer>();
    }

    internal void UpdatePlantObject()
    {
        if (_rendererMaterial == null)
        {
            return;
        }

        Color32 newColor = new Color32(160, 160, 160, 255); // Grauton

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

        _rendererMaterial.material.color = newColor;
        _rendererMaterial.material.SetColor("_EmissionColor", newColor);
    }
}
