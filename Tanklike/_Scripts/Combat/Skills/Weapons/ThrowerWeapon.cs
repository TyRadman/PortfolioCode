using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_Thrower_Default", menuName = "TankLike/Skills/Weapons/Thrower")]
    public class ThrowerWeapon : Weapon
    {
        [field: SerializeField, Header("Shot Settings")] public AmmunationData BulletData { get; private set; }
        [field: SerializeField] public float BulletSpeed { get; private set; }
        [field: SerializeField] public float MaxDistance = 100f;

        //public override void OnShot(TankComponents shooter = null, Transform shootingPoint = null, float angle = 0)
        //{
        //    base.OnShot(shooter);

        //    Rocket rocket = GameManager.Instance.VisualEffectsManager.Rockets.Rocket_01;
        //    rocket.SetValues(Damage);
        //    shooter.Shooter.ShootLaser(laser, _duration, shootingPoint, angle);
        //    rocket.SetUpBulletdata(BulletData);
        //    rocket.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);
        //    rocket.gameObject.SetActive(true);
        //    rocket.Add(bullet);
        //    Vector2 randomCircle = _radius * 0.5f * Random.insideUnitCircle;
        //    Vector3 point = _indicator.position + new Vector3(randomCircle.x, 0f, randomCircle.y);// - _verticalDistance * 1.1f * Vector3.up; ;
        //                                                                                          // enable the bullet's collider, mesh, and trail
        //    bullet.EnableBullet();
        //    bullet.SetActive(true);

        //    Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
        //    bullet.SetValues(BulletSpeed, Damage, MaxDistance);
        //    ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
        //    shooter.Shooter.SpawnBullet(bullet, shootingPoint, angle);
        //    shooter.Shooter.ShowShootingEffects(muzzle, shootingPoint);
        //}

        //public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        //{
        //    base.OnShot(spawnBulletAction, shootingPoint, angle);

        //    Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
        //    bullet.SetUpBulletdata(BulletData);
        //    bullet.SetTargetLayerMask(_targetLayerMask);
        //    bullet.SetValues(BulletSpeed, Damage, MaxDistance);
        //    ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
        //    spawnBulletAction(bullet, shootingPoint, angle);
        //}

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
