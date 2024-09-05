using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class AbilitySelectionGameState : IState
    {
        private StateMachine<GameStateType> _stateMachine;

        public AbilitySelectionGameState(StateMachine<GameStateType> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            Debug.Log("ABILITY SELECTION GAME STATE");
            // Disable all UI screens in the main canvas
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
