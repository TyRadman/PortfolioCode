using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    public class GemCollectable : Collectable
    {
        [SerializeField] private int _amount = 1;

        public override void OnCollected(PlayerComponents player)
        {
            //GameManager.Instance.PlayersManager.AddGems(_amount);
            base.OnCollected(player);
        }
    }
}
