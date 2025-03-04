using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    GameObject _mainRoom;
    GameObject _dryingRoom;
    Light _globalLight;
    bool _isDark = false;
    private void Start()
    {
        _mainRoom = GameObject.Find("MainRoom");
        _dryingRoom = FindInactiveObjectByName("DryingRoom");
        _globalLight = GameObject.Find("GlobalLight").GetComponent<Light>();

        UIManager.GoingToDryingRoomAction += GoingToDryingRoom;
        UIManager.GoingToMainRoomAction += GoingToMainRoom;
    }
    void GoingToDryingRoom()
    {
        _mainRoom.SetActive(false);
        _dryingRoom.SetActive(true);

        if(!_isDark)
            ToggleGlobalLightDark();
    }
    void GoingToMainRoom()
    {
        _mainRoom.SetActive(true);
        _dryingRoom.SetActive(false);

        if (_isDark)
            ToggleGlobalLightDark();
    }
    void ToggleGlobalLightDark()
    {
        if (_isDark)
        {
            _isDark = false;
            _globalLight.intensity = 2;
            RenderSettings.ambientIntensity = 1;
            return;
        }

        _isDark = true;
        _globalLight.intensity = .5f;
        RenderSettings.ambientIntensity = .3f;
    }

    GameObject FindInactiveObjectByName(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }
}

