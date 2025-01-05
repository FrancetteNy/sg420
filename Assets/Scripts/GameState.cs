using System;

[Serializable]
public class GameState
{
    public Growlight Growlight;
    public int CurrentDay { get; private set; }
    public static Action DayChanged;

    public GameState()
    {
        CurrentDay = 1;
        Growlight = new();
    }

    public void AdvanceDay()
    {
        CurrentDay++;
        DayChanged?.Invoke();
    }
}
