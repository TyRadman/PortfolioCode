using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "OnImpact_Piercing", menuName = MENU_MAIN + "Piercing Impact")]
    public class PiercingImpact : OnImpact
    {
        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            // if there is a target, then apply damage to it, play the impact effects, and continue
            if (target != null)
            {
                target.TakeDamage(damage, Vector3.zero, bullet.GetInstigator(), bullet.transform.position);
                // play the effect 
                bullet.PlayImpactEffects();
            }
            // otherwise, it must be a wall, therefore, end the bullet
            else
            {
                bullet.DisableBullet();
            }
        }
    }
}
