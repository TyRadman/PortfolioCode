using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;
    using Combat.Abilities;

    public class PlayerSuperAbilityRecharger : MonoBehaviour, IController
    {
        [SerializeField] private AbilityRechargingMethod _currentRechargingMethod;
        [SerializeField] private PlayerComponents _components;
        /// <summary>
        /// A value between 0 and 1 where 1 indicating the ability is fully charged and ready to use
        /// </summary>
        private float _abilityChargeAmount = 0f;
        private float _chargePerSecond = 0f;
        private float _chargePerEnemyHit = 0f;
        private float _chargePerHit = 0f;

        public bool IsActive { get; private set; }

        #region Set Up
        public void SetUpRechargeMethods(RechargingValues rechargingMethods)
        {
            // unsubscribe, just in case
            _components.Health.OnHit -= RechargeOnHit;
            _components.Shooter.OnTargetHit -= RechargeOnEnemyHit;
            
            _currentRechargingMethod = rechargingMethods.RechargingMethod;
            // get the values that are gonna be used with every burst of super recharge
            _chargePerSecond = 1f / rechargingMethods.Time;
            _chargePerHit = 1f / rechargingMethods.NumberOfHits;
            _chargePerEnemyHit = 1f / rechargingMethods.NumberOfEnemyHits;

            // subscribe recharging methods to their corresponding events
            if ((rechargingMethods.RechargingMethod & AbilityRechargingMethod.OnPlayerHit) != 0) _components.Health.OnHit += RechargeOnHit;

            if ((rechargingMethods.RechargingMethod & AbilityRechargingMethod.OnEnemyHit) != 0) _components.Shooter.OnTargetHit += RechargeOnEnemyHit;

            // we start recharging by default since we start with an empty bar
            EnableRecharging();
        }
        #endregion

        public void EnableRecharging()
        {
            // set the amount charged of the ability to zero
            _abilityChargeAmount = 0f;

            // we check if the current recharging methods include charging the super over time
            if ((_currentRechargingMethod & AbilityRechargingMethod.OverTime) != 0)
            {
                StartCoroutine(OverTimeChargingProcess());
            }
        }

        private void RechargeOnHit()
        {
            if (_abilityChargeAmount >= 1f)
            {
                return;
            }

            AddAbilityChargeAmount(_chargePerHit);
        }

        private void RechargeOnEnemyHit()
        {
            if (_abilityChargeAmount >= 1f)
            {
                return;
            }

            AddAbilityChargeAmount(_chargePerEnemyHit);
        }

        private IEnumerator OverTimeChargingProcess()
        {
            while (_abilityChargeAmount < 1f)
            {
                AddAbilityChargeAmount(_chargePerSecond * Time.deltaTime);
                yield return null;
            }
        }

        private void AddAbilityChargeAmount(float amount)
        {
            _abilityChargeAmount += amount;
            // update UI
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetSuperAbilityChargeAmount(1 - _abilityChargeAmount, 0);

            if (_abilityChargeAmount >= 1f)
            {
                // reset the amount in case it overflowed
                _abilityChargeAmount = 1f;
                // enable the usage of the super ability
                ((PlayerSuperAbilities)_components.SuperAbility).EnableAbilityUsage();
            }
        }

        public void FullyChargeSuperAbility()
        {
            if (_abilityChargeAmount == 1f)
            {
                return;
            }

            _abilityChargeAmount = 1f;
            ((PlayerSuperAbilities)_components.SuperAbility).EnableAbilityUsage();
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetSuperAbilityChargeAmount(1 - _abilityChargeAmount, 0);
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
