using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Laser_Aim", menuName = MENU_PATH + "Laser/Aim State")]
    public class LaserEnemyAimState : AimState
    {   
        private float _aimDuration;
        private float _aimTimer;

        public override void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

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
