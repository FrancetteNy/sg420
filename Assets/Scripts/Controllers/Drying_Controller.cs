using System.Collections.Generic;
using System;
using UnityEngine;
using static AgeDrying;

public class Drying_Controller : MonoBehaviour
{
    public bool? Sex;
    public AgeDrying Age;
    public Strain Strain;
    Outline _outline;
    GameObject _plantObject;
    private HighlightController _highlightController;

    private void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        ConstructPlantHighlightAndClickFunction();
    }
    private void Start()
    {
        _outline = GetComponent<Outline>();
    }

    public Dictionary<string, object> DataDictionary()
    {
        string dryingStage = string.Empty;
        switch (Age.Stage)
        {
            case DryingStage.Drying:
                dryingStage = "Drying";
                break;
            case DryingStage.Ready:
                dryingStage = "Ready";
                break;
        }
        var data = new Dictionary<string, object>
        {
            { "sex", Sex },
            { "age", Age },
            { "strain", Strain },
            { "dryingStage",  dryingStage}
        };
        return data;
    }

    private void ConstructPlantHighlightAndClickFunction()
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(this.gameObject);
        highlightBuilder.WithClickAction((data) =>
        {
            if(GameManager.instance.GetTreeCount(Strain.ToString()) > 0 && !_plantObject)
            {
                GameManager.instance.UpdateTreeCount(Strain.ToString(), -1);
                _plantObject = Instantiate(GameManager.instance.sativa, transform);
            }
        });
        highlightBuilder.WithClickAction2((data) =>
        {
            Age.Stage = AgeDrying.DryingStage.Drying;
            Destroy(_plantObject);
            GameManager.instance.UpdateTreeCount($"{Strain} Dried",1);

            int totalTreesDried = GameManager.instance.GetTotalTreesDried();
            //NotificationManager.Instance.PushNotificationDetailsOfTree(Strain.ToString(), 10, sativaCount_Dried);
        });

        highlightBuilder.Apply();
    }
}


[Serializable]
public class AgeDrying
{
    public enum DryingStage
    {
        Drying,
        Ready
    }
    public DryingStage Stage;
    public int AgeNumber; // The AgeNumber has, depending on the Stage, a different meaning
    public AgeDrying(DryingStage stage, int ageNumber)
    {
        Stage = stage;
        AgeNumber = ageNumber;
    }
    override public string ToString()
    {
        string interval = string.Empty;
        switch (Stage)
        {
            case DryingStage.Drying:
            case DryingStage.Ready:
                if (AgeNumber == 1)
                {
                    interval = "Tag";
                }
                else
                {
                    interval = "Tage";
                }
                break;
        }
        return $"{AgeNumber} {interval}";
    }
}