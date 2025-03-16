using SG420UILibrary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class NotificationManager : MonoBehaviour
{
    private VisualElement _root;
    private Queue<NotificationData> _notificationDataQueue;
    private Queue<Notification> _availableNotifications;
    private List<Notification> _shownNotifications;

    private Dictionary<int, Translate> _notificationPositionToTranslate = new Dictionary<int, Translate>{
        {0, new Translate(0,0)},
        {1, new Translate(0,-80)},
        {2, new Translate(0,-160)},
    };
    private Translate _defaultTranslate = new Translate(0, 80);
    private StyleList<TimeValue> _defaultTimeValue;
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _root = FindAnyObjectByType<UIDocument>().rootVisualElement;

        _notificationDataQueue = new Queue<NotificationData>();

        _availableNotifications = new Queue<Notification>();
        foreach (var notification in new Notification[] { new Notification(), new Notification(), new Notification() })
        {
            _availableNotifications.Enqueue(notification);
            _root.Add(notification);
            notification.CloseButton.clicked += Close(notification);
            notification.style.translate = _defaultTranslate;
            if (_defaultTimeValue == null)
            {
                _defaultTimeValue = notification.style.transitionDuration;
                notification.RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            }
            notification.style.transitionDuration = StyleKeyword.Initial;
            notification.style.display = DisplayStyle.None;
            notification.RegisterCallback<MouseDownEvent>((_) =>
            {
                if (notification.OnClick == null)
                    return;
                notification.OnClick.Invoke();
                Close(notification).Invoke();
            });
        }

        _shownNotifications = new List<Notification>();
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        _defaultTranslate = new Translate(0, evt.newRect.height);
        _notificationPositionToTranslate = new Dictionary<int, Translate>{
        {0, new Translate(0,0)},
        {1, new Translate(0,-evt.newRect.height)},
        {2, new Translate(0,-2 * evt.newRect.height)},
    };
    }

    private Action Close(Notification notification)
    {
        return () =>
        {
            if (!_shownNotifications.Contains(notification))
            {
                return;
            }
            _shownNotifications.Remove(notification);
            _availableNotifications.Enqueue(notification);
            notification.style.display = DisplayStyle.None;
            notification.style.translate = _defaultTranslate;
            notification.style.transitionDuration = StyleKeyword.Initial;
        };
    }



    // Update is called once per frame
    void Update()
    {
        if (_notificationDataQueue == null || _availableNotifications == null) return;
        while (_notificationDataQueue.Count > 0 && _availableNotifications.Count > 0)
        {
            var data = _notificationDataQueue.Dequeue();
            var notification = _availableNotifications.Dequeue();
            notification.UpdateNotification(data);
            notification.style.display = DisplayStyle.Flex;
            notification.style.transitionDuration = _defaultTimeValue;
            _shownNotifications.Insert(0, notification);
        }
        List<Notification> notificationsToClose = new List<Notification>();
        for (int i = 0; i < _shownNotifications.Count; i++)
        {
            var notification = _shownNotifications[i];
            notification.BringToFront();
            notification.style.translate = _notificationPositionToTranslate[i];
            if (notification.TimeUntilNotificationCloses > 0)
            {
                notification.TimeUntilNotificationCloses -= Time.deltaTime;
                if (notification.TimeUntilNotificationCloses <= 0)
                {
                    notificationsToClose.Add(notification);
                }
            }

        }
        foreach (var notification in notificationsToClose)
        {
            Close(notification).Invoke();
        }
    }

    public void HideAndDisable()
    {
        gameObject.SetActive(false);
    }

    public void ShowAndEnable()
    {
        gameObject.SetActive(true);
    }

    public void AddNotification(NotificationData notificationData)
    {
        _notificationDataQueue.Enqueue(notificationData);
    }
}


public class NotificationData
{
    public string Title;
    public string Message;
    public float TimeToShowNotification;
    public Action OnClick;

    public NotificationData(string title, string message, float timeToShowNotification = 3, Action onClick = null)
    {
        Title = title;
        Message = message;
        TimeToShowNotification = timeToShowNotification;
        OnClick = onClick;
    }
}
