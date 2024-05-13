using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Archer_Move", menuName = MENU_PATH + "Archer/Move State")]
    public class ArcherMoveState : MoveState
    {
        [SerializeField] private Vector2 _movementDistanceRange;

        private Transform _target;
        private Vector3 _targetPoint;
        private bool _targetPointFound;
        private int _pointFinderCounter;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _stateMachine = stateMachine;
            _movement.OnTargetReached += OnTargetReachedHandler;
        }

        public override void OnEnter()
        {
            //Debug.Log("MOVE STATE");
            _isActive = true;

            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            //get random point and move to it
            float angle = Random.Range(0.0f, Mathf.PI * 2);
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            dir *= Random.Range(_movementDistanceRange.x, _movementDistanceRange.y);
            dir.y = 0.5f;
            _pointFinderCounter = 0;
            _targetPointFound = false;

            while (!_targetPointFound && _pointFinderCounter < 50)
            {
                MoveToTarget(_movement.transform.position + dir);
                _pointFinderCounter++;
            }

            if(!_targetPointFound)
            {
                _stateMachine.ChangeState(EnemyStateType.AIM);
            }
        }

        public override void OnUpdate()
        {
            if (_target == null)
            {
                _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
            }

            _turretController.HandleTurretRotation(_target);
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _movement.OnTargetReached -= OnTargetReachedHandler;
        }

        private bool MoveToTarget(Vector3 target)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 10.0f, NavMesh.AllAreas))
            {
                _targetPoint = hit.position;
                _targetPointFound = true;
                _movement.SetTargetPosition(_targetPoint);
                return true;
            }

            return false;
        }

        private void OnTargetReachedHandler()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(EnemyStateType.AIM);
        }
    }
}
