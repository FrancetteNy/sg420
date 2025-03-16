using UnityEngine;

public class PlantController : MonoBehaviour
{
    public PlantData PlantData;

    public bool IsPlantable()
    {
        return PlantData == null || PlantData.Strain == Strain.None;
    }

    public void PlantSeed(Seed seed)
    {

        PlantData = new PlantData(seed.Type, 100, Age.GrowthStage.GERMINATION);

    }

}

