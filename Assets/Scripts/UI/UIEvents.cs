using System;
using UnityEngine.UIElements;

class UIEvents
{
    //UIView Events
    public static Action<int> ShowDetailView;
    public static Action HideDetailView;
    public static Action ShowHUDView;
    public static Action HideHUDView;
    public static Action ShowLightOverview;
    public static Action HideLightOverview;
    public static Action ShowEncyclopedia;
    public static Action HideEncyclopedia;

    //Notification Events
    public static Action<NotificationData> AddNotification;

}
