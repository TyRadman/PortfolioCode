using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class SplashScreenGameState : IState
    {
        private StateMachine<GameStateType> _stateMachine;

        public SplashScreenGameState(StateMachine<GameStateType> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            Debug.Log("SPLASH SCREEN GAME STATE");
        }

        public void OnUpdate()
        {
        }

        public void OnExit()
        {
        }

        public void OnDispose()
        {
        }
    }
}
