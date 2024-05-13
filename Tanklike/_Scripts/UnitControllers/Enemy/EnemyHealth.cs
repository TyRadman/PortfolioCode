using System.Collections;
using System.Collections.Generic;
using System.Text;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyHealth : TankHealth
    {
        [Header("Death Explosion")]
        [SerializeField] protected bool _explodeOnDeath;
        [SerializeField] protected float _explosionForce = 10f;
        [SerializeField] protected float _explosionRadius = 2f;
        [SerializeField] protected float _upwardsModifier = 0.5f;
        protected int _playerIndex = -1;
        private EnemyComponents _enemyComponents;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if (components is EnemyComponents)
            {
                _enemyComponents = (EnemyComponents)components;
            }
        }

        public override void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            base.TakeDamage(damage, direction, shooter, bulletPosition);

            if (shooter != null)
            {
                _playerIndex = ((PlayerComponents)shooter).PlayerIndex;
            }

            if (shooter != null)
            {
                shooter.Shooter.OnTargetHit?.Invoke();
            }
        }

        public override void Die()
        {
            base.Die();
            EnemyData enemyStats = (EnemyData)_stats;
            _enemyComponents.ItemDrop.DropItem();
            GameManager.Instance.ReportManager.ReportEnemyKill(enemyStats, _playerIndex);
            GameManager.Instance.EnemiesManager.RemoveEnemy(GetComponent<EnemyAIController>());

            if (_explodeOnDeath)
            {
                DetonateBody();
            }
        }

        private void DetonateBody()
        {
            EnemyData enemyStats = (EnemyData)_stats;
            EnemyParts parts = GameManager.Instance.EnemiesManager.GetEnemyPartsByType(enemyStats.EnemyType);
            parts.transform.position = transform.position;
            parts.transform.rotation = transform.rotation;
            parts.gameObject.SetActive(true);
            parts.StartExplosion(_explosionForce, _explosionRadius, _upwardsModifier, _turret.rotation, _body.rotation);
        }

        public void Explode()
        {
            base.Die();
            GameManager.Instance.EnemiesManager.RemoveEnemy(GetComponent<EnemyAIController>());
            DetonateBody();
        }
    }
}
