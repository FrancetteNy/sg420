using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
class InventoryView : UIView
{
    InventoryController _controller;

    
    public InventoryView(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/Inventory");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/Inventory");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("Inventory");

        _controller = Manager.gameObject.AddComponent<InventoryController>();
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
        _controller.RefreshInventory();
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.HideInventar();
    }

}

