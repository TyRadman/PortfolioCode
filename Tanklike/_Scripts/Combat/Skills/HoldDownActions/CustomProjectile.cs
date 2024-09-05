using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = PREFIX + "CustomShot", menuName = Directories.ABILITIES + "Custom shot")]
    public class CustomProjectile : Ability
    {
        [SerializeField] private float _animationDelay = 0.15f;
        [Header("Special Values")]
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Vector2 _indicatorRange;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _components = components;
            _weapon.SetUp(_components);
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

            StraightIndicator airIndicator = (StraightIndicator)indicator;
            airIndicator.SetUpValues(_indicatorRange);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            _components.Animation.PlayShootAnimation();
            _components.StartCoroutine(ShootCustomProjectile());
        }

        private IEnumerator ShootCustomProjectile()
        {
            yield return new WaitForSeconds(_animationDelay);
            _weapon.OnShot();
        }
    }
}
