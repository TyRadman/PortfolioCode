using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Shotgun_Move", menuName = MENU_PATH + "Shotgun/Move State")]
    public class ShotgunMoveState : MoveState
    {
        [SerializeField] private Vector2 _movementDistanceRange;

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

            _pointFinderCounter = 0;
            _targetPointFound = false;

            while (!_targetPointFound && _pointFinderCounter < 50)
            {
                //get random point and move to it
                float angle = Random.Range(0.0f, Mathf.PI * 2);
                Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                dir *= Random.Range(_movementDistanceRange.x, _movementDistanceRange.y);
                dir.y = 0.5f;

                MoveToTarget(_movement.transform.position + dir);
                _pointFinderCounter++;
            }

            if (!_targetPointFound)
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

        private void OnTargetReachedHandler()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(EnemyStateType.AIM);
        }
    }
}
