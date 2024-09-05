using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    // TODO: this class needs to be removed
    public class GemCollectable : Collectable
    {

        public override void OnCollected(PlayerComponents player)
        {
            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, 1, player.PlayerIndex);

            base.OnCollected(player);
        }
    }
}
