using System.Collections;
using System.Collections.Generic;
using System.Text;
using TankLike.UI;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UnitControllers
{
    public class EnemyHealth : TankHealth
    {
        [Header("Health Bar")]
        [SerializeField] private bool _enableHealthBar;
        [SerializeField] private GameObject _healthBarCanvas;
        [SerializeField] private HealthBar _healthBar;

        public int PlayerIndex { get; protected set; } = -1;
        private EnemyComponents _enemyComponents;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if (components is EnemyComponents enemyComponents)
            {
                _enemyComponents = enemyComponents;
            }

            if(_healthBarCanvas != null)
            {
                if (_enableHealthBar)
                {
                    _healthBarCanvas.SetActive(true);
                    _healthBar.SetupHealthBar();
                }
                else
                {
                    _healthBarCanvas.SetActive(false);
                }
            }         
        }

        public override void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            if (IsDead)
            {
                return;
            }

            base.TakeDamage(damage, direction, shooter, bulletPosition);

            if (shooter != null)
            {
                PlayerIndex = ((PlayerComponents)shooter).PlayerIndex;
            }

            if (shooter != null)
            {
                shooter.Shooter.OnTargetHit?.Invoke();
            }

            if (_healthBar != null)
            {
                _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
            }
        }

        public override void Die()
        {
            base.Die();

            if (IsDead)
            {
                return;
            }

            EnemyData enemyStats = (EnemyData)_stats;
            _components.TankBodyParts.HandlePartsExplosion((EnemyData)_stats);      
            _enemyComponents.ItemDrop.DropItem();
            GameManager.Instance.ReportManager.ReportEnemyKill(enemyStats, PlayerIndex);
            GameManager.Instance.EnemiesManager.RemoveEnemy(_enemyComponents);
        }

        public void Explode()
        {
            base.Die();
            _components.TankBodyParts.HandlePartsExplosion((EnemyData)_stats);
            GameManager.Instance.EnemiesManager.RemoveEnemy(_enemyComponents);
        }
    }
}
