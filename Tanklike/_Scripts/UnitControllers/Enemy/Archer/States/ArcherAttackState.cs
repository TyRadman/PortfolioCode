using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Archer_Attack", menuName = MENU_PATH + "Archer/Attack State")]
    public class ArcherAttackState : AttackState
    {
        [SerializeField] private float _moveWhileShootingChance = 0f;
        [SerializeField] private Vector2 _movementDistanceRange;

        private EnemyMovement _movement;
        private Transform _target;

        private bool _isTelegraphing;
        [SerializeField] private Vector2 _attackCooldownRange;
        private float _cooldownDuration;
        private float _cooldownTimer;
        private bool _isCooldown;

        private Vector3 _targetPoint;
        private bool _targetPointFound;
        private int _pointFinderCounter;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _stateMachine = stateMachine;
            _movement = (EnemyMovement)enemyComponents.Movement;
            _shooter = (EnemyShooter)enemyComponents.Shooter;
            _shooter.OnAttackFinished += OnAttackFinishedHandler;
            _shooter.OnTelegraphFinished += OnTelegraphFinishedHandler;
        }

        public override void OnEnter()
        {
            //Debug.Log("ATTACK STATE");

            _isActive = true;
            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            _shooter.TelegraphAttack();
            _isTelegraphing = true;

            _isCooldown = false;
            _cooldownDuration = Random.Range(_attackCooldownRange.x, _attackCooldownRange.y);
            _cooldownTimer = 0f;

            float moveChance = Random.Range(0f, 1f);

            if (moveChance <= _moveWhileShootingChance)
            {
                StartMovement();
            }
        }

        public override void OnUpdate()
        {
            if(_target == null)
            {
                _target= GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
            }

            _turretController.HandleTurretRotation(_target);
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _shooter.OnShootFinished -= OnAttackFinishedHandler;
        }

        private void StartMovement()
        {
            //Debug.Log("CHOSE MOVEMENT");

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

            if (!_targetPointFound)
            {
                _stateMachine.ChangeState(EnemyStateType.AIM);
            }
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

        private void OnTelegraphFinishedHandler()
        {
            if (!_isActive)
                return;

            _isTelegraphing = false;
            _shooter.StartAttackRoutine(_attacksAmountRange.RandomValue(), _breakBetweenAttacks);
        }

        private void OnAttackFinishedHandler()
        {
            if (!_isActive)
                return;

            //_isCooldown = true;
            _stateMachine.ChangeState(EnemyStateType.MOVE);
        }
    }
}
