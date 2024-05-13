using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class MoveState : State
    {
        protected bool _isActive;
        protected EnemyMovement _movement;
        protected EnemyTurretController _turretController;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _movement = (EnemyMovement)enemyComponents.Movement;
            _turretController = (EnemyTurretController)enemyComponents.TurretController;
        }
    }
}
