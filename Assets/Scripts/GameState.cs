using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public Growlight Growlight;
    public int CurrentDay;
    public JsonableListWrapper<PlantData> PlantDataList;
    public JsonableListWrapper<string> UnlockedEncyclopediaEntries;
    public JsonableListWrapper<Seed> SamenInventar;
    public Room Room;
    public int Geld = 100;
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
        UnlockedEncyclopediaEntries = new(new List <String> {});
        Room = Room.START;
        ChatData = new();
        SamenInventar = new(new List<Seed>
            {
                new Seed("Indica", 2, true),
                new Seed("Sativa", 2, true),
                new Seed("Ruderalis", 2, true),    
            });
        DoneQuestsList = new(new());
        ActiveQuestsList = new(new());
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
public abstract class InventoryItem
{
    public string Name;
   
}
[Serializable]
public class Seed : InventoryItem
{
    public string Type; // Indica, Sativa, etc.
    public bool IsFeminized;
    public int Quantity;

    public Seed(string type, int quantity, bool isFeminized = true)
    {
        Type = type;
        IsFeminized = isFeminized;
        Name = $"{Type} Samen";
        Quantity = quantity;
    }    
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