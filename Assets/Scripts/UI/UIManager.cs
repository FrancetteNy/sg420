using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    ModalView _modalView;

    NotificationManager _notificationManager;

    private UIView _currentView;
    public UIView CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                _currentView = value;
                GameState.UpdateHUD?.Invoke();
            }
        }
    }
    UIView _previousView;

    ObservableStack<UIView> _overlayViews;
    InputSystem_Actions _actions;

    HighlightController _highlightController;


    GameObject _mainRoom;
    GameObject _dryingRoom;
    public bool IsReadyToChangeRoom => _currentView == _hudView && _overlayViews.Count == 0;
    public bool MainRoomIsActive => _mainRoom?.activeSelf ?? false;
    public bool DryingRoomIsActive => _dryingRoom?.activeSelf ?? false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _overlayViews = new ObservableStack<UIView>();
        _overlayViews.CollectionChanged += OnOverlayViewsChanged;
        AddAllUIViews();
        _notificationManager = gameObject.AddComponent<NotificationManager>();
        SetupNotificationMananger();
        SetupActionSystem();
        _highlightController = FindAnyObjectByType<HighlightController>();
        SetupRoomGameObjects();
        GameState.UpdateHUD?.Invoke();
    }

    private void OnOverlayViewsChanged()
    {
        GameState.UpdateHUD?.Invoke();
    }

    private void SetupRoomGameObjects()
    {
        _mainRoom = GameObject.Find("MainRoom");
        _dryingRoom = GameObject.Find("DryingRoom");
        _dryingRoom.SetActive(false);
    }

    private void SetupActionSystem()
    {
        _actions = new InputSystem_Actions();
        _actions.UI.Enable();
        _actions.UI.Cancel.performed += OnCancelPerformed;
        _actions.Player.MoveToDryingRoom.Enable();
        _actions.Player.MoveToMainRoom.Enable();
        _actions.Player.MoveToDryingRoom.performed += OnMoveToDryingRoom;
        _actions.Player.MoveToMainRoom.performed += OnMoveToMainRoom;
    }
    public void ChangeToMainRoom()
    {
        if (!IsReadyToChangeRoom)
            return;
        _dryingRoom.SetActive(false);
        _mainRoom.SetActive(true);
        RenderSettings.ambientIntensity = 1;
        GameState.UpdateHUD?.Invoke();
    }
    public void ChangeToDryingRoom()
    {
        if (!IsReadyToChangeRoom)
            return;
        _mainRoom.SetActive(false);
        _dryingRoom.SetActive(true);
        RenderSettings.ambientIntensity = .3f;
        GameState.UpdateHUD?.Invoke();
    }
    private void OnMoveToMainRoom(InputAction.CallbackContext context) => ChangeToMainRoom();

    private void OnMoveToDryingRoom(InputAction.CallbackContext context)=> ChangeToDryingRoom();

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!GameStateManagerSingleton.Instance.IsGameLoaded)
            return;
        if(_overlayViews.Count > 0)
        {
            var view = _overlayViews.Peek();
            view.OnCancelPerformed(context);
        }
        else if (CurrentView != null) 
        {
            CurrentView?.OnCancelPerformed(context);
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
        _modalView = new ModalView(_root, this);

        UIEvents.ShowDetailView += OnDetailViewShown;
        UIEvents.HideDetailView += OnHudShown;

        UIEvents.ShowHUDView += OnHudShown;
        UIEvents.HideHUDView += _hudView.Hide;

        UIEvents.ShowLightOverview += OnLightOverviewShown;
        UIEvents.HideLightOverview += HideOverlay;

        UIEvents.ShowInventar += OnInventarShown;
        UIEvents.HideInventar += HideOverlay;

        UIEvents.ShowShop += OnShopShown;
        UIEvents.HideShop += HideOverlay;
        
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

        UIEvents.ShowModalView += OnModalViewShown;
        UIEvents.HideModalView += HideOverlay;

        _previousView = null;
        if (GameStateManagerSingleton.Instance.IsGameLoaded)
        {
            CurrentView = _mainMenuView;
            UIEvents.ShowHUDView.Invoke();
        }
        else
        {
            CurrentView = _hudView;
            UIEvents.ShowMainMenuView.Invoke();
        }
    }



    private void OnDetailViewShown(int index) => ShowView(_detailView, index);
    private void OnHudShown() => ShowView(_hudView);
    private void OnLightOverviewShown() => ShowOverlay(_lightOverview);
    private void OnEncyclopediaShown() => ShowOverlay(_encyclopedia);
    private void OnChatViewShown() => ShowView(_chatView);
    private void OnMainMenuViewShown() => ShowView(_mainMenuView);
    private void OnInventarShown () => ShowOverlay(_inventar);
    private void OnShopShown () => ShowOverlay(_shop);
    private void OnQuestLogShown() => ShowOverlay(_questLog);
    private void OnGroupWateringShown() => ShowOverlay(_groupWateringView);
    private void OnOnboardingViewShown(List<OnboardingData> list) {
        _onboardingView.SetData(list);
        ShowOverlay(_onboardingView);
    }
    private void OnModalViewShown(string title, string description, UnityAction action)
    {
        if (_overlayViews.Contains(_modalView))
            return;
        _overlayViews.Push(_modalView);
        _modalView.BringToFront();
        _modalView.Show(title, description, action);
        UpdateHighlightController();
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
        UIEvents.HideInventar -= HideOverlay;
        _inventar.Dispose();
        UIEvents.ShowShop -= OnShopShown;
        UIEvents.HideShop -= HideOverlay;
        _shop.Dispose();
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
        UIEvents.ShowModalView -= OnModalViewShown;
        UIEvents.HideModalView -= HideOverlay;
        _modalView.Dispose();

        _actions.UI.Disable();
        _actions.UI.Cancel.performed -= OnCancelPerformed;

        _actions.Player.MoveToDryingRoom.Disable();
        _actions.Player.MoveToMainRoom.Disable();
        _actions.Player.MoveToDryingRoom.performed -= OnMoveToDryingRoom;
        _actions.Player.MoveToMainRoom.performed -= OnMoveToMainRoom;

        UIEvents.AddNotification -= _notificationManager.AddNotification;

        _overlayViews.CollectionChanged -= OnOverlayViewsChanged;
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
        if (view == CurrentView)
            return;


        HideCurrentView();
        _previousView = CurrentView;
        CurrentView = view;
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
        bool isHudView = CurrentView == _hudView && _overlayViews.Count == 0;
        _highlightController.enabled = isHudView;
    }

    private void ShowPreviousView()
    {
        while (_overlayViews.TryPop(out var overlayView))
        {
            overlayView.Hide();
        }
        if (CurrentView == _previousView || _previousView is null || CurrentView is null)
            return;
        ShowView(_previousView);
    }

    private void HideCurrentView()
    {
        if (CurrentView != null)
        {
            CurrentView.Hide();
        }
    }

}


public class ObservableStack<T>
{
    public event Action CollectionChanged;
    private Stack<T> _stack;
    public ObservableStack()
    {
        _stack = new Stack<T>();
    }
    public void Push(T item)
    {
        _stack.Push(item);
        this.CollectionChanged();
    }

    public bool TryPop(out T item)
    {
        if (_stack.TryPop(out item))
        {
            this.CollectionChanged();
            return true;
        }
        else
        {
            item = default;
            return false;
        }
    }

    public T Peek() => _stack.Peek();

    public int Count => _stack.Count;

    public bool Contains(T item) => _stack.Contains(item);
}