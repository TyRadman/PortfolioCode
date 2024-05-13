using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    public class AmmoCollectable : Collectable
    {
        public override void OnCollected(PlayerComponents player)
        {
            player.Overheat.FillBars();

            base.OnCollected(player);
        }
    }
}
