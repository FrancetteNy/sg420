
using UnityEngine;
using UnityEngine.UIElements;

class Shop : UIView
{
    ShopController _controller;
    HighlightController _highlightController;
    
    public Shop(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/Shop");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/Shop");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("Shop");

        _controller = Manager.gameObject.AddComponent<ShopController>();
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
        _controller.enabled = true;
        View.style.display = DisplayStyle.Flex;
        _highlightController.enabled = false;
    }

}
