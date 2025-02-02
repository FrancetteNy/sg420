using System;
using UnityEngine;
public class PlantController : MonoBehaviour
{
    public PlantData PlantData;
    public Action StageChanged;
    Outline _outline;
    private void Start()
    {
        _outline = GetComponent<Outline>();
        StageChanged += OnStageChanged;
    }
    private void OnStageChanged()
    {
        if (PlantData.Age.Stage == Age.GrowthStage.FLOWERING)
        {
            _outline.OutlineColor = Color.green;
        }
        else
        {
            _outline.OutlineColor = Color.yellow;
        }
    }
    public bool IsPlantable()
    {
        return PlantData == null || PlantData.Strain == Strain.None;
    }

    public void PlantSeed(Strain strain)
    {
        if (PlantData == null)
        {
            PlantData = new PlantData();
        }
        PlantData.Strain = strain;
        PlantData.Age.Stage = Age.GrowthStage.GERMINATION;
        PlantData.Potsize = Potsize.Cultivation;
        PlantData.Sex = false;
        PlantData.Soil = new(0, 0);
    }

}