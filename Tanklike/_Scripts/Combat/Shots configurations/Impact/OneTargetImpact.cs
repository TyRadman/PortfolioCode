using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "OnImpact_SingleTarget", menuName = MENU_MAIN + "Single Target")]
    public class OneTargetImpact : OnImpact
    {
        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            // whatever definition the target has for this method
            if (target != null)
            {
                Transform bulletTransform = bullet.transform;
                target.TakeDamage(damage, bulletTransform.forward, bullet.GetInstigator(), bulletTransform.position, bullet); //not sure if we need the direction yet
            }

            // play the effect 
            bullet.DisableBullet();
        }
    }
}
