using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class GameplayGameState : IState
    {
        private StateMachine<GameStateType> _stateMachine;

        public GameplayGameState(StateMachine<GameStateType> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            Debug.Log("GAMEPLAY GAME STATE");

            // Set active scene
            Scene targetScene = SceneManager.GetSceneByName("S_Gameplay");
            SceneManager.SetActiveScene(targetScene);

            // Enable the main menu screen only
            GameManager.Instance.ResultsUIController.gameObject.SetActive(true);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(true);
            GameManager.Instance.HUDController.gameObject.SetActive(true);

            GameManager.Instance.RoomsManager.SetUp();

            GameManager.Instance.LevelGenerator.GenerateLevel();        

            GameManager.Instance.DestructiblesManager.SetUp();
            GameManager.Instance.NotificationsManager.SetUp();
            GameManager.Instance.InputManager.SetUp();
            GameManager.Instance.DamagePopUpManager.SetUp();
            GameManager.Instance.VisualEffectsManager.SetUp();
            GameManager.Instance.BossKeysManager.SetUp();

            GameManager.Instance.PlayersManager.SetUp();
            GameManager.Instance.EnemiesManager.SetUp();
            GameManager.Instance.BossesManager.SetUp();

            GameManager.Instance.PlayersManager.PlayerSpawner.SpawnPlayers();

            GameManager.Instance.CameraManager.SetUp(false);
            //GameManager.Instance.ObstaclesVanisher.SetUp();
            GameManager.Instance.ReportManager.SetUp();
            GameManager.Instance.InteractableAreasManager.SetUp();
            GameManager.Instance.CollectableManager.SetUp();

            GameManager.Instance.ShopsManager.SetUp();
            GameManager.Instance.QuestsManager.SetUp();
            GameManager.Instance.BulletsManager.SetUp();

            GameManager.Instance.InputManager.EnablePlayerInput();

            GameManager.Instance.OffScreenIndicator.SetUp();
            GameManager.Instance.PauseMenuManager.SetUp();
            GameManager.Instance.HUDController.SetUp();
            GameManager.Instance.EffectsUIController.SetUp();
        }

        public void OnUpdate()
        {
        }

        public void OnExit()
        {
            //GameManager.Instance.RoomsManager.SetUp();

            //GameManager.Instance.LevelGenerator.GenerateLevel();

            //GameManager.Instance.DestructiblesManager.SetUp();
            //GameManager.Instance.NotificationsManager.SetUp();
            //GameManager.Instance.InputManager.SetUp();
            //GameManager.Instance.DamagePopUpManager.SetUp();
            //GameManager.Instance.VisualEffectsManager.SetUp();

            //GameManager.Instance.PlayersManager.SetUp();
            //GameManager.Instance.EnemiesManager.SetUp();
            //GameManager.Instance.BossesManager.SetUp();

            //GameManager.Instance.PlayerSpawner.SpawnPlayers();

            //GameManager.Instance.CameraManager.SetUp(false);
            ////GameManager.Instance.ObstaclesVanisher.SetUp();
            //GameManager.Instance.InteractableAreasManager.SetUp();
            //GameManager.Instance.CollectableManager.SetUp();

            //GameManager.Instance.BossKeysManager.SetUp();
            //GameManager.Instance.ShopsManager.SetUp();
            //GameManager.Instance.QuestsManager.SetUp();

            //GameManager.Instance.InputManager.EnablePlayerInput();

            //GameManager.Instance.OffScreenIndicator.SetUp();
            //GameManager.Instance.PauseMenuManager.SetUp();
            GameManager.Instance.HUDController.Dispose();
            //GameManager.Instance.EffectsUIController.SetUp();
        }

        public void OnDispose()
        {
        }
    }
}
