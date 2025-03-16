using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
class Inventar : UIView
{
    InventarController _controller;

    
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
        UIEvents.HideInventar();
    }

}
