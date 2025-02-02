using System.Collections.Generic;
using System;
using UnityEngine;

public class Drying_Controller : MonoBehaviour
{
    public DriedData PlantDriedData;
    public GameObject PlantObject;
    public Action StageChanged;
    Outline _outline;
    private void Start()
    {
        PlantObject = transform.Find("Plant").gameObject;
        _outline = GetComponent<Outline>();
        StageChanged += OnStageChanged;
    }
    private void OnStageChanged()
    {
        if (PlantDriedData.Age.Stage == AgeDrying.DryingStage.Ready)
        {
            _outline.OutlineColor = Color.green;
        }
        else
        {
            _outline.OutlineColor = Color.yellow;
        }
    }
}