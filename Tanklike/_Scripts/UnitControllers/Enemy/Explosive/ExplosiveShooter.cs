using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class ExplosiveShooter : EnemyShooter
    {
        public void Explode()
        {
            _currentWeapon.OnShot();
        }

        public void SetRadius(float radius)
        {
            ((AOEWeapon)_currentWeapon).SetExplosionRadius(radius);
        }
    }
}
