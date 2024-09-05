using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Misc;
using TankLike.Utils;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_Shot_Default", menuName = DIRECTORY + "Normal Shot")]
    public class NormalShot : Weapon
    {
        [field: SerializeField] public float BulletSpeed { get; private set; }
        [field: SerializeField] public float MaxDistance = 100f;
        [field: SerializeField] public int ShotsNumber = 1;
        [field: SerializeField] public float AngleBetweenShots = 0f;

        public override void OnShot(Transform shootingPoint = null, float angle = 0)
        {
            base.OnShot();
            float angleIncement = AngleBetweenShots / (ShotsNumber - 1);
            float currentAngle = angle;

            if (ShotsNumber > 1)
            {
                currentAngle = _components.transform.eulerAngles.y - AngleBetweenShots / 2;
            }

            for (int i = 0; i < ShotsNumber; i++)
            {
                Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
                bullet.SetUpBulletdata(BulletData);
                bullet.SetValues(BulletSpeed, Damage, MaxDistance);
                bullet.SetTargetLayerMask(_components.Tag.ToString());
                ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                SpawnBullet(bullet, shootingPoint, currentAngle);
                ShowShootingEffects(muzzle, shootingPoint);

                currentAngle += angleIncement;
            }
        }

        public override void OnShot(Vector3 spawnPoint, Vector3 rotation, float angle = 0)
        {
            base.OnShot(spawnPoint, rotation, angle);

            float angleIncement = AngleBetweenShots / (ShotsNumber - 1);
            float currentAngle = angle;

            if (ShotsNumber > 1)
            {
                currentAngle = _components.transform.eulerAngles.y - AngleBetweenShots / 2;
            }

            for (int i = 0; i < ShotsNumber; i++)
            {
                Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
                bullet.SetUpBulletdata(BulletData);
                bullet.SetValues(BulletSpeed, Damage, MaxDistance);
                bullet.SetTargetLayerMask(_components.Tag.ToString());
                ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                SpawnBullet(bullet, spawnPoint, rotation, currentAngle);
                ShowShootingEffects(muzzle, spawnPoint, rotation);

                currentAngle += angleIncement;
            }
        }

        public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            base.OnShot(spawnBulletAction, shootingPoint, angle);
            float angleIncement = AngleBetweenShots / (ShotsNumber - 1);
            float currentAngle = angle;

            if (ShotsNumber > 1)
            {
                currentAngle = shootingPoint.transform.eulerAngles.y - AngleBetweenShots / 2;
            }

            for (int i = 0; i < ShotsNumber; i++)
            {
                Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
                bullet.SetUpBulletdata(BulletData);
                bullet.SetTargetLayerMask(_targetLayerMask);
                bullet.SetValues(BulletSpeed, Damage, MaxDistance);
                ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                spawnBulletAction(bullet, shootingPoint, currentAngle);

                currentAngle += angleIncement;
            }
        }

        #region SpawnBullet Overloads
        public void SpawnBullet(Bullet bullet, Transform shootingPoint = null, float angle = 0)
        {
            // create the bullet
            bullet.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (shootingPoint != null)
            {
                Vector3 eulerRotation = shootingPoint.eulerAngles;
                eulerRotation.x = 0;
                eulerRotation.z = 0;
                rotation = Quaternion.Euler(eulerRotation);
                position = shootingPoint.position;
            }
            else
            {
                var shootingPoints = _components.Shooter.ShootingPoints;

                if (shootingPoints.Count > 0)
                {
                    Vector3 eulerRotation = shootingPoints[0].eulerAngles;
                    eulerRotation.x = 0;
                    eulerRotation.z = 0;
                    rotation = Quaternion.Euler(eulerRotation);
                    position = shootingPoints[0].position;
                }
            }

            if (angle != 0)
            {
                rotation *= Quaternion.Euler(0f, angle, 0f);
            }

            bullet.transform.SetPositionAndRotation(position, rotation);
            bullet.StartBullet(_components);
            bullet.SetTargetLayerMask(Helper.GetOpposingTag(_components.gameObject.tag));
        }

        public void SpawnBullet(Bullet bullet, Vector3 spawnPoint, Vector3 direction, float angle = 0)
        {
            bullet.gameObject.SetActive(true);

            Vector3 eulerRotation = direction;
            eulerRotation.x = 0;
            eulerRotation.y += angle;
            Quaternion rotation = Quaternion.Euler(eulerRotation);

            bullet.transform.SetPositionAndRotation(spawnPoint, rotation);
            bullet.StartBullet(_components);
            bullet.SetTargetLayerMask(Helper.GetOpposingTag(_components.gameObject.tag));
        }
        #endregion

        #region SpawnShootingEffects Overloads
        public void ShowShootingEffects(ParticleSystemHandler muzzleEffect, Transform shootingPoint = null)
        {
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (shootingPoint != null)
            {
                position = shootingPoint.position;
                rotation = shootingPoint.rotation;
            }
            else
            {
                var shootingPoints = _components.Shooter.ShootingPoints;

                if (shootingPoints.Count > 0)
                {
                    rotation = shootingPoints[0].rotation;
                    position = shootingPoints[0].position;
                }
            }

            muzzleEffect.transform.SetPositionAndRotation(position, rotation);
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();
        }

        public void ShowShootingEffects(ParticleSystemHandler muzzleEffect, Vector3 position, Vector3 rotation)
        {
            Quaternion effectRotation = Quaternion.Euler(rotation);
            muzzleEffect.transform.SetPositionAndRotation(position, effectRotation);
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();
        }
        #endregion

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            // create the pools for the bullet
        }

        public override void SetSpeed(float speed)
        {
            BulletSpeed = speed;
        }
    }
}
