using System;
using System.Collections.Generic;
using UnityEngine;
using static Age;

[Serializable]
public class PlantController : MonoBehaviour
{
    public bool? Sex;
    public Potsize Potsize;
    public Age Age;
    public Strain Strain;
    public Soil Soil;

    public Dictionary<string, object> DataDictionary()
    {
        string growthStage = string.Empty;
        switch (Age.Stage)
        {
            case GrowthStage.Germination:
                growthStage = "Keimung";
                break;
            case GrowthStage.Seedling:
                growthStage = "Setzlingsphase";
                break;
            case GrowthStage.VegetativeGrowth:
                growthStage = "Vegetatives Wachstum";
                break;
            case GrowthStage.Flowering:
                growthStage = "Blüte";
                break;
        }
        string potsizeString = string.Empty;
        switch (Potsize)
        {
            case Potsize.Cultivation:
                potsizeString = "Anzuchtstopf(9cm)";
                break;
            case Potsize.Small:
                potsizeString = "Kleiner Topf(15cm)";
                break;
            case Potsize.Medium:
                potsizeString = "Mittlerer Topf(19cm)";
                break;
            case Potsize.Large:
                potsizeString = "Großer Topf(23cm)";
                break;
        }
        var data = new Dictionary<string, object>
        {
            { "sex", Sex },
            { "potsize", potsizeString },
            { "age", Age },
            { "strain", Strain },
            { "water", Soil.StoredWater },
            { "nutrients", Soil.StoredNutrients },
            { "growthStage",  growthStage}
        };
        return data;
    }
}

public enum Potsize
{
    Cultivation = 500,      // 9cm diameter
    Small = 1500,           // 15cm diameter
    Medium = 3000,          // 19cm diameter
    Large = 5000            // 23cm diameter
}

[Serializable]
public class Age
{
    public enum GrowthStage
    {
        Germination,
        Seedling,
        VegetativeGrowth,
        Flowering
    }
    public GrowthStage Stage;
    public int AgeNumber; // The AgeNumber has, depending on the Stage, a different meaning
    public Age(GrowthStage stage, int ageNumber)
    {
        Stage = stage;
        AgeNumber = ageNumber;
    }
    override public string ToString()
    {
        string interval = string.Empty;
        switch (Stage)
        {
            case GrowthStage.Germination:
            case GrowthStage.Seedling:
                if (AgeNumber == 1)
                {
                    interval = "Tag";
                }
                else
                {
                    interval = "Tage";
                }
                break;
            case GrowthStage.VegetativeGrowth:
            case GrowthStage.Flowering:
                if (AgeNumber == 1)
                {
                    interval = "Woche";
                }
                else
                {
                    interval = "Wochen";
                }

                break;
        }
        return $"{AgeNumber} {interval}";
    }
}

[Serializable]
public enum Strain
{
    Sativa,
    Indica,
    Ruderalis
}

[Serializable]
public class Soil
{
    public float StoredWater;
    public float StoredNutrients;

    public Soil(float storedWater, float storedNutrients)
    {
        StoredWater = storedWater;
        StoredNutrients = storedNutrients;
    }
}
