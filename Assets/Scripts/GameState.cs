using System;
using System.Collections.Generic;

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
    public int CompletedDriedPlants => CompletedDriedPlantDataList.List.Count;
    public JsonableListWrapper<string> UnlockedEncyclopediaEntries;
    public Room Room;

    public static Action DayChanged;
    public static Action<string> EncyclopediaEntryUnlocked;

    public ChatData ChatData;

    public JsonableListWrapper<QuestWithObjectiveIndex> DoneQuestsList;
    public JsonableListWrapper<QuestWithObjectiveIndex> ActiveQuestsList;

    public OnboardingDoneData OnboardingDoneData;

    public GameState()
    {
        CurrentDay = 1;
        Growlight = new();
        PlantDataList = new(new List<PlantData> { new(), new(), new(), new()});
        HarvestedPlantDataList = new();
        CurrentlyDryingPlants = new(new() { new(), new(), new(), new() });
        UnlockedEncyclopediaEntries = new();
        Room = Room.START;
        ChatData = new();
        DoneQuestsList = new();
        ActiveQuestsList = new();
        OnboardingDoneData = new();
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
    public OnboardingDoneData()
    {
        DetailviewOnboardingIsDone = false;
    }
}