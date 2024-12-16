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

public class SingletonViaPrefab : MonoBehaviour
{
    // This is really the only blurb of code you need to implement a Unity singleton
    private static SingletonViaPrefab _instance;
    public static SingletonViaPrefab Instance
    {
        get
        {
            if (!_instance)
            {
                // NOTE: read docs to see directory requirements for Resources.Load!
                var prefab = Resources.Load<GameObject>("PathToYourSingletonViaPrefab");
                // create the prefab in your scene
                var inScene = Instantiate<GameObject>(prefab);
                // try find the instance inside the prefab
                _instance = inScene.GetComponentInChildren<SingletonViaPrefab>();
                // guess there isn't one, add one
                if (!_instance)
                    _instance = inScene.AddComponent<SingletonViaPrefab>();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_instance.transform.root.gameObject);
            }
            return _instance;
        }
    }

    // NOTE: alternatively to a prefab, you could use a ScriptableObject derived asset,
    // make a reference to it here, and populated that reference at the Resources.Load
    // line above.

    // implement your Awake, Start, Update, or other methods here... (optional)
}