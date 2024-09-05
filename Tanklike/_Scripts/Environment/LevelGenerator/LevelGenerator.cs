using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.MapMaker;
using TankLike.Utils;
using System.Linq;
using UnityEngine;
using TankLike.Cam;
using TankLike.LevelGeneration;

namespace TankLike.Environment.LevelGeneration
{
    public class LevelGenerator : MonoBehaviour
    {
        [field: SerializeField] public RoomsBuilder RoomsBuilder { get; private set; }
        [field: SerializeField] public ShopsBuilder ShopsBuilder { get; private set; }
        [Header("Editor")]
        public MapTiles_SO MapToBuild;
        public LevelData LevelData;
        public NormalRoom RoomReference;
        public BuildConfigs Configs;
        public LevelType LevelType;


        public void GenerateLevel()
        {
            if (LevelType == LevelType.Random)
            {
                GenerateRandomLevel();
            }
            else
            {
                GenerateCustomLevel();
            }
        }

        private void GenerateCustomLevel()
        {
            //GameManager.Instance.LevelMap.CreateLevelMap(RoomsBuilder.GetRoomsGrid());
            //GameManager.Instance.RoomsManager.SetCurrentRoom(tempRoomToStartFrom
            CameraLimits limits = new CameraLimits();
            limits.SetValues(GameManager.Instance.RoomsManager.CurrentRoom.CameraLimits);
            limits.AddOffset(GameManager.Instance.RoomsManager.CurrentRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
        }

        private void GenerateRandomLevel()
        {
            RoomsBuilder.SetUp();
            RoomsBuilder.BuildRandomRooms();
            GameManager.Instance.LevelMap.CreateLevelMap(RoomsBuilder.GetRoomsGrid());
            ShopsBuilder.BuildShops();
            Room startingRoom = GameManager.Instance.RoomsManager.CurrentRoom;

            // set first room camera limits
            CameraLimits limits = new CameraLimits();
            limits.SetValues(startingRoom.CameraLimits);
            limits.AddOffset(startingRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
            Room currentRoom = GameManager.Instance.RoomsManager.CurrentRoom;
            GameManager.Instance.MinimapManager.PositionMinimapAtRoom(currentRoom.transform, currentRoom.RoomDimensions);

        }

        public void EditorGenerateLevel()
        {
            RoomsBuilder.BuildRandomRooms();
            ShopsBuilder.BuildShops();
        }
    }
}