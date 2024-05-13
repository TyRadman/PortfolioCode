using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Notifications
{
    [CreateAssetMenu(fileName = "Notification Settings", menuName = "UI/ Notification Settings")]
    public class NotificationBarSettings_SO : ScriptableObject
    {
        public NotificationType Type;
        public Sprite Icon;
        public string Name;
    }
}
