using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class BossSceneManager : GameManager
    {
        protected override void Start()
        {
            Debug.Log("Setting up Boss Scene Manager");

            NotificationsManager.SetUp();
            AudioManager.SetUp();
            InputManager.SetUp();
            PauseMenuManager.SetUp();
            DamagePopUpManager.SetUp();
            PlayerSpawner.SpawnPlayers();
            
            CameraManager.SetUp(false);
            ObstaclesVanisher.SetUp();
            PoolingManager.Setup(_spawnablesParent);
            InteractableAreasManager.SetUp();
            CollectableManager.SetUp();
            VisualEffectsManager.SetUp(BulletsDatabase);

            BossesManager.SetUp(BossesDatabase);

            BossKeysManager.SetUp();
            ShopsManager.SetUp();
            QuestsManager.SetUp();

            InputManager.EnablePlayerInput(true);
            OffScreenIndicator.SetUp();

            RoomsManager.LoadBossRoom();
        }
    }
}
