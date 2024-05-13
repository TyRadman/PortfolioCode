using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Archer_Aim", menuName = MENU_PATH + "Archer/Aim State")]
    public class ArcherAimState : AimState
    {
        private Transform _target;
        private int _rotationDirection;
        private float _aimDuration;
        private float _aimTimer;

        [Header("Archer values")]
        [SerializeField] private Vector2 _standbyDurationRange;
        private float _standbyDuration;
        private float _standbyTimer;

        public override void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

            //find target position and set rotation direction
            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
            _rotationDirection = _movement.GetRotationDirection(_target);

            //set random values for state parameters
            _aimDuration = Random.Range(_aimDurationRange.x, _aimDurationRange.y);
            _aimTimer = 0f;
            _standbyDuration = Random.Range(_standbyDurationRange.x, _standbyDurationRange.y);
            _standbyTimer = 0f;
        }

        public override void OnUpdate()
        {
            //switch to MOVE state when the timer is up
            if (_aimTimer >= _aimDuration)
            {
                _stateMachine.ChangeState(EnemyStateType.MOVE);
            }

            _aimTimer += Time.deltaTime;

            _turretController.HandleTurretRotation(_target);

            //stop rotating and check if the target can be shot
            if (_shooter.IsWayToTargetBlocked(_target))
                return;

            //if not, stand by, and shoot if there is no cooldown
            _standbyTimer += Time.deltaTime;

            if (_standbyTimer >= _standbyDuration)
            {
                if (_shooter.IsCanShoot()) _stateMachine.ChangeState(EnemyStateType.ATTACK);
            }

            ////rotate until facing the target
            //float dot = _movement.GetDotToTarget(_target);

            //if(dot < 0.999f)
            //{
            //    _rotationDirection = _movement.GetRotationDirection(_target);
            //    _movement.TurnTurret(_rotationDirection);
            //}
            //else
            //{
            //    _movement.TurnTurret(0);

            //    //stop rotating and check if the target can be shot
            //    if (_shooter.IsWayToTargetBlocked(_target))
            //        return;

            //    //if not, stand by, and shoot if there is no cooldown
            //    _standbyTimer += Time.deltaTime;

            //    if(_standbyTimer >= _standbyDuration)
            //    {
            //        if(_shooter.IsCanShoot()) _stateMachine.ChangeState(EnemyStateType.ATTACK);
            //    }
            //}
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
