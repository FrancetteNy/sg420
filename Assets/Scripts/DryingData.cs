using System.Collections.Generic;
using System;
using UnityEngine;
using static AgeDrying;

[Serializable]
public class DriedData
{
    public bool? Sex;
    public AgeDrying Age;
    public Strain Strain;

    public DriedData()
    {
        this.Sex = false;
        this.Age = new(DryingStage.Empty, 0);
        this.Strain = Strain.Sativa;
    }

    public Dictionary<string, object> DataDictionary()
    {
        string dryingStage = string.Empty;
        switch (Age.Stage)
        {
            case DryingStage.Empty:
                dryingStage = "Others";
                break;
            case DryingStage.DryingStart:
                dryingStage = "Drying Start";
                break;
            case DryingStage.DryingMid:
                dryingStage = "Drying Mid";
                break;
            case DryingStage.DryingEnd:
                dryingStage = "Drying End";
                break;
            case DryingStage.Ready:
                dryingStage = "Ready";
                break;
        }
        var data = new Dictionary<string, object>
    {
        { "sex", Sex },
        { "age", Age },
        { "strain", Strain },
        { "dryingStage",  dryingStage}
    };
        return data;
    }
}


[Serializable]
public class AgeDrying
{
    public enum DryingStage
    {
        Empty,
        DryingStart,
        DryingMid,
        DryingEnd,
        Ready,
    }
    public DryingStage Stage;
    public int AgeNumber; // The AgeNumber has, depending on the Stage, a different meaning
    public Strain Strain;
    public AgeDrying(DryingStage stage, int ageNumber)
    {
        Stage = stage;
        AgeNumber = ageNumber;
    }
    override public string ToString()
    {
        string interval = string.Empty;
        switch (Stage)
        {
            case DryingStage.Ready:
                if (AgeNumber == 1)
                {
                    interval = "Tag";
                }
                else
                {
                    interval = "Tage";
                }
                break;
        }
        return $"{AgeNumber} {interval}";
    }

    public DryingStage GetNextStage() => Stage switch
    {
        DryingStage.Empty => DryingStage.DryingStart,
        DryingStage.DryingStart => DryingStage.DryingMid,
        DryingStage.DryingMid => DryingStage.DryingEnd,
        DryingStage.DryingEnd => DryingStage.Ready,
        DryingStage.Ready => DryingStage.Ready,
        _ => throw new ArgumentOutOfRangeException(nameof(DryingStage), $"Not expected GrowthStage value {Stage}"),
    };

    public void ResetPlantData()
    {
        Stage = DryingStage.Empty;
        AgeNumber = 0;
        Strain = Strain.None;
    }
}