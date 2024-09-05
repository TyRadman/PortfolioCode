using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class LobbySceneController : SceneController
    {
        [SerializeField] private PlayerJoinManager _playerJoinManager;

        private const string ABILITY_SELECTION_SCENE = "S_AbilitySelection";
        private const string LOBBY_SCENE = "S_Lobby";

        public override void SetUp()
        {
            // If the S_AbilitySelection scene is the active scene (the game started in the normal sequence), set up the scene
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            if (sceneName == ABILITY_SELECTION_SCENE)
            {
                Debug.Log("SETUP LOBBY SCENE");
                StartCoroutine(SetupRoutine());
            }
        }

        public override void StarterSetUp()
        {
            Debug.Log("STARTER SETUP LOBBY SCENE");
            StartCoroutine(SetupRoutine());
        }

        private IEnumerator SetupRoutine()
        {
            // Wait until the S_Lobby scene is loaded, 
            Scene scene = SceneManager.GetSceneByName(LOBBY_SCENE);

            while (!scene.isLoaded)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(LOBBY_SCENE));

            SetUpManagers();
        }

        protected override void SetUpManagers()
        {
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);

            _playerJoinManager.SetUp();
        }
    }
}
