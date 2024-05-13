using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = PREFIX + "Shotgun", menuName = Directories.ABILITIES + "Shotgun")]
    public class ShotGunSA : Ability
    {
        [Header("Special Values")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Weapon _shot;
        [SerializeField] private int _bulletsNumber = 3;
        [SerializeField] private float _angleBetweenShots = 15f;
        [SerializeField] private float _startingAngle;
        private TankShooter _shooter;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            _shooter = tankTransform.GetComponent<TankShooter>();
            //_startingAngle = (_bulletsNumber - 1) * _angleBetweenShots / -2f;
        }

        public override void OnActivateAbility()
        {
            base.OnActivateAbility();

            for (int i = 0; i < _bulletsNumber; i++)
            {
                _shooter.Shoot(_shot, _startingAngle + _angleBetweenShots * i, false);
            }
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
        }
    }
}
