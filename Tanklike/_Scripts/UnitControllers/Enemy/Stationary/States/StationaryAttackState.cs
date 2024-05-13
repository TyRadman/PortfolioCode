using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Aimer_Attack", menuName = MENU_PATH + "Aimer/Attack")]
    public class StationaryAttackState : AttackState
    {
        private EnemyMovement _movement;
        private Transform _target;
        private bool _isTelegraphing;

        private Coroutine _attackCoroutine;

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
            _isActive = true;
            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            _shooter.TelegraphAttack();
            _isTelegraphing = true;
        }

        public override void OnUpdate()
        {
            _turretController.HandleTurretRotation(_target);
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _shooter.OnShootFinished -= OnAttackFinishedHandler;
            _shooter.OnTelegraphFinished -= OnTelegraphFinishedHandler;
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

            _stateMachine.ChangeState(EnemyStateType.AIM);
        }
    }
}
