using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuView : UIView
{
    List<Button> _buttonList;
    Button _continueButton;
    Button _loadButton;
    Button _saveButton;
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
        RegisterButtonCallbacks();

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
        var newGameButton = View.Q<Button>("new-game-button");
        newGameButton.clicked += OnNewGameButtonClicked;
        _buttonList.Add(newGameButton);
        _continueButton = View.Q<Button>("continue-button");
        _continueButton.clicked += OnContinueButtonClicked;
        _buttonList.Add(_continueButton);
        _saveButton = View.Q<Button>("save-button");
        _saveButton.clicked += OnSaveButtonClicked;
        _buttonList.Add(_saveButton);
        _loadButton = View.Q<Button>("load-button");
        _loadButton.clicked += OnLoadButtonClicked;
        _buttonList.Add(_loadButton);
        var closeButton = View.Q<Button>("close-button");
        closeButton.clicked += OnCloseButtonClicked;
        _buttonList.Add(closeButton);
    }

    private void OnNewGameButtonClicked()
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
        GameStateManagerSingleton.Instance.NewGame();
        DisableButtons();
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
        _loadButton.SetEnabled(GameStateManagerSingleton.Instance.HasGameToLoad);
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

    public override void Show()
    {
        base.Show();
        _continueButton.SetEnabled(GameStateManagerSingleton.Instance.IsGameLoaded);
        _saveButton.SetEnabled(GameStateManagerSingleton.Instance.IsGameLoaded);
        _loadButton.SetEnabled(GameStateManagerSingleton.Instance.HasGameToLoad);
    }

}
