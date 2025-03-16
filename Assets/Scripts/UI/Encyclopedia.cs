
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class Encyclopedia : UIView
{
    EncyclopediaController _controller;
    public Encyclopedia(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/Encyclopedia");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/Encyclopedia");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("Encyclopedia");

        _controller = Manager.gameObject.AddComponent<EncyclopediaController>();
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
        _controller.ReloadEntries();
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.HideEncyclopedia();
    }
}

