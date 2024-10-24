using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Environment;
using TankLike.UI;
using UnityEngine;

namespace TankLike
{
    public class RoomsManager : MonoBehaviour, IManager
    {
        [field: SerializeField] public List<Room> Rooms { get; private set; }
        [field: SerializeField] public Room CurrentRoom { get; private set; }
        public System.Action<Room> OnRoomEntered;
        private const float SWITCH_ROOMS_DURATION = 0.5f;

        [Header("References")]
        [SerializeField] private GameObject _roomsCoverPrefab;


        public bool IsActive { get; private set; }
        public List<Room> SpecialVisitedRooms { get; private set; } = new List<Room>();

        private GameObject _roomsCover;


        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _roomsCover = Instantiate(_roomsCoverPrefab);
        }
        public void Dispose()
        {
            IsActive = false;

            _roomsCover = null;
        }
        #endregion

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }

        public void SetCurrentRoom(Room room)
        {
            CurrentRoom = room;

            if(_roomsCover != null)
            {
                _roomsCover.transform.position = room.transform.position;
            }

            GameManager.Instance.TeleportationManager.SetDestinationRoom(room);
        }

        public void SwitchRoom(Room nextRoom, RoomGate enterGate = null)
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

            GameManager.Instance.FadeUIController.StartFadeIn();
            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeInDuration);

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
            SetCurrentRoom(nextRoom);
            CurrentRoom.LoadRoom();

            // disable camera interpolation so that the players don't see our level's guts
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(false);
            GameManager.Instance.OffScreenIndicator.Enable(false);

            // Respawn players
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                Transform point = nextRoom.Spawner.SpawnPoints.GetRandomSpawnPoint(); 
                
                if(enterGate != null)
                {
                    point = enterGate.StartPoints[i];
                }

                Vector3 position = point.position;
                position.y = 1f;
                Quaternion rotation = Quaternion.LookRotation(point.forward);
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
            GameManager.Instance.FadeUIController.StartFadeOut();
            GameManager.Instance.InputManager.EnablePlayerInput();
            OnRoomEntered?.Invoke(CurrentRoom);
            // change the camera constraints
            CameraLimits limits = new CameraLimits();
            limits.SetValues(nextRoom.CameraLimits);
            limits.AddOffset(nextRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeOutDuration);
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(true);
            GameManager.Instance.OffScreenIndicator.Enable(true);
        }

        public void TeleportToRoom(Room nextRoom, RoomGate enterGate = null)
        {
            StartCoroutine(TeleportToRoomRoutine(nextRoom, enterGate));
        }

        private IEnumerator TeleportToRoomRoutine(Room nextRoom, RoomGate enterGate)
        {
            // disable the gates first to avoid having this be triggered by the second player before the room unloads
            CurrentRoom.DisableGates();

            GameManager.Instance.FadeUIController.StartFadeIn();
            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeInDuration);

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
            SetCurrentRoom(nextRoom);
            CurrentRoom.LoadRoom();

            // disable camera interpolation so that the players don't see our level's guts
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(false);
            GameManager.Instance.OffScreenIndicator.Enable(false);

            // Respawn players
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                Transform point = nextRoom.Spawner.SpawnPoints.GetRandomSpawnPoint();

                if (enterGate != null)
                {
                    point = enterGate.StartPoints[i];
                }

                Vector3 position = point.position;
                position.y = 1f;
                Quaternion rotation = Quaternion.LookRotation(point.forward);
                GameManager.Instance.PlayersManager.GetPlayer(i).transform.SetPositionAndRotation(position, Quaternion.identity);
                ((UnitControllers.PlayerMovement)GameManager.Instance.PlayersManager.GetPlayer(i).Movement).SetBodyRotation(rotation);
            }

            yield return new WaitForSeconds(SWITCH_ROOMS_DURATION);
            GameManager.Instance.FadeUIController.StartFadeOut();
            OnRoomEntered?.Invoke(CurrentRoom);
            // change the camera constraints
            CameraLimits limits = new CameraLimits();
            limits.SetValues(nextRoom.CameraLimits);
            limits.AddOffset(nextRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeOutDuration);
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(true);
            GameManager.Instance.OffScreenIndicator.Enable(true);
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
