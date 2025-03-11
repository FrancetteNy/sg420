using System.Collections.Generic;
using System;
using UnityEngine.Events;

class UIEvents
{
    //UIView Events
    public static Action ShowPreviousView;

    public static Action<int> ShowDetailView;
    public static Action HideDetailView;

    public static Action ShowHUDView;
    public static Action HideHUDView;

    public static Action ShowLightOverview;
    public static Action HideLightOverview;

    public static Action ShowEncyclopedia;
    public static Action HideEncyclopedia;

    public static Action ShowChatView;
    public static Action HideChatView;

    public static Action ShowMainMenuView;
    public static Action HideMainMenuView;

    public static Action ShowQuestLog;
    public static Action HideQuestLog;

    public static Action ShowGroupWateringView;
    public static Action HideGroupWateringView;

    public static Action<List<OnboardingData>> ShowOnboardingView;
    public static Action HideOnboardingView;

    public static Action<string, string, UnityAction> ShowModalView;
    public static Action HideModalView;

    //Notification Events
    public static Action<NotificationData> AddNotification;

}
