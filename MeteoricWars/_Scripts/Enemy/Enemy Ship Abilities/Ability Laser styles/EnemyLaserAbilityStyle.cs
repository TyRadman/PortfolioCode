using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyLaserAbilityStyle : EnemyAbilityStyle
    {
        public LaserAbilityStyles Style;
        // we must cache the action, otherwise, stopping all coroutines when the ship is destroyed would stop all other active ships
        protected Coroutine ActionCoroutine;
        public Vector2 WarningTimeRange;
        public Vector2 ShootingTimeRange;
        public float WarningTime;
        [HideInInspector] public float ShootingTime;
        public LaserIndicator Indicator;

        protected virtual void Awake()
        {

        }

        public virtual void Perform(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {

        }

        /// <summary>
        /// Stops the laser shooting process (must be perfected)Shoots a laser beam that hits all enemies on its way.
        /// </summary>
        public virtual void ForceStop()
        {
            if (ActionCoroutine != null)
            {
                GameManager.i.GeneralValues.StopCoroutine(ActionCoroutine);
            }

            GameManager.i.GeneralValues.StopAllCoroutines();
            Indicator.StopIndicator();
        }

        public virtual void SetUp(float _difficulty, RedLaserAbility _ability, Transform _ship)
        {
            WarningTime = WarningTimeRange.Lerp(_difficulty);
            ShootingTime = ShootingTimeRange.Lerp(_difficulty);
            _ability.SetLaserDuration(ShootingTime);
        }
    }
}