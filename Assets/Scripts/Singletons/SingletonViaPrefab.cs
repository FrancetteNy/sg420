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

/// <summary>
/// Just extend this class like public class SoundManagerSingleton : SingletonViaPrefab<SoundManagerSingleton>
/// You need a "EmptySingletonObject"-prefab that is the default for all Singletons
/// If you want to add more, you can either just add them in an overwritten InitializeSingleton or just override the whole public static T Instance and use a custom prefab
/// </summary>
/// <typeparam name="T">This is your Classname</typeparam>
public class SingletonViaPrefab<T> : MonoBehaviour where T : MonoBehaviour
{
    // This is really the only blurb of code you need to implement a Unity singleton
    protected static T Protected_Instance;
    protected static GameObject Inscene;
    public static T Instance
    {
        get
        {
            if (!Protected_Instance)
            {
                // NOTE: read docs to see directory requirements for Resources.Load!
                var prefab = Resources.Load<GameObject>("EmptySingletonObject");
                if (prefab == null)
                {
                    Debug.LogError("Prefab not found at: " + "EmptySingletonObject");
                    return null;
                }
                // create the prefab in your scene
                Inscene = Instantiate<GameObject>(prefab);
                // try find the instance inside the prefab
                Protected_Instance = Inscene.GetComponentInChildren<T>();
                // guess there isn't one, add one
                if (!Protected_Instance)
                    Protected_Instance = Inscene.AddComponent<T>();
                Inscene.name = typeof(T).Name;
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(Protected_Instance.transform.root.gameObject);
                (Protected_Instance as SingletonViaPrefab<T>)?.InitializeSingleton();
            }
            return Protected_Instance;
        }
    }

    // Allow derived classes to override initialization behavior.
    protected virtual void InitializeSingleton()
    {
        // Default: do nothing.
    }

    // NOTE: alternatively to a prefab, you could use a ScriptableObject derived asset,
    // make a reference to it here, and populated that reference at the Resources.Load
    // line above.

    // implement your Awake, Start, Update, or other methods here... (optional)
}