using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class BossState : ScriptableObject, IState
    {
        [field: SerializeField] public BossStateType BossStateType;

        protected StateMachine<BossStateType> _stateMachine;

        public const string MENU_PATH = Directories.BOSSES + "States/";

        public virtual void SetUp(StateMachine<BossStateType> stateMachine, BossComponents enemyComponents)
        {
            _stateMachine = stateMachine;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnDispose()
        {
        }
    }
}
