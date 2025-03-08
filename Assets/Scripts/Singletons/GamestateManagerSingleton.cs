using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameStateManagerSingleton : SingletonViaPrefab<GameStateManagerSingleton>
{
    public GameState GameState;
    public bool IsGameLoaded = false;
    protected override void InitializeSingleton()
    {
        base.InitializeSingleton();
        Instance._saveFilePath = Application.persistentDataPath + "/SaveData.json";
        Instance.GameState = new GameState();
        Instance.Load();
    }
    private void OnEnable()
    {
        GameState.EncyclopediaEntryUnlocked += UnlockEncyclopediaEntry;
    }

    private void OnDisable()
    {
        GameState.EncyclopediaEntryUnlocked -= UnlockEncyclopediaEntry;
    }

    public void AdvanceDay()
    {
        GameState.CurrentDay++;
        GameState.DayChanged?.Invoke();
        Save();
        UIEvents.AddNotification.Invoke(new NotificationData("Spiel gespeichert", $"Tag {GameState.CurrentDay} gestartet.", 3));
    }


    private string _saveFilePath;

    public void Save()
    {
        string writeToFile = JsonUtility.ToJson(GameState);
        //you can do whatever after, but for checking lets create file.
        File.WriteAllText(_saveFilePath, writeToFile);
    }

    public void Load()
    {
        if (IsGameLoaded)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (File.Exists(_saveFilePath))
        {
            string loadedData = File.ReadAllText(_saveFilePath);
            GameState = JsonUtility.FromJson<GameState>(loadedData);
            IsGameLoaded = true;
        }
        else
        {
            Debug.Log("File does not exist " + _saveFilePath);
        }
    }

    private void UnlockEncyclopediaEntry(string entry)
    {
        List<String> currentEntries = GameState.UnlockedEncyclopediaEntries.List;
        if (!currentEntries.Contains(entry))
        {
            GameState.UnlockedEncyclopediaEntries.List.Add(entry);
        }
    }

}