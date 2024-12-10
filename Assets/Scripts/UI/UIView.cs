using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIView : IDisposable
{
    public VisualTreeAsset Asset;
    protected VisualElement Root;
    protected VisualElement View;
    protected UIManager Manager;

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
    public abstract void Show();
    public abstract void Hide();
    public abstract void Dispose();
}
