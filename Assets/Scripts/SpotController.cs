using UnityEngine;

public class SpotController : MonoBehaviour
{
    private HighlightController _highlightController;
    private Light _spotLight;
    private Renderer _lightRenderer;
    Material[] _materials;
    void Awake()
    {
        _highlightController = FindAnyObjectByType<HighlightController>();
        _spotLight = GetComponentInChildren<Light>();
        _lightRenderer = GetComponentInChildren<Renderer>();
        _materials = _lightRenderer.sharedMaterials;
        _materials[1] = new Material(_lightRenderer.materials[1]);
        _materials = _lightRenderer.sharedMaterials;
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
                _materials[1].EnableKeyword("_EMISSION");
            else
                _materials[1].DisableKeyword("_EMISSION");
        });

        highlightBuilder.Apply();

    }
}
