using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerHealth : TankHealth
    {
        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            GameManager.Instance.HUDController.PlayerHUDs[((PlayerComponents)_components).PlayerIndex].SetupHealthBar(_maxHealth);
        }

        public override void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            if (!_canTakeDamage)
            {
                return;
            }

            base.TakeDamage(damage, direction, shooter, bulletPosition);
            GameManager.Instance.HUDController.PlayerHUDs[((PlayerComponents)_components).PlayerIndex].UpdateHealthBar(_currentHealth, _maxHealth);
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.HIT);
            // recharge ability if there's an event subscribed
        }

        public override void Die()
        {
            base.Die();
            gameObject.SetActive(false);
            GameManager.Instance.PlayersManager.ReportPlayerDeath(((PlayerComponents)_components).PlayerIndex);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            GameManager.Instance.HUDController.PlayerHUDs[((PlayerComponents)_components).PlayerIndex].UpdateHealthBar(_currentHealth, _maxHealth);
        }
    }
}
