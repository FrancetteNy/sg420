using System;
using UnityEngine.UIElements;
namespace SG420UILibrary
{
    [UxmlElement]
    public partial class Notification : VisualElement
    {

        public static readonly string UssClassName = "notification";
        public static readonly string UssTopRowClassName = "notification__top-row";
        public static readonly string UssTitleLabelClassName = "notification__title-label";
        public static readonly string UssBodyLabelClassName = "notification__body-label";
        public static readonly string UssCloseButtonClassName = "notification__close-button";
        Label _titleLabel;
        Label _bodyElement;
        public Button CloseButton;
        public float TimeUntilNotificationCloses;

        public Action OnClick;

        public Notification():this(new("-","-")) { }

        public Notification(NotificationData notificationData)
        {
            var topRow = new VisualElement();
            topRow.AddToClassList(UssTopRowClassName);
            Add(topRow);
            _titleLabel = new Label();
            _titleLabel.AddToClassList(UssTitleLabelClassName);
            topRow.Add(_titleLabel);

            CloseButton = new Button();
            CloseButton.text = "X";
            CloseButton.AddToClassList(UssCloseButtonClassName);
            topRow.Add(CloseButton);
            CloseButton.clicked += OnClick;

            _bodyElement = new Label();
            _bodyElement.AddToClassList(UssBodyLabelClassName);
            Add(_bodyElement);
            UpdateNotification(notificationData);
            AddToClassList(UssClassName);
        }

        public void UpdateNotification(NotificationData notificationData) {
            _titleLabel.text = notificationData.Title;
            _bodyElement.text = notificationData.Message;
            TimeUntilNotificationCloses = notificationData.TimeToShowNotification;
            OnClick = notificationData.OnClick;
        }
    }

}