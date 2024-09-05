using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class AbilitySelectionSceneController : SceneController
    {
        private const string MAIN_MENU_SCENE = "S_MainMenu";
        private const string ABILITY_SELECTION_SCENE = "S_AbilitySelection";

        public override void SetUp()
        {
            // If the S_MainMeny scene is the active scene (the game started in the normal sequence), set up the scene
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            if (sceneName == MAIN_MENU_SCENE)
            {
                Debug.Log("SETUP ABILITY SELECTION SCENE");
                StartCoroutine(SetupRoutine(ABILITY_SELECTION_SCENE));
            }
        }

        public override void StarterSetUp()
        {
            Debug.Log("STARTER SETUP ABILITY SELECTION SCENE");
            StartCoroutine(SetupRoutine(ABILITY_SELECTION_SCENE));
        }

        protected override void SetUpManagers()
        {
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);
        }
    }
}
