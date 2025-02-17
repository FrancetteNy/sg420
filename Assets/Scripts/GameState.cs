using System;
using System.Collections.Generic;
using static DryingProcess;

[Serializable]
public class GameState
{
    public Growlight Growlight;
    public int CurrentDay;
    public JsonableListWrapper<PlantData> PlantDataList;

    public JsonableListWrapper<DriedData> PlantDriedDataList;
    public int TreesCount;
    public int Score;
    public int Getrocknet;
    public Room Room;

    public static Action DayChanged;
    public static Action InventorzChanged;
    public static Action ScoreChanged;
    public static Action GetrocknetChanged;
    public static Action ErnteAction;

    public JsonableListWrapper<string> UnlockedEncyclopediaEntries;

    public static Action<string> EncyclopediaEntryUnlocked;

    public GameState()
    {
        CurrentDay = 1;
        TreesCount = 0;
        Score = 0;
        Getrocknet = 0;
        Growlight = new();
        PlantDataList = new(new List<PlantData> { new(), new(), new(), new()});
        PlantDriedDataList = new(new List<DriedData> { new(), new(), new(), new() });

        UnlockedEncyclopediaEntries = new(new List <String> {});
        Room = Room.START;
    }

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