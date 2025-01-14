using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using UI.HUD;
    using Combat;
    using Cam;
    using Utils;
    using UI.DamagePopUp;
    using System;

    public class PlayerHealth : TankHealth
    {
        [Header("Health Flash")]
        [SerializeField] private int _flashThreshold = 100;

        private PlayerComponents _playerComponents;
        private PlayerHUD _HUD;

        public override void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            
            base.SetUp(_playerComponents);

            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];
        }

        public override void TakeDamage(int damage, Vector3 direction, UnitComponents shooter, Vector3 bulletPosition, Ammunition damageDealer = null)
        {
            if (!_canTakeDamage || IsConstrained || IsDead)
            {
                return;
            }

            base.TakeDamage(damage, direction, shooter, bulletPosition, damageDealer);

            if(_currentHealth != 0f)
            {
                GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.HIT);
                GameManager.Instance.HUDController.DamageScreenUIController.ShowDamageScreen(_playerComponents.PlayerIndex, _currentHealth <= _flashThreshold);
            }
        }

        public override void Die()
        {
            base.Die();
            gameObject.SetActive(false);
            _playerComponents.TankBodyParts.HandlePartsExplosion();
        }

        public override void RestoreFullHealth()
        {
            base.RestoreFullHealth();
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            _HUD.UpdateHealthBar(_lastCurrentHealth, _currentHealth, _maxHealth);

            if (_currentHealth <= _flashThreshold)
            {
                _HUD.StartHealthBarFlash();
            }
            else
            {
                _HUD.StopHealthBarFlash();
                GameManager.Instance.HUDController.DamageScreenUIController.HideLowHealthOverlay(_playerComponents.PlayerIndex);
            }
        }

        public override void SetHealthPercentage(float value, bool playEffects = true)
        {
            value = Mathf.Clamp01(value);

            int expectedHealth = Mathf.CeilToInt((float)_maxHealth * value);

            if(!playEffects)
            {
                int health = expectedHealth - _currentHealth;

                _currentHealth = expectedHealth;
                _lastCurrentHealth = expectedHealth; // To avoid playing the damage effects (when lastCurrentHealth < currentHealth)
                UpdateHealthUI();

                // Display pop up
                if (PopUpAnchor != null)
                {
                    DamagePopUpType type = health < 0 ? DamagePopUpType.Damage : DamagePopUpType.Heal;
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(type, health, PopUpAnchor.Anchor);
                }

                return;
            }

            // if the current health is greater than the new health, then the change is a damage, otherwise, it's a heal
            if (_currentHealth >= expectedHealth)
            {
                int damageAmountToInflict = _currentHealth - expectedHealth;
                TakeDamage(damageAmountToInflict, Vector3.zero, null, Vector3.zero);
            }
            else
            {
                int amountTohealth = expectedHealth - _currentHealth;
                Heal(amountTohealth);
            }
        }

        #region Utilities
        protected override void AddToHealthValue(int value)
        {
            base.AddToHealthValue(value);
            UpdateHealthUI();
        }

        protected override void SetHealthValue(int value)
        {
            base.SetHealthValue(value);

            _HUD.SetHealthBar(_currentHealth, _maxHealth);

            if (_currentHealth <= _flashThreshold)
            {
                _HUD.StartHealthBarFlash();
            }
            else
            {
                _HUD.StopHealthBarFlash();
                GameManager.Instance.HUDController.DamageScreenUIController.HideLowHealthOverlay(_playerComponents.PlayerIndex);
            }
        }
        #endregion

        #region IController
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Restart()
        {
            base.Restart();
            RestoreFullHealth();
            _HUD.SetupHealthBar(_maxHealth);

            OnHit += _playerComponents.Shield.ActivateShield;
            OnDeath += _playerComponents.OnDeathHandler;

            int layer = GameManager.Instance.Constants.Alignments.Find(a => a.Alignment == TankAlignment.PLAYER).LayerNumber;
            SetDamageDetectorsLayer(layer);

            if (GameManager.Instance.PlayersManager != null)
            {
                OnDeath += GameManager.Instance.PlayersManager.ReportPlayerDeath;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _currentHealth = 0;
            _HUD.ResetHealthBar();
            _HUD.StopHealthBarFlash();
            GameManager.Instance.HUDController.DamageScreenUIController.HideLowHealthOverlay(_playerComponents.PlayerIndex);

            OnHit -= _playerComponents.Shield.ActivateShield;
            OnDeath -= _playerComponents.OnDeathHandler;

            if (GameManager.Instance.PlayersManager != null)
            {
                OnDeath -= GameManager.Instance.PlayersManager.ReportPlayerDeath;
            }
        }
        #endregion
    }
}
