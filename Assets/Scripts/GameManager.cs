using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Dictionary<string, TreesDetails> trees = new Dictionary<string, TreesDetails>();

    public HUDController UI;

    public GameObject sativa;
    IEnumerator Start()
    {
        yield return null;
        instance = this;
        UI = (UI != null) ? UI : FindAnyObjectByType<HUDController>();

        trees = new Dictionary<string, TreesDetails>()
        {
            { "Sativa", new TreesDetails{ TextMeshPro = UI.sativaText } },
            { "Indica", new TreesDetails{ TextMeshPro = UI.indicaText } },
            { "Ruderalis", new TreesDetails{ TextMeshPro = UI.ruderalisText } },
            { "Sativa Dried", new TreesDetails{ TextMeshPro = UI.sativaText } },
            { "Indica Dried", new TreesDetails{ TextMeshPro = UI.indicaText } },
            { "Ruderalis Dried", new TreesDetails{ TextMeshPro = UI.ruderalisText } }
        };

        foreach (string key in trees.Keys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                UpdateTreeCount(key, PlayerPrefs.GetInt(key));
            }
        }
    }
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
    public void UpdateTreeCount(string key, int num)
    {
        if (trees.ContainsKey(key))
        {
            trees[key].count += num;
            PlayerPrefs.SetInt(key, trees[key].count);
            UpdateUI(key);
        }
    }

    public int GetTreeCount(string key)
    {
        return trees.ContainsKey(key) ? trees[key].count : 0;
    }

    private void UpdateUI(string key)
    {
        string keyWithoutDried = key.Replace("Dried", "").Trim();
        int treeDriedCount = trees[$"{keyWithoutDried} Dried"].count;
        trees[key].TextMeshPro.text = $"{keyWithoutDried} : {trees[keyWithoutDried].count}, Dried : {treeDriedCount}";
    }

    public int GetTotalTrees()
    {
        return GetTreeCount("Sativa") + GetTreeCount("Indica") + GetTreeCount("Ruderalis");
    }

    public int GetTotalTreesDried()
    {
        return GetTreeCount("Sativa Dried") + GetTreeCount("Indica Dried") + GetTreeCount("Ruderalis Dried");
    }
}

public class TreesDetails
{
    public int count = 0;
    public Label TextMeshPro;
}