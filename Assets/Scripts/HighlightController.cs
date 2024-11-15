using System;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private List<HighlightData> highlightData = new();
    [SerializeField]
    private Outline.Mode outlineMode = Outline.Mode.OutlineVisible;
    [SerializeField]
    private Color outlineColor = Color.yellow;
    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 5f;

    public void AddHighlightObject(GameObject objectToHighlight, Action actionToInvokeOnClick)
    {
        var outline = objectToHighlight.AddComponent<Outline>();
        highlightData.Add(new HighlightData(objectToHighlight, outline, actionToInvokeOnClick));
        setOutlineSettings(outline);
    }
    public void AddHighlightObject(GameObject objectToHighlight)
    {
        var outline = objectToHighlight.AddComponent<Outline>();
        highlightData.Add(new HighlightData(objectToHighlight, outline));
        setOutlineSettings(outline);
    }
    public bool RemoveHighlightObject(GameObject objectToRemove)
    {
        int index = highlightData.FindIndex(data => data.objectToHighlight == objectToRemove);
        if (index < 0) return false;
        var data = highlightData[index];
        highlightData.RemoveAt(index);
        Destroy(data.outline);
        return true;
    }
    private void setOutlineSettings(Outline outline)
    {
        outline.OutlineWidth = outlineWidth;
        outline.OutlineMode = outlineMode;
        outline.OutlineColor = outlineColor;
    }
    private void Update()
    {
        foreach (var data in highlightData)
        {
            GameObject obj = data.objectToHighlight;
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (IsMouseOver(renderer.gameObject))
                {
                    data.outline.enabled = true;
                    if (Input.GetMouseButtonDown(0) && data.mouseClickFunction != null)
                    {
                        data.mouseClickFunction.Invoke();
                    }
                }
                else
                {
                    data.outline.enabled = false;
                }
            }
        }
    }

    private bool IsMouseOver(GameObject obj)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == obj)
        {
            return true;
        }
        return false;
    }
#nullable enable
    private struct HighlightData
    {
        public GameObject objectToHighlight;
        public Outline outline;
        public Action? mouseClickFunction;
        public HighlightData(GameObject objectToHighlight, Outline outline)
        {
            this.objectToHighlight = objectToHighlight;
            this.outline = outline;
            this.mouseClickFunction = null;
        }
        public HighlightData(GameObject objectToHighlight, Outline outline, Action mouseClickFunction)
        {
            this.objectToHighlight = objectToHighlight;
            this.outline = outline;
            this.mouseClickFunction = mouseClickFunction;
        }
    }
#nullable disable
}
