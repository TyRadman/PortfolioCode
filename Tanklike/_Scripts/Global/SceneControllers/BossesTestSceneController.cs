using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Environment;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class BossesTestSceneController : SceneController
    {
        [SerializeField] private BossRoom _bossRoom;
        [SerializeField] private CameraLimits _cameraLimits;

        private const string BOSSES_SCENE = "S_Bosses";

        public override void SetUp()
        {
        }

        public override void StarterSetUp()
        {
            Debug.Log("STARTER SETUP BOSSES SCENE");
            StartCoroutine(SetupRoutine());
        }

        private IEnumerator SetupRoutine()
        {
            // Wait until the S_Bosses scene is loaded, 
            Scene scene = SceneManager.GetSceneByName(BOSSES_SCENE);

            while (!scene.isLoaded)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(BOSSES_SCENE));

            SetUpManagers();
        }

        protected override void SetUpManagers()
        {
            // Set bossRoom as the current room
            GameManager.Instance.RoomsManager.SetCurrentRoom(_bossRoom);

            // Set camera limits
            //GameManager.Instance.CameraManager.SetCamerasLimits(_cameraLimits);

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
    }
}
