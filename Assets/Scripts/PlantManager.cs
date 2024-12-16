using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public ObservableCollection<GameObject> Plants = new();
    private HighlightController _highlightController;
    private int _plantLayer;
    void Start()
    {
        _plantLayer = LayerMask.NameToLayer("Plant");
        _highlightController = FindAnyObjectByType<HighlightController>();
        //Collect the initial Plants
        foreach (var plantController in gameObject.GetComponentsInChildren<PlantController>())
        {
            Plants.Add(plantController.gameObject);
            SetLayerOfAllChildren(plantController.gameObject);
        }
        //Make sure that everything is up to date if anything changes
        Plants.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) => UpdateHighlightAndDetailView());
        UpdateHighlightAndDetailView();
        UIEvents.ShowDetailView += (_) => _highlightController.enabled = false;
    }

    private void SetLayerOfAllChildren(GameObject gameObject)
    {
        gameObject.layer = _plantLayer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayerOfAllChildren(child.gameObject);
        }
    }

    private void UpdateHighlightAndDetailView()
    {
        //DetailViewController.PlantsChanged.Invoke();
        for (int i = 0; i < Plants.Count; i++)
        {
            ConstructPlantHighlightAndClickFunction(i);
        }
    }

    private void ConstructPlantHighlightAndClickFunction(int index)
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(Plants[index]);
        highlightBuilder.WithClickAction((data) =>
        {
            data.Outline.enabled = false;
            UIEvents.ShowDetailView.Invoke(index);
        });

        highlightBuilder.Apply();

    }

}
