using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Misc;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_Shot_Default", menuName = "TankLike/Skills/Weapons/Normal Shot")]
    public class NormalShot : Weapon
    {
        [field: SerializeField] public float BulletSpeed { get; private set; }
        [field: SerializeField] public float MaxDistance = 100f;

        public override void OnShot(TankComponents shooter = null, Transform shootingPoint = null, float angle = 0)
        {
            base.OnShot(shooter);

            Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
            bullet.SetUpBulletdata(BulletData);
            bullet.SetValues(BulletSpeed, Damage, MaxDistance);
            ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
            shooter.Shooter.SpawnBullet(bullet, shootingPoint, angle);
            shooter.Shooter.ShowShootingEffects(muzzle, shootingPoint);
        }

        public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            base.OnShot(spawnBulletAction, shootingPoint, angle);

            Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
            bullet.SetUpBulletdata(BulletData);
            bullet.SetTargetLayerMask(_targetLayerMask);
            bullet.SetValues(BulletSpeed, Damage, MaxDistance);
            ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
            spawnBulletAction(bullet, shootingPoint, angle);
        }

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            // create the pools for the bullet

        }

        public override void SetSpeed(float speed)
        {
            BulletSpeed = speed;
        }
    }
}
