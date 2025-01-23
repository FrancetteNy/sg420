using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;

    //public HUDController UI;

    //public GameObject Sativa;
    //IEnumerator Start()
    //{
    //    yield return null;
    //    Instance = this;
    //    UI = (UI != null) ? UI : FindAnyObjectByType<HUDController>();


 
    //}
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Camera.main.transform.position = GameObject.Find("DryingRoomCameraPosition").transform.position;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Camera.main.transform.position = GameObject.Find("MainRoomCameraPosition").transform.position;
        }
    }
    //public void UpdateTreeCount(string key, int num)
    //{
    //    if (_trees.ContainsKey(key))
    //    {
    //        _trees[key].Count += num;
    //        PlayerPrefs.SetInt(key, _trees[key].Count);
    //        UpdateUI(key);
    //    }
    //}

    //public int GetTreeCount(string key)
    //{
    //    return _trees.ContainsKey(key) ? _trees[key].Count : 0;
    //}

    //private void UpdateUI(string key)
    //{
    //    string keyWithoutDried = key.Replace("Dried", "").Trim();
    //    int treeDriedCount = _trees[$"{keyWithoutDried} Dried"].Count;
    //    _trees[key].TextMeshPro.text = $"{keyWithoutDried} : {_trees[keyWithoutDried].Count}, Dried : {treeDriedCount}";
    //}

    //public int GetTotalTrees()
    //{
    //    return GetTreeCount("Sativa") + GetTreeCount("Indica") + GetTreeCount("Ruderalis");
    //}

    //public int GetTotalTreesDried()
    //{
    //    return GetTreeCount("Sativa Dried") + GetTreeCount("Indica Dried") + GetTreeCount("Ruderalis Dried");
    //}
}

