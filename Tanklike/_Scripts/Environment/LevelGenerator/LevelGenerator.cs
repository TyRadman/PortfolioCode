using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.MapMaker;
using TankLike.Utils;
using System.Linq;
using UnityEngine;
using TankLike.Cam;

namespace TankLike.Environment.LevelGeneration
{
    public class LevelGenerator : MonoBehaviour
    {
        [field: SerializeField] public RoomsBuilder RoomsBuilder { get; private set; }
        [field: SerializeField] public ShopsBuilder ShopsBuilder { get; private set; }

        public void GenerateLevel()
        {
            RoomsBuilder.BuildRooms();
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
            RoomsBuilder.BuildRooms();
            ShopsBuilder.BuildShops();
        }
    }
}