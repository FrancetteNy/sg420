using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class LightOverview : UIView
{
    LightOverviewController _controller;
    public LightOverview(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/LightOverview");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/LightOverview");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("LightOverview");

        _controller = Manager.gameObject.AddComponent<LightOverviewController>();
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
        UIEvents.HideLightOverview();
    }
}

