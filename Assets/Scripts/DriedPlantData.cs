using System;

[Serializable]
public class DriedPlantData
{
    public PlantData OldPlantData;
    public DryingAge DryingAge;
    public DriedPlantData()
    {
        DryingAge = new();
    }

    public DriedPlantData Initialize(PlantData plantData)
    {
        OldPlantData = plantData;
        DryingAge = new();
        return this;
    }

}
public enum DryingStage
{
    Empty,
    DryingStart,
    DryingMid,
    DryingEnd,
    DryingFinished,

}
[Serializable]

public class DryingAge
{
    public DryingStage Stage;
    public int AgeNumber;
    public DryingAge() : this(DryingStage.Empty, 0)
    { 
    }
    public DryingAge(DryingStage stage, int ageNumber)
    {
        Stage = stage;
        AgeNumber = ageNumber;
    }
    public DryingStage GetNextStage() => Stage switch
    {
        DryingStage.Empty => DryingStage.DryingStart,
        DryingStage.DryingStart => DryingStage.DryingMid,
        DryingStage.DryingMid => DryingStage.DryingEnd,
        DryingStage.DryingEnd => DryingStage.DryingFinished,
        DryingStage.DryingFinished => DryingStage.DryingFinished,
        _ => throw new ArgumentOutOfRangeException(nameof(DryingStage), $"Not expected GrowthStage value {Stage}"),
    };
}

