using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuView : UIView
{
    List<Button> _buttonList;
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

        SceneManager.sceneLoaded += EnableButtons;
    }

    private void EnableButtons(Scene _, LoadSceneMode __)
    {
        foreach (var button in _buttonList)
        { 
            button.SetEnabled(true);
        }
    }
    private void DisableButtons()
    {
        foreach (var button in _buttonList)
        {
            button.SetEnabled(false);
        }
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        base.OnCancelPerformed(context);
    }

    private void RegisterButtonCallbacks()
    {
        _buttonList = new List<Button>();
        var continueButton = View.Q<Button>("continue-button");
        continueButton.clicked += OnContinueButtonClicked;
        _buttonList.Add(continueButton);
        var saveButton = View.Q<Button>("save-button");
        saveButton.clicked += OnSaveButtonClicked;
        _buttonList.Add(saveButton);
        var loadButton = View.Q<Button>("load-button");
        loadButton.clicked += OnLoadButtonClicked;
        _buttonList.Add(loadButton);
        var closeButton = View.Q<Button>("close-button");
        closeButton.clicked += OnCloseButtonClicked;
        _buttonList.Add(closeButton);
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
        DisableButtons();
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
