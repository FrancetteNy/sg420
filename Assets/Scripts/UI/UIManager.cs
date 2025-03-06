using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    VisualElement _root;
    List<UIView> _allUIViews = new();

    DetailView _detailView;
    HUDView _hudView;
    LightOverview _lightOverview;
    Encyclopedia _encyclopedia;
    ChatView _chatView;
    MainMenuView _mainMenuView;
    QuestLog _questLog;
    GroupWateringView _groupWateringView;

    NotificationManagerSingleton _notificationManager;

    UIView _currentView;
    UIView _previousView;

    public static Action GoingToDryingRoomAction;
    public static Action GoingToMainRoomAction;

    InputSystem_Actions _actions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        AddAllUIViews();
        SetupNotificationMananger();
        SetupActionSystem();
    }

    private void SetupActionSystem()
    {
        _actions = new InputSystem_Actions();
        _actions.Player.Enable();
        _actions.UI.Enable();
        _actions.UI.Cancel.performed += OnCancelPerformed;
        _actions.Player.MoveToDryingRoom.performed += MoveToDryingRoom_performed;
        _actions.Player.MoveToMainRoom.performed += MoveToMainRoom_performed;
    }
    private void MoveToMainRoom_performed(InputAction.CallbackContext obj)
    {
        GoingToMainRoomAction?.Invoke();
    }

    private void MoveToDryingRoom_performed(InputAction.CallbackContext obj)
    {
        GoingToDryingRoomAction?.Invoke();
    }
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        _currentView?.OnCancelPerformed(context);
    }

    private void SetupNotificationMananger()
    {
        _notificationManager = NotificationManagerSingleton.Instance;
        UIEvents.AddNotification += _notificationManager.AddNotification;
    }

    private void AddAllUIViews()
    {
        UIEvents.ShowPreviousView += ShowPreviousView;

        _chatView = new ChatView(_root, this);
        _detailView = new DetailView(_root, this);
        _hudView = new HUDView(_root, this);
        _lightOverview = new LightOverview(_root, this);
        _encyclopedia = new Encyclopedia(_root, this);
        _mainMenuView = new MainMenuView(_root, this);
        _questLog = new QuestLog(_root, this);
        _groupWateringView = new GroupWateringView(_root, this);

        UIEvents.ShowDetailView += OnDetailViewShown;
        UIEvents.HideDetailView += OnHudShown;

        UIEvents.ShowHUDView += OnHudShown;
        UIEvents.HideHUDView += _hudView.Hide;

        UIEvents.ShowLightOverview += OnLightOverviewShown;
        UIEvents.HideLightOverview += OnHudShown;

        UIEvents.ShowEncyclopedia += OnEncyclopediaShown;
        UIEvents.HideEncyclopedia += OnHudShown;

        UIEvents.ShowChatView += OnChatViewShown;
        UIEvents.HideChatView += OnHudShown;

        UIEvents.ShowMainMenuView += OnMainMenuViewShown;
        UIEvents.HideMainMenuView += OnHudShown;

        UIEvents.ShowQuestLog += OnQuestLogShown;
        UIEvents.HideQuestLog += OnHudShown;
        UIEvents.ShowGroupWateringView += OnGroupWateringShown;
        UIEvents.HideGroupWateringView += OnHudShown;


        _allUIViews.Add(_detailView);
        _allUIViews.Add(_hudView);
        _allUIViews.Add(_lightOverview);
        _allUIViews.Add(_encyclopedia);
        _allUIViews.Add(_chatView);
        _allUIViews.Add(_mainMenuView);
        _allUIViews.Add(_groupWateringView);

        _currentView = _hudView;
        _previousView = _hudView;
        UIEvents.ShowMainMenuView.Invoke();
    }


    private void OnDetailViewShown(int index) => ShowView(_detailView, index);
    private void OnHudShown() => ShowView(_hudView);
    private void OnLightOverviewShown() => ShowView(_lightOverview);
    private void OnEncyclopediaShown() => ShowView(_encyclopedia);
    private void OnChatViewShown() => ShowView(_chatView);
    private void OnMainMenuViewShown() => ShowView(_mainMenuView);
    private void OnQuestLogShown() => ShowView(_questLog);
    private void OnGroupWateringShown() => ShowView(_groupWateringView);


    private void OnDestroy()
    {
        UIEvents.ShowPreviousView -= ShowPreviousView;

        UIEvents.ShowDetailView -= OnDetailViewShown;
        UIEvents.HideDetailView -= OnHudShown;
        _detailView.Dispose();
        UIEvents.ShowHUDView -= OnHudShown;
        UIEvents.HideHUDView -= _hudView.Hide;
        _hudView.Dispose();
        UIEvents.ShowLightOverview -= OnLightOverviewShown;
        UIEvents.HideLightOverview -= OnHudShown;
        _lightOverview.Dispose();
        UIEvents.ShowEncyclopedia -= OnEncyclopediaShown;
        UIEvents.HideEncyclopedia -= OnHudShown;
        _encyclopedia.Dispose();
        UIEvents.ShowChatView -= OnChatViewShown;
        UIEvents.HideChatView -= OnHudShown;
        _chatView.Dispose();
        UIEvents.ShowMainMenuView -= OnMainMenuViewShown;
        UIEvents.HideMainMenuView -= OnHudShown;
        _mainMenuView.Dispose();
        UIEvents.ShowQuestLog -= OnQuestLogShown;
        UIEvents.HideQuestLog -= OnHudShown;
        _questLog.Dispose();
        UIEvents.ShowGroupWateringView -= OnGroupWateringShown;
        UIEvents.HideGroupWateringView -= OnHudShown;
        _groupWateringView.Dispose();


        _actions.UI.Cancel.performed -= OnCancelPerformed;
    }

    private void ShowView(UIView view, int? index = null)
    {
        if (view == _currentView)
            return;

        HideCurrentView();
        _previousView = _currentView;
        _currentView = view;

        if (index.HasValue)
            (view as DetailView).Show(index.Value);
        else
            view.Show();
    }

    private void ShowPreviousView()
    {
        if (_currentView == _previousView || _previousView is null || _currentView is null)
            return;
        ShowView(_previousView);
    }

    private void HideCurrentView()
    {
        if (_currentView != null)
        {
            _currentView.Hide();
        }
    }

}