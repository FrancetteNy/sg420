using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //GameStateManagerSingleton.Instance.ResetData();
            Camera.main.transform.position = GameObject.Find("DryingRoomCameraPosition").transform.position;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Camera.main.transform.position = GameObject.Find("MainRoomCameraPosition").transform.position;
        }
    }
   
}

