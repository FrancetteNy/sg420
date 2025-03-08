using UnityEngine;

public class SpotController : MonoBehaviour
{
    private HighlightController _highlightController;
    private Light _spotLight;
    private Material _emmisiveMaterial;
    void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        _spotLight = GetComponentInChildren<Light>();
        var renderer = GetComponentInChildren<Renderer>();

        _emmisiveMaterial = renderer.sharedMaterials[1];
        //Collect the initial Plants
        //Make sure that everything is up to date if anything changes
        ConstructPlantHighlightAndClickFunction();
    }


    private void ConstructPlantHighlightAndClickFunction()
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(this.gameObject);
        highlightBuilder.WithClickAction((data) =>
        {
            bool isEnabled = _spotLight.enabled;
            _spotLight.enabled = !isEnabled;
            if (!isEnabled)
                _emmisiveMaterial.EnableKeyword("_EMISSION");
            else
                _emmisiveMaterial.DisableKeyword("_EMISSION");
        });

        highlightBuilder.Apply();

    }
}
