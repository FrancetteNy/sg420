using System.Collections.Generic;
using System;
using UnityEngine;

public class Drying_Controller : MonoBehaviour
{
    public PlantDriedData PlantDriedData;
    public GameObject PlantObject;
    private void Start()
    {
        PlantObject = transform.Find("Plant").gameObject;
    }
}