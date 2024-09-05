using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using TankLike.MainMenu;
    using UI.MainMenu;
    using UnityEngine.SceneManagement;

    public class MainMenuSceneController : SceneController
    {
        [SerializeField] private MainMenuUIController _mainMenuUIController;
        [SerializeField] private MainMenuInputManager _inputManager;

        private const string BOOTSTRAP_SCENE = "S_Bootstrap";
        private const string MAIN_MENU_SCENE = "S_MainMenu";

        public override void SetUp()
        {
            // If the bootstrap scene is the active scene (the game started in the normal sequence), set up the scene
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            if (sceneName == BOOTSTRAP_SCENE)
            {
                Debug.Log("SETUP MAIN MENU SCENE");
                StartCoroutine(SetupRoutine());
            }
        }

        public override void StarterSetUp()
        {
            Debug.Log("STARTER SETUP MAIN MENU SCENE");
            StartCoroutine(SetupRoutine());
        }

        private IEnumerator SetupRoutine()
        {
            // Wait until the S_MainMenu scene is loaded, 
            Scene scene = SceneManager.GetSceneByName(MAIN_MENU_SCENE);

            while (!scene.isLoaded)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(MAIN_MENU_SCENE));

            SetUpManagers();
        }

        protected override void SetUpManagers()
        {
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);

            _mainMenuUIController.SetUp(_inputManager);
            _inputManager.SetUp(_mainMenuUIController);
        }
    }
}
