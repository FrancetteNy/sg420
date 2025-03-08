using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class UIView : IDisposable
{
    public VisualTreeAsset Asset;
    protected VisualElement Root;
    protected VisualElement View;
    protected UIManager Manager;
    public bool IsOpen;

    public UIView(VisualElement root, UIManager manager)
    {
        Root = root;
        Manager = manager;
        Initialize();
    }
    protected virtual void Initialize()
    {
        if (Root == null)
        {
            Debug.LogError("Root not set");
            return;
        }
        if (Manager == null)
        {
            Debug.LogError("Manager not set");
            return;
        }
    }

    public virtual void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.ShowPreviousView();
    }
    public virtual void Show()
    {
        View.style.display = DisplayStyle.Flex;
        IsOpen = true;
    }
    public virtual void Hide()
    {
        View.style.display = DisplayStyle.None;
        IsOpen = false;
    }
    public virtual void Dispose()
    {
    }
    public virtual void BringToFront()
    {
        Root.BringToFront();
    }
}
