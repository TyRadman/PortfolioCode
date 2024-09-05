using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerTurretController : TankTurretController
    {
        public override void HandleTurretRotation(Transform target)
        {
            Vector3 direction = (target.position - _turret.position).normalized;
            _turret.rotation = Quaternion.LookRotation(direction);
        }
    }
}
