using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainMenuView : UIView
{
    public MainMenuView(VisualElement root, UIManager manager) : base(root, manager)
    {

    }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/MainMenuView");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/MainMenuView");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("MainMenuView");

        //_controller = Manager.gameObject.AddComponent<HUDController>();
        //_controller.Initialize(View);
        RegisterButtonCallbacks();
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        base.OnCancelPerformed(context);
    }

    private void RegisterButtonCallbacks()
    {
        View.Q<Button>("continue-button").clicked += OnContinueButtonClicked;
        View.Q<Button>("save-button").clicked += OnSaveButtonClicked;
        View.Q<Button>("load-button").clicked += OnLoadButtonClicked;
        View.Q<Button>("close-button").clicked += OnCloseButtonClicked;
    }

    private void OnCloseButtonClicked()
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    private void OnLoadButtonClicked()
    {
        GameStateManagerSingleton.Instance.Load();
        SoundManagerSingleton.Instance.PlaySound("Click");
    }

    private void OnSaveButtonClicked()
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
        GameStateManagerSingleton.Instance.Save();
    }

    private void OnContinueButtonClicked()
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
        UIEvents.ShowPreviousView();
    }

    public override void Dispose()
    {
        base.Dispose();
        Root.Remove(View);
    }

    public override void Hide()
    {
        base.Hide();
        //View.style.display = DisplayStyle.None;
    }

    public override void Show()
    {
        base.Show();
        //View.style.display = DisplayStyle.Flex;
    }

}
