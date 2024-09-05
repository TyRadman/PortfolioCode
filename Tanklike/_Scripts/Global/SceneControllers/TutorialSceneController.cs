using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using TankLike.Tutorial;
using UnityEngine;

namespace TankLike
{
    public class TutorialSceneController : SceneController
    {
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private NormalRoom _tutorialRoom;

        private const string TUTORIAL_SCENE = "S_Tutorial";

        public override void SetUp()
        {
        }

        public override void StarterSetUp()
        {
            Debug.Log("STARTER SETUP ABILITY SELECTION SCENE");
            StartCoroutine(SetupRoutine(TUTORIAL_SCENE));
        }

        protected override void SetUpManagers()
        {
            // Set bossRoom as the current room
            GameManager.Instance.RoomsManager.SetCurrentRoom(_tutorialRoom);

            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);

            // Enable the main menu screen only
            GameManager.Instance.ResultsUIController.gameObject.SetActive(true);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(true);
            GameManager.Instance.HUDController.gameObject.SetActive(true);

            GameManager.Instance.DestructiblesManager.SetUp();
            GameManager.Instance.NotificationsManager.SetUp();
            GameManager.Instance.InputManager.SetUp();
            GameManager.Instance.DamagePopUpManager.SetUp();
            GameManager.Instance.VisualEffectsManager.SetUp();
            GameManager.Instance.BossKeysManager.SetUp();

            GameManager.Instance.PlayersManager.SetUp();
            GameManager.Instance.EnemiesManager.EnableSpawnEnemies(false);
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

            _tutorialManager.SetUp();
        }
    }
}
