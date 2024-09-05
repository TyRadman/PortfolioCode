using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = PREFIX + "MachineGun", menuName = Directories.ABILITIES + "Machine Gun")]
    public class MachineGun : Ability
    {
        [Header("Special Values")]
        [SerializeField] private int _shotsNumber;
        [SerializeField] private float _shootingDuration = 3f;
        [SerializeField] private float _crosshairSpeedMultiplier = 0.5f;
        [SerializeField][Range(0f, 90f)] private float _maxRandomAngle = 5f;
        [SerializeField] private Weapon _weapon;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _weapon.SetUp(components);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            _components.StartCoroutine(ShootingProcess(_components.Shooter));
        }

        private IEnumerator ShootingProcess(TankShooter shooter)
        {
            //ApplyConstraints(true);
            // slow down the turret rotation speed
            ((PlayerComponents)_components).Crosshair.SetCrosshairSpeedMultiplier(_crosshairSpeedMultiplier);

            float shotDuration = _shootingDuration / _shotsNumber;
            WaitForSeconds wait = new WaitForSeconds(shotDuration);
            float halfAngle = _maxRandomAngle / 2;

            for (int i = 0; i < _shotsNumber; i++)
            {
                float angle = Random.Range(-halfAngle, halfAngle);
                shooter.Shoot(_weapon, angle, false);
                yield return wait;
            }

            //ApplyConstraints(false);
            ((PlayerComponents)_components).Crosshair.ResetCrosshairSpeedMultiplier();
        }
    }
}
