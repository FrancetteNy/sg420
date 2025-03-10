using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    VisualElement _root;

    DetailView _detailView;
    HUDView _hudView;
    LightOverview _lightOverview;
    Encyclopedia _encyclopedia;
    Inventar _inventar;
    Shop _shop;
    ChatView _chatView;
    MainMenuView _mainMenuView;
    QuestLog _questLog;
    GroupWateringView _groupWateringView;
    OnboardingView _onboardingView;

    NotificationManager _notificationManager;

    UIView _currentView;
    UIView _previousView;

    Stack<UIView> _overlayViews;
    InputSystem_Actions _actions;

    HighlightController _highlightController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _overlayViews = new Stack<UIView>();
        AddAllUIViews();
        _notificationManager = gameObject.AddComponent<NotificationManager>();
        SetupNotificationMananger();
        SetupActionSystem();
        _highlightController = FindAnyObjectByType<HighlightController>();
    }

    private void SetupActionSystem()
    {
        _actions = new InputSystem_Actions();
        _actions.UI.Enable();
        _actions.UI.Cancel.performed += OnCancelPerformed;
    }
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!GameStateManagerSingleton.Instance.IsGameLoaded)
            return;
        if(_overlayViews.Count > 0)
        {
            var view = _overlayViews.Peek();
            view.OnCancelPerformed(context);
        }
        else if (_currentView != null) 
        {
            _currentView?.OnCancelPerformed(context);
        }
    }

    private void SetupNotificationMananger()
    {
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
        _inventar = new Inventar(_root, this);
        _mainMenuView = new MainMenuView(_root, this);
        _shop = new Shop(_root, this);
        _questLog = new QuestLog(_root, this);
        _groupWateringView = new GroupWateringView(_root, this);
        _onboardingView = new OnboardingView(_root, this);

        UIEvents.ShowDetailView += OnDetailViewShown;
        UIEvents.HideDetailView += OnHudShown;

        UIEvents.ShowHUDView += OnHudShown;
        UIEvents.HideHUDView += _hudView.Hide;

        UIEvents.ShowLightOverview += OnLightOverviewShown;
        UIEvents.HideLightOverview += HideOverlay;

        UIEvents.ShowInventar += OnInventarShown;
        UIEvents.HideInventar += OnHudShown;

        UIEvents.ShowShop += OnShopShown;
        UIEvents.HideShop += OnHudShown;
        
        UIEvents.HideEncyclopedia += () => ShowView(_hudView);
        UIEvents.ShowEncyclopedia += OnEncyclopediaShown;
        UIEvents.HideEncyclopedia += HideOverlay;

        UIEvents.ShowChatView += OnChatViewShown;
        UIEvents.HideChatView += OnHudShown;

        UIEvents.ShowMainMenuView += OnMainMenuViewShown;
        UIEvents.HideMainMenuView += OnHudShown;

        UIEvents.ShowQuestLog += OnQuestLogShown;
        UIEvents.HideQuestLog += HideOverlay;

        UIEvents.ShowGroupWateringView += OnGroupWateringShown;
        UIEvents.HideGroupWateringView += HideOverlay;

        UIEvents.ShowOnboardingView += OnOnboardingViewShown;
        UIEvents.HideOnboardingView += HideOverlay;

        _previousView = null;
        if (GameStateManagerSingleton.Instance.IsGameLoaded)
        {
            _currentView = _mainMenuView;
            UIEvents.ShowHUDView.Invoke();
        }
        else
        {
            _currentView = _hudView;
            UIEvents.ShowMainMenuView.Invoke();
        }
    }


    private void OnDetailViewShown(int index) => ShowView(_detailView, index);
    private void OnHudShown() => ShowView(_hudView);
    private void OnLightOverviewShown() => ShowOverlay(_lightOverview);
    private void OnEncyclopediaShown() => ShowOverlay(_encyclopedia);
    private void OnChatViewShown() => ShowView(_chatView);
    private void OnMainMenuViewShown() => ShowView(_mainMenuView);
    private void OnInventarShown () => ShowView(_inventar);
    private void OnShopShown () => ShowView(_shop);
    private void OnQuestLogShown() => ShowOverlay(_questLog);
    private void OnGroupWateringShown() => ShowOverlay(_groupWateringView);
    private void OnOnboardingViewShown(List<OnboardingData> list) {
        _onboardingView.SetData(list);
        ShowOverlay(_onboardingView);
    }


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
        UIEvents.HideLightOverview -= HideOverlay;
        _lightOverview.Dispose();
        UIEvents.ShowEncyclopedia -= OnEncyclopediaShown;
        UIEvents.HideEncyclopedia -= HideOverlay;
        _encyclopedia.Dispose();
        UIEvents.ShowInventar -= OnInventarShown;
        UIEvents.HideInventar -= OnHudShown;
        _inventar.Dispose();
        UIEvents.ShowShop -= OnShopShown;
        UIEvents.HideShop -= OnHudShown;
        _shop.Dispose();
        UIEvents.ShowChatView -= () => ShowView(_chatView);
        UIEvents.HideChatView -= () => ShowView(_hudView);
        UIEvents.ShowChatView -= OnChatViewShown;
        UIEvents.HideChatView -= OnHudShown;
        _chatView.Dispose();
        UIEvents.ShowMainMenuView -= OnMainMenuViewShown;
        UIEvents.HideMainMenuView -= OnHudShown;
        _mainMenuView.Dispose();
        UIEvents.ShowQuestLog -= OnQuestLogShown;
        UIEvents.HideQuestLog -= HideOverlay;
        _questLog.Dispose();
        UIEvents.ShowGroupWateringView -= OnGroupWateringShown;
        UIEvents.HideGroupWateringView -= HideOverlay;
        _groupWateringView.Dispose();
        UIEvents.ShowOnboardingView -= OnOnboardingViewShown;
        UIEvents.HideOnboardingView -= HideOverlay;
        _onboardingView.Dispose();

        _actions.UI.Disable();
        _actions.UI.Cancel.performed -= OnCancelPerformed;
        UIEvents.AddNotification -= _notificationManager.AddNotification;
    }

    private void ShowOverlay(UIView view)
    {
        if (_overlayViews.Contains(view))
            return;
        _overlayViews.Push(view);
        view.BringToFront();
        view.Show();
        UpdateHighlightController();
    }
    private void HideOverlay()
    {
        if (_overlayViews.TryPop(out var overlayView))
        {
            overlayView.Hide();
        }
        UpdateHighlightController();
    }
    private void ShowView(UIView view, int? index = null)
    {
        while (_overlayViews.TryPop(out var overlayView))
        {
            overlayView.Hide();
        }
        if (view == _currentView)
            return;


        HideCurrentView();
        _previousView = _currentView;
        _currentView = view;
        UpdateHighlightController();

        if (index.HasValue)
            (view as DetailView).Show(index.Value);
        else
            view.Show();
    }

    private void UpdateHighlightController()
    {
        if (_highlightController == null)
            return;
        bool isHudView = _currentView == _hudView && _overlayViews.Count == 0;
        _highlightController.enabled = isHudView;
    }

    private void ShowPreviousView()
    {
        while (_overlayViews.TryPop(out var overlayView))
        {
            overlayView.Hide();
        }
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