using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class HealthPointsHolder : Collectable
    {
        public override void OnTriggerAction(PlayerComponents _player)
        {
            var stats = _player.GetComponent<PlayerStats>();

            if (stats != null)
            {
                _player.GetComponent<PlayerHealth>().TakeDamage(CollectablePoints);
            }
            else
            {
                _player.transform.parent.GetComponent<PlayerHealth>().TakeDamage(CollectablePoints);
            }

            PointsPopUp.i.CallText(transform.position, PointsPopUp.PopUpTextType.Life, CollectablePoints);

            base.OnTriggerAction(_player);
        }
    }
}
