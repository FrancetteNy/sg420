using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public Growlight Growlight;
    public int CurrentDay;
    public JsonableListWrapper<PlantData> PlantDataList;
    public JsonableListWrapper<string> UnlockedEncyclopediaEntries;
    public Room Room;

    public static Action DayChanged;
    public static Action<string> EncyclopediaEntryUnlocked;

    public ChatData ChatData;
    

    public GameState()
    {
        CurrentDay = 1;
        Growlight = new();
        PlantDataList = new(new List<PlantData> { new(), new(), new(), new()});
        UnlockedEncyclopediaEntries = new(new List <String> {});
        Room = Room.START;
        ChatData = new();
    }
}

[Serializable]
public class ChatData
{
    public ChatData()
    {
        DoneChatIDs = new(new());
        MetRequirements = new(new());
        KnownNPCs = new(new());
        ChatIDsAvailable = new(new());
        ChatUnlocks = new(new());
        NextChanceToUnlockSomething = 0.3f;
    }
    public JsonableListWrapper<int> DoneChatIDs;
    public JsonableListWrapper<MetRequirements> MetRequirements;
    public JsonableListWrapper<string> KnownNPCs;
    public JsonableListWrapper<int> ChatIDsAvailable;
    public JsonableListWrapper<string> ChatUnlocks;
    public float NextChanceToUnlockSomething;
}

[Serializable]
public class MetRequirements{
    public MetRequirements(string requirementName, int requirementValue)
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
}

[Serializable]
/***
 * This can represent where and how many Plants there are
 ***/
public enum Room
{
    START
}