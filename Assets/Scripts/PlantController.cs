using System;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    public PlantData PlantData;

    public bool IsEmpty()
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
    }

}
