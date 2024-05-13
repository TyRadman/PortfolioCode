using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class AimState : State
    {
        [Header("Aim Settings")]
        [SerializeField] protected Vector2 _aimDurationRange;

        protected bool _isActive;
        protected EnemyMovement _movement;
        protected EnemyShooter _shooter;
        protected EnemyTurretController _turretController;

        public void SetAimDurationRange(Vector2 aimDurationRange)
        {
            _aimDurationRange = aimDurationRange;
        }

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);
            _movement = (EnemyMovement)enemyComponents.Movement;
            _shooter = (EnemyShooter)enemyComponents.Shooter;
            _turretController = (EnemyTurretController)enemyComponents.TurretController;
        }
    }
}
