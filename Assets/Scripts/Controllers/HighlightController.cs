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
        data.Renderers = data.ObjectToHighlight.GetComponentsInChildren<Renderer>();
        Debug.Log(data.Outline);
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
    private void Start()
    {
        DontDestroyOnLoad(this.transform.root.gameObject);
    }
    private void Update()
    {
        foreach (var data in _highlightData)
        {
            if (IsMouseOver(data))
            {
                string tag = data.Tag;

                if (tag == "Plant")
                {
                    PlantController plantController = data.ObjectToHighlight.GetComponent<PlantController>();
                    Drying_Controller dryingController = data.ObjectToHighlight.GetComponent<Drying_Controller>();

                    if (plantController && plantController.PlantData.Age.Stage == Age.GrowthStage.FLOWERING)
                    {
                        _outlineColor = Color.green;
                    }
                    else if (dryingController && dryingController.Age.Stage == AgeDrying.DryingStage.Ready)
                    {
                        _outlineColor = Color.green;
                    }
                    else
                    {
                        _outlineColor = Color.yellow;
                    }
                }
                else
                {
                    _outlineColor = Color.yellow;
                }
                SetOutlineSettings(data.Outline, data.OutlineMode, _outlineColor, data.OutlineWidth);

                data.Outline.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {
                    if (tag == "Plant" && _outlineColor == Color.green && data.MouseClickFunction2 != null)
                    {
                        data.MouseClickFunction2.Invoke(data);
                        break;
                    }
                    if (data.MouseClickFunction != null)
                    {
                        SoundManagerSingleton.Instance.PlaySound("Click");
                        data.MouseClickFunction.Invoke(data);
                        break;
                    }
                }
            }
            else
            {
                data.Outline.enabled = false;
            }

            if (!this.enabled)
            {
                break;
            }
        }
    }

    private void GetAllChildRenderers(Transform transform, List<Renderer> list)
    {
        list.AddRange(transform.gameObject.GetComponents<Renderer>());
        foreach (Transform child in transform)
        {
            GetAllChildRenderers(child, list);
        }
    }

    private bool IsMouseOver(HighlightData data)
    {
        foreach (var renderer in data.Renderers)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == renderer.gameObject)
                return true;
        }
        return false;
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
                MouseClickFunction = null,
                MouseClickFunction2 = null,
                Tag = objectToHighlight.tag
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

        public HighlightBuilder WithClickAction2(Action<HighlightData> action)
        {
            _data.MouseClickFunction2 = action;
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
        public Action<HighlightData>? MouseClickFunction2;
        public Outline.Mode OutlineMode;
        public Color OutlineColor;
        public float OutlineWidth;
        public string Tag;
        public Renderer[] Renderers;
    }
#nullable disable
}