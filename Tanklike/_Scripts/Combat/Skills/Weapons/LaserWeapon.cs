using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_Laser_NAME", menuName = DIRECTORY + "Laser")]
    public class LaserWeapon : Weapon
    {
        [Header("Laser Settings")]
        [SerializeField] private Laser _laserPrefab;
        [SerializeField] private float _duration;
        [field: SerializeField] public float MaxLength;
        [field: SerializeField] public float Thickness;
        [field: SerializeField] public float DamageInterval = 0.1f;

        private List<IPoolable> _activePoolables;

        public override void OnShot(Transform shootingPoint = null, float angle = 0)
        {
            base.OnShot();

            Laser laser = GameManager.Instance.VisualEffectsManager.Lasers.GetLaser(BulletData.GUID);
            laser.SetValues(MaxLength, Thickness, Damage, DamageInterval, _duration);
            ShootLaser(laser, shootingPoint, angle);
        }

        public void ShootLaser(Laser laser, Transform shootingPoint = null, float angle = 0)
        {
            // create the bullet
            laser.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            var shootingPoints = _components.Shooter.ShootingPoints;

            if (shootingPoints != null)
            {
                rotation = shootingPoints[0].rotation;
                position = shootingPoints[0].position;
            }

            if (shootingPoint != null)
            {
                rotation = shootingPoint.rotation;
                position = shootingPoint.position;
            }
            else
            {
                shootingPoint = shootingPoints[0];
            }

            if (angle != 0)
            {
                rotation *= Quaternion.Euler(0f, angle, 0f);
            }

            laser.transform.SetPositionAndRotation(position, rotation);
            laser.transform.parent = shootingPoint;
            laser.SetUp(_components, RemoveFromActivePoolables);
            laser.SetTargetLayerMask(Helper.GetOpposingTag(_components.gameObject.tag));
            // activate laser
            laser.Activate();
            _activePoolables.Add(laser);
        }

        private void RemoveFromActivePoolables(IPoolable poolable)
        {
            if (_activePoolables.Contains(poolable))
            {
                _activePoolables.Remove(poolable);
            }
        }

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _activePoolables = new List<IPoolable>();
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public float GetLaserDuration()
        {
            return _duration;
        }

        public override void DisposeWeapon()
        {
            base.DisposeWeapon();

            if (_activePoolables.Count > 0)
            {
                _activePoolables.ForEach(e => e.TurnOff());
            }

            _activePoolables.Clear();
        }
    }
}
