using UnityEngine;

public class SpotController : MonoBehaviour
{
    private HighlightController _highlightController;
    private Light _spotLight;
    Material _emmisiveMaterial;
    void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        _spotLight = GetComponentInChildren<Light>();
        var lightRenderer = GetComponentInChildren<Renderer>();
        new Material(lightRenderer.materials[1]); // the constructor has somehow a sideffect that creates a new material for that renderer or something like that? idk
        _emmisiveMaterial = lightRenderer.sharedMaterials[1];
        ConstructPlantHighlightAndClickFunction();
    }


    private void ConstructPlantHighlightAndClickFunction()
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(this.gameObject);
        highlightBuilder.WithClickAction((data) =>
        {
            _spotLight.enabled = !_spotLight.enabled;
            if (_spotLight.enabled)
                _emmisiveMaterial.EnableKeyword("_EMISSION");
            else
                _emmisiveMaterial.DisableKeyword("_EMISSION");
        });

        highlightBuilder.Apply();

    }
}
