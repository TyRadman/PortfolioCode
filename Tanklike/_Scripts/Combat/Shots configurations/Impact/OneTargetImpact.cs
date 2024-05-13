using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "One Target Impact", menuName = "Shot Configurations/Impact/One Target Impact")]
    public class OneTargetImpact : OnImpact
    {
        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            // whatever definition the target has for this method
            if (target != null)
            {
                target.TakeDamage(damage, Vector3.zero, bullet.GetShooter(), bullet.transform.position); //not sure if we need the direction yet
            }

            // play the effect 
            bullet.DisableBullet();
        }
    }
}
