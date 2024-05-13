using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public class IceShotTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private Weapon _shot;
        private PlayerShooter _playerShooter;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);
            //// create bullet
            //_shot = Instantiate(_bulletPrefab, null);
            //_shot.gameObject.SetActive(false);

            _playerShooter = tankTransform.GetComponent<PlayerShooter>();
        }

        public override void UseTool()
        {
            base.UseTool();

            _playerShooter.SetCustomShot(_shot);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }
    }
}
