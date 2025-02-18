using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    VisualElement _root;
    List<UIView> _allUIViews = new();

    DetailView _detailView;
    HUDView _hudView;
    LightOverview _lightOverview;
    Encyclopedia _encyclopedia;
    Inventar _inventar;
    ChatView _chatView;

    NotificationManagerSingleton _notificationManager;

    UIView _currentView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        AddAllUIViews();
        SetupNotificationMananger();
    }

    private void SetupNotificationMananger()
    {
        _notificationManager = NotificationManagerSingleton.Instance;
        UIEvents.AddNotification += _notificationManager.AddNotification;
    }

    private void AddAllUIViews()
    {
        _chatView = new ChatView(_root, this);
        _detailView = new DetailView(_root, this);
        _hudView = new HUDView(_root, this);
        _lightOverview = new LightOverview(_root, this);
        _encyclopedia = new Encyclopedia(_root, this);
        _inventar = new Inventar(_root, this);

        UIEvents.ShowDetailView += ShowDetailView;
        UIEvents.HideDetailView += () => ShowView(_hudView);

        UIEvents.ShowHUDView += () => ShowView(_hudView);
        UIEvents.HideHUDView += _hudView.Hide;

        UIEvents.ShowLightOverview += () => ShowView(_lightOverview);
        UIEvents.HideLightOverview += () => ShowView(_hudView);

        UIEvents.ShowEncyclopedia += () => ShowView(_encyclopedia);
        UIEvents.HideEncyclopedia += () => ShowView(_encyclopedia);
        
        UIEvents.ShowInventar += () => ShowView(_inventar);
        UIEvents.HideInventar += () => ShowView(_inventar);
        UIEvents.HideEncyclopedia += () => ShowView(_hudView);

        UIEvents.ShowChatView += () => ShowView(_chatView);
        UIEvents.HideChatView += () => ShowView(_hudView);


        _allUIViews.Add(_detailView);
        _allUIViews.Add(_hudView);
        _allUIViews.Add(_lightOverview);
        _allUIViews.Add(_encyclopedia);
        _allUIViews.Add(_inventar);
        _allUIViews.Add(_chatView);

        _currentView = _hudView;
    }



    private void OnDestroy()
    {
        UIEvents.ShowDetailView -= ShowDetailView;
        UIEvents.HideDetailView -= () => ShowView(_hudView);
        _detailView.Dispose();
        UIEvents.ShowHUDView -= () => ShowView(_hudView);
        UIEvents.HideHUDView -= _hudView.Hide;
        _hudView.Dispose();
        UIEvents.ShowLightOverview -= () => ShowView(_lightOverview);
        UIEvents.HideLightOverview -= () => ShowView(_hudView);
        _lightOverview.Dispose();
        UIEvents.ShowEncyclopedia -= () => ShowView(_encyclopedia);
        UIEvents.HideEncyclopedia -= () => ShowView(_hudView);
        _encyclopedia.Dispose();
        UIEvents.ShowInventar -= () => ShowView(_inventar);
        UIEvents.HideInventar -= () => ShowView(_inventar);
        _inventar.Dispose();
        UIEvents.ShowChatView -= () => ShowView(_chatView);
        UIEvents.HideChatView -= () => ShowView(_hudView);
        _chatView.Dispose();
    }
    private void ShowDetailView(int index)
    {
        if (_detailView == _currentView)
            return;
        HideCurrentView();
        _currentView = _detailView;
        _detailView.Show(index);
    }


    private void ShowView(UIView view)
    {
        if (view == _currentView)
            return;
        HideCurrentView();
        _currentView = view;
        view.Show();
    }

    private void HideCurrentView()
    {
        if (_currentView != null)
        { 
            _currentView.Hide();
        }
    }

}