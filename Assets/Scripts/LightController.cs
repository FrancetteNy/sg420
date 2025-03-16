using UnityEngine;

public class LightController : MonoBehaviour
{
    private HighlightController _highlightController;
    void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        //Collect the initial Plants
        //Make sure that everything is up to date if anything changes
        ConstructPlantHighlightAndClickFunction();
    }


    private void ConstructPlantHighlightAndClickFunction()
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(this.gameObject);
        highlightBuilder.WithClickAction((data) =>
        {
            UIEvents.ShowLightOverview.Invoke();
        });

        highlightBuilder.Apply();

    }
}

