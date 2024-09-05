using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class BossesTestGameState : IState
    {
        private StateMachine<GameStateType> _stateMachine;

        public BossesTestGameState(StateMachine<GameStateType> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            Debug.Log("BOSSES TEST GAME STATE");

            //// Set active scene
            //Scene targetScene = SceneManager.GetSceneByName("S_Gameplay");
            //SceneManager.SetActiveScene(targetScene);

            // Enable gameplay UI screens
            GameManager.Instance.ResultsUIController.gameObject.SetActive(true);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(true);
            GameManager.Instance.HUDController.gameObject.SetActive(true);

            GameManager.Instance.RoomsManager.SetUp();
            GameManager.Instance.RoomsManager.LoadBossRoom();

            GameManager.Instance.NotificationsManager.SetUp();
            GameManager.Instance.AudioManager.SetUp();
            GameManager.Instance.InputManager.SetUp();
            GameManager.Instance.DamagePopUpManager.SetUp();
            GameManager.Instance.VisualEffectsManager.SetUp();

            GameManager.Instance.PlayersManager.SetUp();
            GameManager.Instance.BossesManager.SetUp();

            GameManager.Instance.PlayersManager.PlayerSpawner.SpawnPlayers();

            GameManager.Instance.CameraManager.SetUp(false);
            GameManager.Instance.InteractableAreasManager.SetUp();
            GameManager.Instance.CollectableManager.SetUp();

            GameManager.Instance.BossKeysManager.SetUp();

            GameManager.Instance.ShopsManager.SetUp();
            GameManager.Instance.QuestsManager.SetUp();
            GameManager.Instance.BulletsManager.SetUp();

            GameManager.Instance.InputManager.EnablePlayerInput();

            GameManager.Instance.OffScreenIndicator.SetUp();
            GameManager.Instance.PauseMenuManager.SetUp();
            GameManager.Instance.HUDController.SetUp();
            GameManager.Instance.EffectsUIController.SetUp();

            //Reset camera limit
            GameManager.Instance.CameraManager.ResetCameraLimit();

            // Re-set current room to move the room cover
            GameManager.Instance.RoomsManager.SetCurrentRoom(GameManager.Instance.RoomsManager.CurrentRoom);

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
