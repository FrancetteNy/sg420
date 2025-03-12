using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HUDView : UIView
{
    HUDController _controller;
    public HUDView(VisualElement root, UIManager manager) : base(root, manager)
    {
    }

    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/HUD");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/HUD");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("HUD");

        _controller = Manager.gameObject.AddComponent<HUDController>();
        _controller.Initialize(View);
        Hide();
    }
    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.ShowMainMenuView();
    }

    public override void Dispose()
    {
        base.Dispose();
        Root.Remove(View);
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
}
