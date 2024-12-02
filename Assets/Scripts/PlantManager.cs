using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public ObservableCollection<GameObject> Plants = new();
    private HighlightController _highlightController;
    private DetailViewController _detailViewController;
    private int _plantLayer;
    void Awake()
    {
        _plantLayer = LayerMask.NameToLayer("Plant");
        _highlightController = FindAnyObjectByType<HighlightController>();
        _detailViewController = FindAnyObjectByType<DetailViewController>();
        //Collect the initial Plants
        foreach (var plantController in gameObject.GetComponentsInChildren<PlantController>())
        {
            Plants.Add(plantController.gameObject);
            SetLayerOfAllChildren(plantController.gameObject);
        }
        //Make sure that everything is up to date if anything changes
        Plants.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) => UpdateHighlightAndDetailView());
        UpdateHighlightAndDetailView();
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
        _detailViewController.PlantControllers = Plants.Select(plant =>
        {
            SetLayerOfAllChildren(plant);
            return plant.GetComponent<PlantController>();
        }).ToList();
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
            _highlightController.enabled = false;
            _detailViewController.ActivateView(index, () =>
            {
                data.Outline.enabled = true;
                _highlightController.enabled = true;
            });
        });

        highlightBuilder.Apply();

    }

}
