using System.Collections;
using System.Collections.Generic;
using TankLike.UI;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class MainMenuGameState : IState
    {
        private StateMachine<GameStateType> _stateMachine;

        public MainMenuGameState(StateMachine<GameStateType> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            Debug.Log("MAIN MENU GAME STATE");
            // Enable the main menu screen only   
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
