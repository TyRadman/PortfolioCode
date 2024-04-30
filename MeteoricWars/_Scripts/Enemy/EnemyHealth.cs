using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyHealth : ShipHealth
    {
        [SerializeField] private EnemyStats m_Stats;
        [SerializeField] private EnemyComponents m_Components;

        private void OnEnable()
        {
            CurrentHealth = MaxHealth;
        }

        public override void TakeDamage(float _amount, int _userIndex)
        {
            base.TakeDamage(_amount, _userIndex);

            if (!CanBeShot) return;

            if (m_Stats == null) print("Not stats");

            if (GameManager.i.EnemyHealthBar == null) print("Not health bar");

            if (GameManager.i.EnemyHealthBar != null)
            {
                GameManager.i.EnemyHealthBar.UpdateHealthBar(CurrentHealth, MaxHealth, m_Stats.EnemyName);
            }

            if (CurrentHealth <= 0f)
            {
                CanBeShot = false;
                m_Stats.OnShipDestroyed(_userIndex);
            }
        }

        public void SetUpStats(float _health)
        {
            // setting health
            MaxHealth = (int)_health;
            CurrentHealth = (int)_health;
        }
    }
}