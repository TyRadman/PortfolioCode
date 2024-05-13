using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class SummonShooter : TankShooter
    {
        [Header("Settings")]
        [SerializeField] private Transform _summonShooterPoint;

        protected override void Awake()
        {
            base.Awake();
            _shootingPoints.Add(_summonShooterPoint);
        }

        public bool ShouldAttackEnemies()
        {
            if (GameManager.Instance.RoomsManager.CurrentRoom == null)
            {
                return false;
            }

            return true;
        }

        public EnemyAIController GetClosestTarget()
        {
            if (GameManager.Instance.RoomsManager.CurrentRoom == null)
            {
                return null;
            }

            if (GameManager.Instance.RoomsManager.CurrentRoom.Spawner.CurrentlySpawnedEnemies == null)
            {
                return null;
            }

            var enemiesList = GameManager.Instance.RoomsManager.CurrentRoom.Spawner.CurrentlySpawnedEnemies;

            if (enemiesList.Count <= 0)
            {
                return null;
            }

            EnemyAIController closestEnemy = enemiesList[0];
            float dist = Vector3.Distance(transform.position, closestEnemy.transform.position);

            for (int i = 1; i < enemiesList.Count; i++)
            {
                if (Vector3.Distance(transform.position, enemiesList[i].transform.position) < dist)
                {
                    dist = Vector3.Distance(transform.position, enemiesList[i].transform.position);
                    closestEnemy = enemiesList[i];
                }
            }

            return closestEnemy;
        }

        public void ShootTarget(Transform target)
        {
            Vector3 dir = target.position - _shootingPoints[0].position;
            dir.Normalize();
            _shootingPoints[0].rotation = Quaternion.LookRotation(dir);
            _currentWeapon.OnShot(_components, _shootingPoints[0]);
        }
    }
}