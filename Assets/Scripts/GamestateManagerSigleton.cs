using System.Collections.Generic;
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
                _instance.GameState = new GameState();
            }
            return _instance;
        }
    }
    public GameState GameState;
    // NOTE: alternatively to a prefab, you could use a ScriptableObject derived asset,
    // make a reference to it here, and populated that reference at the Resources.Load
    // line above.

    // implement your Awake, Start, Update, or other methods here... (optional)
}