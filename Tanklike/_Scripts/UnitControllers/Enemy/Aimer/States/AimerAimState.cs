using TankLike.UnitControllers;
using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Aimer_Aim", menuName = MENU_PATH + "Aimer/Aim")]
    public class AimerAimState : AimState
    {
        private Transform _target;
        private int _rotationDirection;

        private float _aimDuration;
        // the time after which the tank attacks
        private float _aimTimer;

        public AimerAimState(StateMachine<EnemyStateType> stateMachine, EnemyMovement movement, EnemyShooter shooter,
            Vector2 aimDurationRange)
        {
            _stateMachine = stateMachine;
            _movement = movement;
            _shooter = shooter;
            _aimDurationRange = aimDurationRange;
        }

        public override void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

            //find target position and set rotation direction
            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            //set random values for state parameters
            _aimDuration = Random.Range(_aimDurationRange.x, _aimDurationRange.y);
            _aimTimer = 0f;
        }

        public override void OnUpdate()
        {
            //switch to ATTACK state when the timer is up
            if (_aimTimer >= _aimDuration)
            {
                _stateMachine.ChangeState(EnemyStateType.ATTACK);
            }

            _aimTimer += Time.deltaTime;

            _turretController.HandleTurretRotation(_target);
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
        }
    }
}
