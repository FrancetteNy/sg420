
using static Age;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class PlantData
{
    public bool? Sex;
    public Potsize Potsize;
    public Age Age;
    public Strain Strain;
    public Soil Soil;


    public PlantData()
    {
        this.Sex = false;
        this.Potsize = Potsize.Cultivation;
        this.Age = new(GrowthStage.EMPTY, 0);
        this.Strain = Strain.Sativa;
        this.Soil = new(0, 0);
    }
    public void PlantSeed(Strain strain)
    {
        if (Age.Stage != GrowthStage.EMPTY)
        {
            Debug.LogWarning("Samen kann nicht gepflanzt werden: Der Topf ist nicht leer !");
            return;
        }

        Age.Stage = GrowthStage.GERMINATION;
        Age.AgeNumber = 0; // La germination commence
        Strain = strain;
        Soil = new Soil(100, 50); // Initialisation du sol avec de l'eau et des nutriments
        Debug.Log("Erfolgreich gepflanzter Samen !");
    }



    public Dictionary<string, object> DataDictionary()
    {
        string growthStage = string.Empty;
        switch (Age.Stage)
        {
            case GrowthStage.GERMINATION:
                growthStage = "Keimung";
                break;
            case GrowthStage.SEEDLING:
                growthStage = "Setzlingsphase";
                break;
            case GrowthStage.VEGETATIVEGROWTH:
                growthStage = "Vegetatives Wachstum";
                break;
            case GrowthStage.FLOWERING:
                growthStage = "Blüte";
                break;
            case GrowthStage.EMPTY:
                growthStage = "Leer";
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
        EMPTY,
        GERMINATION,
        SEEDLING,
        VEGETATIVEGROWTH,
        FLOWERING,
        FADED
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
            case GrowthStage.GERMINATION:
            case GrowthStage.SEEDLING:
                if (AgeNumber == 1)
                {
                    interval = "Tag";
                }
                else
                {
                    interval = "Tage";
                }
                break;
            case GrowthStage.VEGETATIVEGROWTH:
            case GrowthStage.FLOWERING:
                if (AgeNumber == 1)
                {
                    interval = "Woche";
                }
                else
                {
                    interval = "Wochen";
                }

                break;
            case GrowthStage.FADED:
                interval = " Tage zu alt";
                break;
        }
        return $"{AgeNumber} {interval}";
    }

    public GrowthStage GetNextStage() => Stage switch
    {
        GrowthStage.GERMINATION => GrowthStage.SEEDLING,
        GrowthStage.SEEDLING => GrowthStage.VEGETATIVEGROWTH,
        GrowthStage.VEGETATIVEGROWTH => GrowthStage.FLOWERING,
        GrowthStage.FLOWERING => GrowthStage.FADED,
        GrowthStage.FADED => GrowthStage.FADED,
        _ => throw new ArgumentOutOfRangeException(nameof(GrowthStage), $"Not expected GrowthStage value {Stage}"),
    };
}
[Serializable]
public enum Strain
{
    None,
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
