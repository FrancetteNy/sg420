using System;
using UnityEngine;
[Serializable]
public class GameState
{
    public int CurrentDay { get; private set; }
    public static Action DayChanged;

    public GameState()
    {
        CurrentDay = 1;
    }

    public void AdvanceDay()
    {
        CurrentDay++;
        DayChanged?.Invoke();
    }
}
