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
        _globalLight = GameObject.Find("GlobalLight")?.GetComponent<Light>();

        // Überprüfen, ob alles korrekt zugewiesen wurde
        if (_mainRoom == null)
        {
            Debug.LogError("MainRoom nicht gefunden!");
        }

        if (_dryingRoom == null)
        {
            Debug.LogError("DryingRoom nicht gefunden!");
        }

        if (_globalLight == null)
        {
            Debug.LogError("GlobalLight nicht gefunden!");
        }

        UIManager.GoingToDryingRoomAction += GoingToDryingRoom;
        UIManager.GoingToMainRoomAction += GoingToMainRoom;
    }

    void GoingToDryingRoom()
    {
        if (_mainRoom != null && _dryingRoom != null)
        {
            _mainRoom.SetActive(false);
            _dryingRoom.SetActive(true);

            if (!_isDark)
                ToggleGlobalLightDark();
        }
        else
        {
            Debug.LogWarning("MainRoom oder DryingRoom fehlt, kann nicht wechseln.");
        }
    }

    void GoingToMainRoom()
    {
        if (_mainRoom != null && _dryingRoom != null)
        {
            _mainRoom.SetActive(true);
            _dryingRoom.SetActive(false);

            if (_isDark)
                ToggleGlobalLightDark();
        }
        else
        {
            Debug.LogWarning("MainRoom oder DryingRoom fehlt, kann nicht wechseln.");
        }
    }

    void ToggleGlobalLightDark()
    {
        if (_globalLight == null)
            return;

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
            if (obj.name == name && obj != null)
            {
                return obj;
            }
        }
        return null;
    }

    private void OnDestroy()
    {
        // Event-Handler abmelden
        UIManager.GoingToDryingRoomAction -= GoingToDryingRoom;
        UIManager.GoingToMainRoomAction -= GoingToMainRoom;
    }
}
