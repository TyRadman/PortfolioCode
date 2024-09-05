using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class LobbyGameState : IState
    {
        private StateMachine<GameStateType> _stateMachine;

        public LobbyGameState(StateMachine<GameStateType> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            Debug.Log("LOBBY GAME STATE");
            // Disable all UI screens in the main canvas
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);
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
