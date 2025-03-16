using UnityEngine;

public class WaterCanController : MonoBehaviour
{
    private HighlightController _highlightController;
    void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        ConstructHighlightAndClickFunction();
    }


    private void ConstructHighlightAndClickFunction()
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(this.gameObject);
        highlightBuilder.WithClickAction((data) =>
        {
            UIEvents.ShowGroupWateringView.Invoke();
        });

        highlightBuilder.Apply();
    }
}
