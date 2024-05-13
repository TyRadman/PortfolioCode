using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    public class BossRoomKey_Collectable : Collectable
    {
        public override void OnCollected(PlayerComponents player)
        {
            base.OnCollected(player);
            // increment the count
            GameManager.Instance.BossKeysManager.OnKeyCollected();
            GameManager.Instance.NotificationsManager.PushCollectionNotification(UI.Notifications.NotificationType.BossKey, 1, player.PlayerIndex);
        }

        public override void DisableCollectable()
        {
            base.DisableCollectable();

            gameObject.SetActive(false);
        }
    }
}
