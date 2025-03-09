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
        _emmisiveMaterial = new Material(lightRenderer.materials[1]);
        _emmisiveMaterial = lightRenderer.sharedMaterials[1]; // i dont understand why we need this, but we need this?
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
