using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Sound;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.Environment
{
    public class NormalRoom : Room
    {
        [field: SerializeField, Header("Settings")] public int SpawningCapacity { get; private set; }
        [field: SerializeField] public bool HasEnemies { get; private set; } = true;
        public bool SpawnedEnemies = false;

        public override void SetUpRoom()
        {
            base.SetUpRoom();

            foreach (GateInfo gate in GatesInfo.Gates)
            {
                gate.Gate.Setup(HasEnemies, this);
            }
        }

        public override void LoadRoom()
        {
            base.LoadRoom();
        }

        public override void UnloadRoom()
        {
            base.UnloadRoom();
        }

        public override void OnRoomEnteredHandler()
        {
            base.OnRoomEnteredHandler();

            if (!Spawner.HasEnemies())
            {
                return;
            }

            // Close every gate in the room
            foreach (var gate in GatesInfo.Gates)
            {
                //gate.Gate.OnRoomEntered -= OnRoomEnteredHandler;
                gate.Gate.CloseGate();
            }

            if (Spawner.CanHaveKeys())
            {
                Spawner.AddKeyHolder();
            }

            Spawner.ActivateSpawnedEnemies();
            GameManager.Instance.AudioManager.Play(_closeGateAudios);
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();
            GameManager.Instance.EnemiesManager.SetFightActivated(true);
            //GameManager.Instance.RoomsManager.SetCurrentRoom(this);

        }

        public void SetHasEnemies(bool hasEnemies)
        {
            HasEnemies = hasEnemies;
        }
    }
}
