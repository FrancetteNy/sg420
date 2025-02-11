
using UnityEngine;
using UnityEngine.UIElements;

class Inventar : UIView
{
    InventarController _controller;
    HighlightController _highlightController;
    public Inventar(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/Inventar");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/Inventar");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("Inventar");

        _controller = Manager.gameObject.AddComponent<InventarController>();
        _controller.Initialize(View);

        _highlightController = GameObject.FindAnyObjectByType<HighlightController>();
        Hide();
    }
    public override void Dispose()
    {
        Root.Remove(View);
        GameObject.Destroy(_controller);
    }

    public override void Hide()
    {
        View.style.display = DisplayStyle.None;
        _controller.enabled = false;
        _highlightController.enabled = true;
    }


    public override void Show()
    {
        View.style.display = DisplayStyle.Flex;
        _controller.enabled = true;
        _highlightController.enabled = false;
    }

}
