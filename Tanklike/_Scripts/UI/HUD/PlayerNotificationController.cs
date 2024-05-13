using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Notifications
{
    public class PlayerNotificationController : MonoBehaviour
    {
        [SerializeField] private List<CollectionNotificationBar> _bars;
        private const float COLLECTION_DISPLAY_DURATION = 3f + CollectionNotificationBar.FADE_OUT_DURATION;

        public void SetUp()
        {
            _bars.ForEach(bb => bb.IsAvailable = true);
            _bars.ForEach(bb => bb.gameObject.SetActive(false));
        }

        public void PushCollectionNotification(NotificationType notificationType, int amount, NotificationBarSettings_SO settings)
        {
            CollectionNotificationBar selectedBar;

            // if there is a bar that holds the selected type of notification and it's not available i.e. is already displayed on the HUD
            if (_bars.Exists(b => b.Type == notificationType && b.CanAddTo))
            {
                selectedBar = _bars.Find(b => b.Type == notificationType && b.CanAddTo);
            }
            // otherwise, just get the next available bar
            else
            {
                selectedBar = _bars.Find(b => b.IsAvailable);
            }

            // fill the information
            selectedBar.FillInfo(settings.Name, settings.Icon, notificationType, amount);
            selectedBar.StartCountDown(COLLECTION_DISPLAY_DURATION);
            selectedBar.Enable();
        }
    }
}
