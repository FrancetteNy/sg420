using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class OnboardingView : UIView
{
    OnboardingController _controller;
    public OnboardingView(VisualElement root, UIManager manager) : base(root, manager)
    {
    }
    protected override void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/OnboardingView");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/OnboardingView");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("Onboarding");

        _controller = Manager.gameObject.AddComponent<OnboardingController>();
        _controller.Initialize(View);
        Hide();
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
    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        return;
    }
    public override void Show()
    {
        base.Show();
        _controller.enabled = true;
        _controller.StartOnboarding();
    }

    internal void SetData(List<OnboardingData> list)
    {
        _controller.SetData(list);
    }
}

