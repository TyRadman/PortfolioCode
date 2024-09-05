using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class GameplaySceneController : SceneController
    {
        private const string LOBBY_SCENE = "S_Lobby";
        private const string GAMEPLAY_SCENE = "S_Gameplay";

        public override void SetUp()
        {
            // If the S_LOBBY scene is the active scene (the game started in the normal sequence), set up the scene
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            if (sceneName == LOBBY_SCENE)
            {
                Debug.Log("SETUP GAMEPLAY SCENE");
                StartCoroutine(SetupRoutine());
            }
        }

        public override void StarterSetUp()
        {
            Debug.Log("STARTER SETUP GAMEPLAY SCENE");
            StartCoroutine(SetupRoutine());
        }

        private IEnumerator SetupRoutine()
        {
            // Wait until the S_Gameplay scene is loaded, 
            Scene scene = SceneManager.GetSceneByName(GAMEPLAY_SCENE);

            while (!scene.isLoaded)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GAMEPLAY_SCENE));

            SetUpManagers();
        }

        protected override void SetUpManagers()
        {
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);

            // Enable the main menu screen only
            GameManager.Instance.ResultsUIController.gameObject.SetActive(true);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(true);
            GameManager.Instance.HUDController.gameObject.SetActive(true);

            GameManager.Instance.RoomsManager.SetUp();

            GameManager.Instance.BossKeysManager.SetUp();
            GameManager.Instance.LevelGenerator.GenerateLevel();

            GameManager.Instance.DestructiblesManager.SetUp();
            GameManager.Instance.NotificationsManager.SetUp();
            GameManager.Instance.InputManager.SetUp();
            GameManager.Instance.DamagePopUpManager.SetUp();
            GameManager.Instance.VisualEffectsManager.SetUp();

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
    }
}
