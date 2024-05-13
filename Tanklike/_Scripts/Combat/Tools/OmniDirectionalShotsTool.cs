using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;
using TankLike.Utils;
using TankLike.UnitControllers;
using TankLike.Cam;

namespace TankLike.Combat
{
    public class OmniDirectionalShotsTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] protected Weapon _normalShot;
        [SerializeField] private int _bulletsNumber = 3;
        [SerializeField] private float _angleBetweenShots = 15f;
        [SerializeField] private float _startingAngle;
        [SerializeField] private float _bulletSpeed;

        private TankComponents _shooter;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _shooter = tankTransform.GetComponent<TankComponents>();
            _normalShot.SetSpeed(_bulletSpeed);
            //_startingAngle = (_bulletsNumber - 1) * _angleBetweenShots / -2f;
        }

        public override void UseTool()
        {
            base.UseTool();

            for (int i = 0; i < _bulletsNumber; i++)
            {
                //Quaternion rotation = Quaternion.Euler(0f, _startingAngle + _angleBetweenShots * i, 0f) * _shooter.transform.rotation;

                _normalShot.OnShot(_shooter, _shooter.transform, _startingAngle + _angleBetweenShots * i);

                GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);

                // create the bullet
                //Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(_bulletdata.GUID);

                //bullet.transform.SetPositionAndRotation(_shooter.transform.position, rotation);
                //bullet.gameObject.SetActive(true);

                //_shooter.LaunchBullet(bullet);

                //_shooter.Shoot(_bulletPrefab, _shooter.transform.position, _startingAngle + _angleBetweenShots * i, false);
            }
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }
    }
}
