using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// by @kurtdekker - to make a Unity singleton that has some
// https://gist.github.com/kurtdekker/2f07be6f6a844cf82110fc42a774a625
// prefab-stored, data associated with it, eg a music manager
//
// To use: access with SingletonViaPrefab.Instance
//
// To set up:
//	- Copy this file (duplicate it)
//	- rename class SingletonViaPrefab to your own classname
//	- rename CS file too
//	- create the prefab asset associated with this singleton
//		NOTE: read docs on Resources.Load() for where it must exist!!
//
// DO NOT DRAG THE PREFAB INTO A SCENE! THIS CODE AUTO-INSTANTIATES IT!
//
// I do not recommend subclassing unless you really know what you're doing.

public class GameStateManagerSingleton : MonoBehaviour
{
    private static GameStateManagerSingleton _instance;
    public static GameStateManagerSingleton Instance
    {
        get
        {
            if (!_instance)
            {
                // NOTE: read docs to see directory requirements for Resources.Load!
                var prefab = Resources.Load<GameObject>("GameStateManagerSingleton");
                // create the prefab in your scene
                var inScene = Instantiate<GameObject>(prefab);
                // try find the instance inside the prefab
                _instance = inScene.GetComponentInChildren<GameStateManagerSingleton>();
                // guess there isn't one, add one
                if (!_instance)
                    _instance = inScene.AddComponent<GameStateManagerSingleton>();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_instance.transform.root.gameObject);

                // get GameState (load if available, else create new one and save)
                _instance._saveFilePath = Application.persistentDataPath + "/SaveData.json";
                _instance.GameState = new GameState();
               _instance.Load();
            }
            return _instance;
        }
    }
    public GameState GameState;

    public void AdvanceDay()
    {
        GameState.CurrentDay++;
        GameState.DayChanged?.Invoke();
        Save();
    }

    public void AdvanceTreesCount(int num)
    {
        GameState.TreesCount += num;
        GameState.InventorzChanged?.Invoke();
        Save();
    }

    public void AdvanceScore(int score)
    {
        GameState.Score += score;
        GameState.ScoreChanged?.Invoke();
        Save();
    }

    public void AdvanceGetrocknet(int num)
    {
        GameState.Getrocknet += num;
        GameState.GetrocknetChanged?.Invoke();
        Save();
    }

    private string _saveFilePath;

    public void Save()
    {
        string writeToFile = JsonUtility.ToJson(GameState);
        //you can do whatever after, but for checking lets create file.
        File.WriteAllText(_saveFilePath, writeToFile);
    }

    public void Load() {
        if (File.Exists(_saveFilePath))
        {
            string loadedData = File.ReadAllText(_saveFilePath);
            GameState = JsonUtility.FromJson<GameState>(loadedData);

        }
        else
        {
            Debug.Log("File does not exist " + _saveFilePath);
        }
    }
    // NOTE: alternatively to a prefab, you could use a ScriptableObject derived asset,
    // make a reference to it here, and populated that reference at the Resources.Load
    // line above.

    // implement your Awake, Start, Update, or other methods here... (optional)
}