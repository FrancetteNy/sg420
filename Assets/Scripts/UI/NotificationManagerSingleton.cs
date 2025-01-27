using SG420UILibrary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// by @kurtdekker - to make a Unity singleton that has some
// https://gist.github.com/kurtdekker/2f07be6f6a844cf82110fc42a774a625
// prefab-stored, data associated with it, eg a music manager
//
// To use: access with SingletonViaPrefab.Instance
//
// To set up:
//	- Copy this file (duplicate it)
//	- rename class SingletonViaPrefab to your own classname
//	- rename CS file too
//	- create the prefab asset associated with this singleton
//		NOTE: read docs on Resources.Load() for where it must exist!!
//
// DO NOT DRAG THE PREFAB INTO A SCENE! THIS CODE AUTO-INSTANTIATES IT!
//
// I do not recommend subclassing unless you really know what you're doing.

public class NotificationManagerSingleton : MonoBehaviour
{
    private VisualElement _notification_space;
    private VisualElement _root;
    private static NotificationManagerSingleton _instance;
    private Queue<NotificationData> _notificationDataQueue;
    private Queue<Notification> _availableNotifications;
    private List<Notification> _shownNotifications;

    private static Dictionary<int, Translate> _notificationPositionToTranslate = new Dictionary<int, Translate>{
        {0, new Translate(0,0)},
        {1, new Translate(0,-80)},
        {2, new Translate(0,-160)},
    };
    private static Translate _defaultTranslate = new Translate(0, 80);
    private static StyleList<TimeValue> _defaultTimeValue;

    public static NotificationManagerSingleton Instance
    {
        get
        {
            if (!_instance)
            {
                // NOTE: read docs to see directory requirements for Resources.Load!
                var prefab = Resources.Load<GameObject>("NotificationManagerSingleton");
                // create the prefab in your scene
                var inScene = Instantiate<GameObject>(prefab);
                // try find the instance inside the prefab
                _instance = inScene.GetComponentInChildren<NotificationManagerSingleton>();
                // guess there isn't one, add one
                if (!_instance)
                    _instance = inScene.AddComponent<NotificationManagerSingleton>();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_instance.transform.root.gameObject);
                _instance.Initialize();
            }
            return _instance;
        }
    }

    private void Initialize()
    {
        _root = FindAnyObjectByType<UIDocument>().rootVisualElement;

        _notificationDataQueue = new Queue<NotificationData>();

        _availableNotifications = new Queue<Notification>();
        foreach (var notification in new Notification[]{ new Notification(), new Notification(), new Notification() })
        {
            _availableNotifications.Enqueue(notification);
            _root.Add(notification);
            notification.CloseButton.clicked += Close(notification);
            notification.style.translate = _defaultTranslate;
            if (_defaultTimeValue == null)
            {
                _defaultTimeValue = notification.style.transitionDuration;
                notification.RegisterCallbackOnce < GeometryChangedEvent >(OnGeometryChanged);
            }
            notification.style.transitionDuration = StyleKeyword.Initial;
            notification.style.display = DisplayStyle.None;
            notification.RegisterCallback<MouseDownEvent>((_) => notification.OnClick?.Invoke());
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
        return () => { 
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
        for ( int i = 0; i <_shownNotifications.Count; i++)
        {
            var notification = _shownNotifications[i];
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
        _instance.gameObject.SetActive(false);
    }

    public void ShowAndEnable()
    {
        _instance.gameObject.SetActive(true);
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

    public NotificationData(string title, string message, float timeToShowNotification = -1, Action onClick = null)
    {
        Title = title;
        Message = message;
        TimeToShowNotification = timeToShowNotification;
        OnClick = onClick;
    }
}