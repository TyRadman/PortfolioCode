using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine.AI;
using TankLike.Cam;
using TankLike.Sound;

namespace TankLike.Environment
{
    public class Room : MonoBehaviour
    {
        public Vector2Int Location;
        [field: SerializeField] public RoomGatesInfo GatesInfo { get; protected set; }
        [field: SerializeField] public RoomType RoomType { get; protected set; }
        [field: SerializeField] public RoomUnitSpawner Spawner { get; protected set; }
        [field: SerializeField] public Transform SpawnablesParent;
        [SerializeField] protected NavMeshSurface _surface;
        [Header("Settings")]
        [SerializeField] protected Audio _openGateAudios;
        [SerializeField] protected Audio _closeGateAudios;
        [field: SerializeField] public CameraLimits CameraLimits { get; protected set; }
        private List<Transform> _droppers = new List<Transform>();
        public Vector2Int RoomDimensions;

        public virtual void SetUpRoom()
        {

        }

        public virtual void LoadRoom()
        {
            // display the room
            gameObject.SetActive(true);
            GatesInfo.Gates.ForEach(g => g.Gate.EnableGate());
        }

        public void DisableGates()
        {
            GatesInfo.Gates.ForEach(g => g.Gate.DisableGate());
        }

        public virtual void UnloadRoom()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnRoomEnteredHandler()
        {

        }

        public void OpenGates()
        {
            GatesInfo.Gates.ForEach(g => g.Gate.OpenGate());
        }

        public void CloseGates()
        {
            GatesInfo.Gates.ForEach(g => g.Gate.CloseGate());
        }

        public void PlayOpenGateAudio()
        {
            GameManager.Instance.AudioManager.Play(_openGateAudios);
        }

        public void BakeRoom()
        {
            _surface.BuildNavMesh();
        }

        public void SetRoomType(RoomType type)
        {
            RoomType = type;
        }

        public void SetCameraLimits(CameraLimits limits)
        {
            // set the edge points of the room as the limits
            CameraLimits = new CameraLimits();
            CameraLimits.HorizontalLimits = limits.HorizontalLimits;
            CameraLimits.VerticalLimits = limits.VerticalLimits;
        }

        public void AddDropper(Transform dropper)
        {
            _droppers.Add(dropper);
        }

        public void RemoveDropper(Transform dropper)
        {
            if (_droppers.Exists(d => d == dropper))
            {
                _droppers.Remove(dropper);
            }
        }

        public List<Transform> GetDroppers()
        {
            if (_droppers.Exists(d => d == null))
            {
                _droppers.RemoveAll(d => d == null);
            }

            return _droppers;
        }

        public Vector3 GetRandomSpawnPoint()
        {
            return Spawner.SpawnPoints.GetRandomSpawnPoint(false).position;
        }

        public Vector3 GetClosestSpawnPointToPosition(Vector3 position, int order)
        {
            return Spawner.SpawnPoints.GetClosestSpawnPointToPosition(position, order).position;
        }
    }
}
