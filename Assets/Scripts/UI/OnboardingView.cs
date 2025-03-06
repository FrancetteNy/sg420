using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class OnboardingView : UIView
{
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
    }

    public override void Show()
    {
        base.Show();
    }
}
