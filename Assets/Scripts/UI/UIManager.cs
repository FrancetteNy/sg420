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

        UIEvents.ShowDetailView += ShowDetailView;
        UIEvents.HideDetailView += () => ResetToHUD();

        UIEvents.ShowHUDView += () => ShowView(_hudView);
        UIEvents.HideHUDView += _hudView.Hide;

        UIEvents.ShowLightOverview += () => ShowView(_lightOverview);
        UIEvents.HideLightOverview += () => ResetToHUD();

        UIEvents.ShowEncyclopedia += () => ShowView(_encyclopedia);
        UIEvents.HideEncyclopedia += () => ResetToHUD();

        UIEvents.ShowChatView += () => ShowView(_chatView);
        UIEvents.HideChatView += () => ResetToHUD();


        _allUIViews.Add(_detailView);
        _allUIViews.Add(_hudView);
        _allUIViews.Add(_lightOverview);
        _allUIViews.Add(_encyclopedia);
        _allUIViews.Add(_chatView);

        _currentView = _hudView;
    }



    private void OnDestroy()
    {
        UIEvents.ShowDetailView -= ShowDetailView;
        UIEvents.HideDetailView -= () => ResetToHUD();
        _detailView.Dispose();
        UIEvents.ShowHUDView -= () => ShowView(_hudView);
        UIEvents.HideHUDView -= _hudView.Hide;
        _hudView.Dispose();
        UIEvents.ShowLightOverview -= () => ShowView(_lightOverview);
        UIEvents.HideLightOverview -= () => ResetToHUD();
        _lightOverview.Dispose();
        UIEvents.ShowEncyclopedia -= () => ShowView(_encyclopedia);
        UIEvents.HideEncyclopedia -= () => ResetToHUD();
        _encyclopedia.Dispose();
        UIEvents.ShowChatView -= () => ShowView(_chatView);
        UIEvents.HideChatView -= () => ResetToHUD();
        _chatView.Dispose();
    }
    private void ShowDetailView(int index)
    {
        HideCurrentView();
        _currentView = _detailView;
        _detailView.Show(index);
    }
    private void ResetToHUD()
    {
        HideCurrentView();
        _hudView.Show();
        _currentView = _hudView;
    }


    private void ShowView(UIView view)
    {
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