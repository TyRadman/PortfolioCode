using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Environment;
using TankLike.UI;
using UnityEngine;

namespace TankLike
{
    public class RoomsManager : MonoBehaviour
    {
        [field: SerializeField] public List<Room> Rooms { get; private set; }
        [field: SerializeField] public Room CurrentRoom { get; private set; }
        public System.Action<Room> OnRoomEntered;
        private const float SWITCH_ROOMS_DURATION = 0.5f;

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }

        public void SetCurrentRoom(Room room)
        {
            CurrentRoom = room;
        }

        public void SwitchRoom(Room nextRoom, RoomGate enterGate)
        {
            StartCoroutine(SwitchRoomRoutine(nextRoom, enterGate));

            if (nextRoom.RoomType == RoomType.Boss)
            {

            }
            else
            {
                NormalRoom nextNormalRoom = (NormalRoom)nextRoom;

                if (nextNormalRoom.HasEnemies && !nextNormalRoom.SpawnedEnemies)
                {
                    GameManager.Instance.EnemiesManager.GetEnemies(nextNormalRoom);
                }
            }
        }

        public void SetupRooms()
        {
            Rooms.ForEach(r => r.SetUpRoom());
        }

        private IEnumerator SwitchRoomRoutine(Room nextRoom, RoomGate enterGate)
        {
            // disable the gates first to avoid having this be triggered by the second player before the room unloads
            CurrentRoom.DisableGates();

            GameManager.Instance.EffectsUIController.FadeUIController.StartFadeIn();
            yield return new WaitForSeconds(GameManager.Instance.EffectsUIController.FadeUIController.FadeInDuration);

            // position the minimap at the top of the room
            GameManager.Instance.MinimapManager.PositionMinimapAtRoom(nextRoom.transform, nextRoom.RoomDimensions);

            // Deactivate players
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                GameManager.Instance.PlayersManager.GetPlayer(i).Deactivate();
            }

            // Deactivate summons
            for (int i = 0; i < GameManager.Instance.SummonsManager.GetActiveSummonsCount(); i++)
            {
                GameManager.Instance.SummonsManager.GetSummon(i).Deactivate();
            }

            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.BulletsManager.DeactivateBullets();

            CurrentRoom.UnloadRoom();
            CurrentRoom = nextRoom;
            CurrentRoom.LoadRoom();

            // disable camera interpolation so that the players don't see our level's guts
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(false);

            // Respawn players
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                Vector3 position = enterGate.StartPoints[i].position;
                position.y = 1f;
                Quaternion rotation = Quaternion.LookRotation(enterGate.StartPoints[i].forward);
                GameManager.Instance.PlayersManager.GetPlayer(i).transform.SetPositionAndRotation(position, Quaternion.identity);
                ((UnitControllers.PlayerMovement)GameManager.Instance.PlayersManager.GetPlayer(i).Movement).SetBodyRotation(rotation);
            }

            // Reactivate players
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                GameManager.Instance.PlayersManager.GetPlayer(i).Activate();
            }

            // Reactivate summons
            for (int i = 0; i < GameManager.Instance.SummonsManager.GetActiveSummonsCount(); i++)
            {
                GameManager.Instance.SummonsManager.GetSummon(i).Activate();
            }

            yield return new WaitForSeconds(SWITCH_ROOMS_DURATION);
            GameManager.Instance.EffectsUIController.FadeUIController.StartFadeOut();
            GameManager.Instance.InputManager.EnablePlayerInput(true);
            OnRoomEntered?.Invoke(CurrentRoom);
            // change the camera constraints
            CameraLimits limits = new CameraLimits();
            limits.SetValues(nextRoom.CameraLimits);
            limits.AddOffset(nextRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
            yield return new WaitForSeconds(GameManager.Instance.EffectsUIController.FadeUIController.FadeOutDuration);
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(true);
        }

        public void OpenAllRooms()
        {
            Rooms.ForEach(r => r.OpenGates());
        }

        // Used for the testing scene only
        public void LoadBossRoom()
        {
            ((BossRoom)CurrentRoom).SetBossData(GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().BossData);
            ((BossRoom)CurrentRoom).SetBossSpawnPoint(CurrentRoom.transform);
            (CurrentRoom).LoadRoom();
            (CurrentRoom).OnRoomEnteredHandler();
        }
    }

    public enum RoomType
    {
        Normal = 0, Start = 1, Boss = 2, Shop = 3, SecretChallenge = 4, SecretShop = 5, BossGate = 6, Workshop = 7,
    }
}
