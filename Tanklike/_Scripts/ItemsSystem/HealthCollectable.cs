using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.ItemsSystem
{
    public class HealthCollectable : Collectable
    {
        [field: SerializeField] public int HealthAmount = 10;

        public override void OnCollected(PlayerComponents player)
        {
            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, HealthAmount, player.PlayerIndex);
            player.Energy.AddEnergy(HealthAmount);
            base.OnCollected(player);
        }
    }
}
