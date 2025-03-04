using System;

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

    //Notification Events
    public static Action<NotificationData> AddNotification;

}
