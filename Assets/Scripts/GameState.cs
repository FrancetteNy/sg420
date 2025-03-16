using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameState
{
    public Growlight Growlight;
    public int CurrentDay;
    public JsonableListWrapper<PlantData> PlantDataList;
    public JsonableListWrapper<PlantData> HarvestedPlantDataList;
    public JsonableListWrapper<DriedPlantData> CurrentlyDryingPlants;
    public JsonableListWrapper<DriedPlantData> CompletedDriedPlantDataList;
    public int HarvestedPlantCount => HarvestedPlantDataList.List.Count;
    public int CompletedDriedPlantsCount => CompletedDriedPlantDataList.List.Count;
    public JsonableListWrapper<string> UnlockedEncyclopediaEntries;
    public JsonableListWrapper<InventoryItem> Inventory;
    public List<Seed> SeedsInInventory => Inventory.List.FindAll((item) => item is Seed).Select((item) => item as Seed).ToList();
    public Dictionary<Seed, int> AvailableSeedsPerType => SeedsInInventory.GroupBy(seed => seed, seed => seed.GetType()).ToDictionary(group => group.Key, group => group.Count()); 

    public Room Room;
    public int Money = 100;

    public static Action DayChanged;
    public static Action<string> EncyclopediaEntryUnlocked;
    public static Action UpdateHUD;


    public ChatData ChatData;

    public JsonableListWrapper<QuestWithObjectiveIndex> DoneQuestsList;
    public JsonableListWrapper<QuestWithObjectiveIndex> ActiveQuestsList;

    public OnboardingDoneData OnboardingDoneData;

    public int CurrentScore;


    public GameState()
    {
        CurrentDay = 1;
        Growlight = new();
        PlantDataList = new(new List<PlantData> { new(), new(), new(), new()});
        HarvestedPlantDataList = new();
        CurrentlyDryingPlants = new(new() { new(), new(), new(), new() });
        CompletedDriedPlantDataList = new();
        UnlockedEncyclopediaEntries = new();
        UnlockedEncyclopediaEntries.List.Add("DetailViewDefault");
        ChatData = new();
        Inventory = new();
        
        DoneQuestsList = new();
        ActiveQuestsList = new();
        OnboardingDoneData = new();
        CurrentScore = 0;
    }
}

[Serializable]
public class ChatData
{
    public ChatData()
    {
        DoneChatIDs = new(new());
        MetRequirements = new(new());
        KnownNPCs = new(new() { "Lara Gruen" });
        ChatIDsAvailable = new(new());
        ChatUnlocks = new(new());
        NextChanceToUnlockSomething = 0.3f;
    }
    public JsonableListWrapper<string> DoneChatIDs;
    public JsonableListWrapper<MetRequirement> MetRequirements;
    public JsonableListWrapper<string> KnownNPCs;
    public JsonableListWrapper<string> ChatIDsAvailable;
    public JsonableListWrapper<string> ChatUnlocks;
    public float NextChanceToUnlockSomething;
}

[Serializable]
public class MetRequirement{
    public MetRequirement(string requirementName, int requirementValue)
    {
        RequirementName = requirementName;
        RequirementValue = requirementValue;
    }
    public string RequirementName;
    public int RequirementValue;
}

[Serializable]
public class JsonableListWrapper<T>
{
    public List<T> List;
    public JsonableListWrapper(List<T> list) => this.List = list;
    public JsonableListWrapper() : this(new()) { }
}


[Serializable]
/***
 * This can represent where and how many Plants there are
 ***/
public enum Room
{
    START
}
[Serializable]
public class OnboardingDoneData
{
    public bool DetailviewOnboardingIsDone;
    public bool HudOnboardingIsDone;
    public OnboardingDoneData()
    {
        DetailviewOnboardingIsDone = false;
        HudOnboardingIsDone = false;
    }
}