using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Explosive_Attack", menuName = MENU_PATH + "Explosive/Attack State")]
    public class ExplosiveAttackState : AttackState
    {
        private EnemyHealth _health;
        private float _cooldownDuration;
        private float _cooldownTimer;
        private bool _isCooldown;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _stateMachine = stateMachine;
            _health = (EnemyHealth)enemyComponents.Health;
            _shooter.OnShootFinished += OnShootFinishedHandler;
        }

        public override void OnEnter()
        {
            //Debug.Log("ATTACK STATE");

            _isActive = true;

            _health.Explode();

            ExplosiveShooter explosiveShooter = (ExplosiveShooter)_shooter;
            explosiveShooter.Explode();
        }

        public override void OnUpdate()
        {
            //if (!_isCooldown)
            //    return;

            //_cooldownTimer += Time.deltaTime;
            //if(_cooldownTimer >= _cooldownDuration)
            //{
            //    _stateMachine.ChangeState(EnemyStateType.MOVE);
            //}
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _shooter.OnShootFinished -= OnShootFinishedHandler;
        }

        private void OnShootFinishedHandler()
        {
            if (!_isActive)
                return;

            _isCooldown = true;
        }
    }
}
