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

    

    public GameState()
    {
        CurrentDay = 1;
        Growlight = new();
        PlantDataList = new(new List<PlantData> { new(), new(), new(), new()});
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