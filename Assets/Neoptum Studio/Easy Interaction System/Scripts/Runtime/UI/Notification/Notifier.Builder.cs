using UnityEngine;

namespace EIS.Runtime.UI.Notification
{
    /// <summary>
    /// Builder class used to create and configure notification data in a fluent manner.
    /// </summary>
    public partial class Notifier
    {
        public class Builder
        {
            private NotificationData notificationData;

            Builder()
            {
                notificationData = new NotificationData();
            }

            public static Builder CreateNew()
            {
                return new Builder();
            }

            public Builder SetIcon(Sprite icon)
            {
                notificationData.icon = icon;
                return this;
            }

            public Builder SetIcon(string iconIndex)
            {
                notificationData.icon = Notifier.Instance.GetIcon(iconIndex);
                return this;
            }

            public Builder SetText(string text)
            {
                notificationData.text = text;
                return this;
            }


            public Builder SetNotificationType(NotificationType notificationType)
            {
                notificationData.notificationType = notificationType;
                return this;
            }

            public GameObject ShowNotification(float duration = 10f)
            {
                return Notifier.Instance.ShowNotification(notificationData, duration);
            }
        }
    }
}