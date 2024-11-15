using System;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private readonly List<HighlightData> _highlightData = new();

    [SerializeField]
    private Outline.Mode _outlineMode = Outline.Mode.OutlineVisible;
    [SerializeField]
    private Color _outlineColor = Color.yellow;
    [SerializeField, Range(0f, 10f)]
    private float _outlineWidth = 5f;

    public HighlightBuilder BeginHighlightObject(GameObject objectToHighlight)
    {
        return new HighlightBuilder(this, objectToHighlight);
    }

    private void AddHighlightData(HighlightData data)
    {
        _highlightData.Add(data);
        SetOutlineSettings(data.Outline, data.OutlineMode, data.OutlineColor, data.OutlineWidth);
    }

    private void SetOutlineSettings(Outline outline, Outline.Mode outlineMode, Color outlineColor, float outlineWidth)
    {
        outline.OutlineWidth = outlineWidth;
        outline.OutlineMode = outlineMode;
        outline.OutlineColor = outlineColor;
    }

    public bool RemoveHighlightObject(GameObject objectToRemove)
    {
        int index = _highlightData.FindIndex(data => data.ObjectToHighlight == objectToRemove);
        if (index < 0)
            return false;

        var data = _highlightData[index];
        _highlightData.RemoveAt(index);
        Destroy(data.Outline);
        return true;
    }

    private void Update()
    {
        foreach (var data in _highlightData)
        {
            foreach (var renderer in data.ObjectToHighlight.GetComponentsInChildren<Renderer>())
            {
                if (IsMouseOver(renderer.gameObject))
                {
                    data.Outline.enabled = true;
                    if (Input.GetMouseButtonDown(0) && data.MouseClickFunction != null)
                    {
                        data.MouseClickFunction.Invoke(data);
                        break;
                    }
                }
                else
                {
                    data.Outline.enabled = false;
                }
            }
            if (!this.enabled)
            {
                break;
            }
        }
    }

    private bool IsMouseOver(GameObject obj)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out var hit) && hit.collider.gameObject == obj;
    }

    public class HighlightBuilder
    {
        private readonly HighlightController _controller;
        private readonly HighlightData _data;

        public HighlightBuilder(HighlightController controller, GameObject objectToHighlight)
        {
            _controller = controller;
            _data = new HighlightData
            {
                ObjectToHighlight = objectToHighlight,
                Outline = objectToHighlight.AddComponent<Outline>(),
                OutlineMode = controller._outlineMode,
                OutlineColor = controller._outlineColor,
                OutlineWidth = controller._outlineWidth,
                MouseClickFunction = null
            };
        }

        public HighlightBuilder WithOutlineMode(Outline.Mode mode)
        {
            _data.OutlineMode = mode;
            return this;
        }

        public HighlightBuilder WithOutlineColor(Color color)
        {
            _data.OutlineColor = color;
            return this;
        }

        public HighlightBuilder WithOutlineWidth(float width)
        {
            _data.OutlineWidth = width;
            return this;
        }

        public HighlightBuilder WithClickAction(Action<HighlightData> action)
        {
            _data.MouseClickFunction = action;
            return this;
        }

        public void Apply()
        {
            _controller.AddHighlightData(_data);
        }
    }

#nullable enable
    public class HighlightData
    {
        public GameObject ObjectToHighlight;
        public Outline Outline;
        public Action<HighlightData>? MouseClickFunction;
        public Outline.Mode OutlineMode;
        public Color OutlineColor;
        public float OutlineWidth;
    }
#nullable disable
}
