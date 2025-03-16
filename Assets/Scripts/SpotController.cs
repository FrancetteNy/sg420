using UnityEngine;

public class SpotController : MonoBehaviour
{
    private HighlightController _highlightController;
    private Light _spotLight;
    private Material _emissiveMaterial;

    void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        _spotLight = GetComponentInChildren<Light>();

        var lightRenderer = GetComponentInChildren<Renderer>();

        // Material-Kopie erzeugen (instanziieren), damit Änderungen nur dieses Objekt betreffen
        _emissiveMaterial = new Material(lightRenderer.materials[1]);
        Material[] materials = lightRenderer.materials;
        materials[1] = _emissiveMaterial;
        lightRenderer.materials = materials;

        ConstructPlantHighlightAndClickFunction();
    }

    private void ConstructPlantHighlightAndClickFunction()
    {
        var highlightBuilder = _highlightController.BeginHighlightObject(this.gameObject);
        highlightBuilder.WithClickAction((data) =>
        {
            _spotLight.enabled = !_spotLight.enabled;
            if (_spotLight.enabled)
                _emissiveMaterial.EnableKeyword("_EMISSION");
            else
                _emissiveMaterial.DisableKeyword("_EMISSION");
        });

        highlightBuilder.Apply();
    }
}

