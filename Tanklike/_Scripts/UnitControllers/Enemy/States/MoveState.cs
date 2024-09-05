using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers.States
{
    public class MoveState : State
    {
        [SerializeField] protected float _maxPathLength = 10f;

        protected bool _isActive;
        protected EnemyMovement _movement;
        protected EnemyTurretController _turretController;

        protected Transform _target;
        protected Vector3 _targetPoint;
        protected bool _targetPointFound;
        protected int _pointFinderCounter;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _movement = (EnemyMovement)enemyComponents.Movement;
            _turretController = (EnemyTurretController)enemyComponents.TurretController;
        }

        protected virtual void MoveToTarget(Vector3 target)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 10.0f, NavMesh.AllAreas))
            {
                _targetPoint = hit.position;

                // Check for path length
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(_movement.transform.position, _targetPoint, NavMesh.AllAreas, path);

                float pathLength = _movement.GetPathLength(path);

                if (pathLength < _maxPathLength)
                {
                    _targetPointFound = true;
                    _movement.SetTargetPosition(_targetPoint);
                }
                else
                {
                    Debug.Log($"Path length {pathLength} is too big!");
                }
            }
        }
    }
}
