using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class HUDView : UIView
{
    public HUDView(VisualElement root, UIManager manager): base(root, manager)
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
    }
    public override void Dispose()
    {
        Root.Remove(View);
    }

    public override void Hide()
    {
        View.style.display = DisplayStyle.None;
    }

    public override void Show()
    {
        View.style.display = DisplayStyle.Flex;
    }
}
