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

    private void Start()
    {
        _mainRoom = GameObject.Find("MainRoom");
        _dryingRoom = FindInactiveObjectByName("DryingRoom");

        PlayerController.Instance.GoingToDryingRoomAction = GoingToDryingRoom;
        PlayerController.Instance.GoingToMainRoomAction = GoingToMainRoom;
    }
    private void Update()
    {
    }

    void GoingToDryingRoom()
    {
        _mainRoom.SetActive(false);
        _dryingRoom.SetActive(true);
    }
    void GoingToMainRoom()
    {
        _mainRoom.SetActive(true);
        _dryingRoom.SetActive(false);
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

