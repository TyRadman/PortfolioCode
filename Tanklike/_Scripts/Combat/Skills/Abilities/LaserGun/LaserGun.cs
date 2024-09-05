using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "LaserGun", menuName = Directories.ABILITIES + "Laser Gun")]
    public class LaserGun : Ability
    {
        [SerializeField] private LaserWeapon _weapon;
        [SerializeField] private Vector2 _indicatorRange;
        [SerializeField] private float _crosshairSensitivityMultiplier = 0.5f;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if (components == null)
            {
                Debug.LogError($"No tank components passed to {name}.");
                return;
            }

            _components = components;
            _weapon.SetUp(components);
            
            if(_components == null)
            {
                Debug.LogError($"{components.gameObject.name} doesn't have a {_components.GetType()} component.");
                return;
            }
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            _weapon.OnShot();

            if (_components is PlayerComponents)
            {
                ((PlayerComponents)_components).Crosshair.SetCrosshairSpeedMultiplier(_crosshairSensitivityMultiplier);
                _components.StartCoroutine(ResetCrosshairSensitivityMultiplier());
            }
        }

        private IEnumerator ResetCrosshairSensitivityMultiplier()
        {
            yield return new WaitForSeconds(GetDuration());
            ((PlayerComponents)_components).Crosshair.ResetCrosshairSpeedMultiplier();
        }

        public override void SetUpIndicatorSpecialValues(BaseIndicator indicator)
        {
            base.SetUpIndicatorSpecialValues(indicator);

            if (indicator is not StraightIndicator)
            {
                Debug.LogError($"Ability {name} is expecting indicator of type {nameof(AirIndicator)}. Wrong indicator.");
                Debug.Break();
                return;
            }

            StraightIndicator straightIndicator = (StraightIndicator)indicator;
            straightIndicator.SetUpValues(_indicatorRange);
        }

        public override float GetDuration()
        {
            return _weapon.GetLaserDuration();
        }
    }
}
