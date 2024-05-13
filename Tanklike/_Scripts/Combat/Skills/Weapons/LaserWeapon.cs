using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_Laser_NAME", menuName = "Skills/Weapons/Laser")]
    public class LaserWeapon : Weapon
    {
        [Header("Laser Settings")]
        [SerializeField] private Laser _laserPrefab;
        [SerializeField] private float _duration;
        [field: SerializeField] public float MaxLength;
        [field: SerializeField] public float Thickness;
        [field: SerializeField] public float DamageInterval = 0.1f;

        private Laser _laser;

        public override void OnShot(TankComponents shooter, Transform shootingPoint = null, float angle = 0)
        {
            base.OnShot(shooter);

            Laser laser = GameManager.Instance.VisualEffectsManager.Lasers.GetLaser(BulletData.GUID);
            laser.SetValues(MaxLength, Thickness, Damage, DamageInterval, _duration);
            shooter.Shooter.ShootLaser(laser, shootingPoint, angle);
        }

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            // create the pools for the bullet

        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public float GetDuration()
        {
            return _duration;
        }
    }
}
