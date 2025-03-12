
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
class Shop : UIView
{
    ShopController _controller;
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

        Hide();
    }
    public override void Dispose()
    {
        base.Dispose();
        Root.Remove(View);
        GameObject.Destroy(_controller);
    }

    public override void Hide()
    {
        base.Hide();
        _controller.enabled = false;
        
    }


    public override void Show()
    {
        base.Show();
        _controller.enabled = true;
        
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.ShowMainMenuView();
    }
}
