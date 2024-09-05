using System.Collections;
using System.Collections.Generic;
using TankLike.Minimap;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Environment
{
    public class RoomGate : MonoBehaviour
    {
        public int GateID { set; get; } = -1;
        [field: SerializeField] public Room ConnectedRoom { get; private set; }
        protected Room _parentRoom;
        [field: SerializeField] public RoomGate ConnectedGate { get; private set; }

        [SerializeField] public GameObject _gateVisuals;

        [Tooltip("Triggers what happens when the player leaves the room through his gate.")]
        [SerializeField] protected RoomTrigger _newRoomEnterTrigger;
        [Tooltip("Triggers what happens when the player fully enters the room of this gate.")]
        [SerializeField] protected RoomTrigger _gateExitTrigger;

        [SerializeField] protected Transform[] _startPoints;
        [SerializeField] protected Animator _gateAnimator;
        [SerializeField] protected GameObject _barrier;

        [Header("Minimap")]
        [SerializeField] protected MinimapIcon _minimapIcon;
        [SerializeField] protected Material _openGateIconMaterial;
        [SerializeField] protected Material _closedGateIconMaterial;
        [field: SerializeField] public GateDirection Direction { set; get; }
        public Transform[] StartPoints => _startPoints;
        private bool _isActive = true;

        public virtual void Setup(bool roomHasEnemies, Room parentRoom)
        {
            _parentRoom = parentRoom;
            _gateVisuals.gameObject.SetActive(roomHasEnemies);

            if (_gateAnimator != null)
            {
                _gateAnimator.gameObject.SetActive(roomHasEnemies);
                _gateAnimator.Play("Idle");
            }

            if (_barrier != null)
            {
                _barrier.SetActive(false);
            }

            _minimapIcon.SwitchMeshMaterial(_openGateIconMaterial);
        }

        /// <summary>
        /// Mutual between all gates
        /// </summary>
        public void EnableGate()
        {
            _newRoomEnterTrigger.OnTriggerEnterEvent += SwitchRoom;
            _gateExitTrigger.OnTriggerEnterEvent += OnRoomEntered;
        }

        public void DisableGate()
        {
            _newRoomEnterTrigger.OnTriggerEnterEvent -= SwitchRoom;
            _gateExitTrigger.OnTriggerEnterEvent -= OnRoomEntered;
        }

        private void OnRoomEntered()
        {
            if (!_isActive)
            {
                return;
            }

            _parentRoom.OnRoomEnteredHandler();
        }

        protected void SwitchRoom()
        {
            GameManager.Instance.RoomsManager.SwitchRoom(ConnectedRoom, ConnectedGate);
        }

        #region Controllers
        public virtual void CloseGate()
        {
            _isActive = false;
            _barrier.SetActive(true);
            _gateAnimator.Play("Close");
            _gateExitTrigger.gameObject.SetActive(false);
            _minimapIcon.SwitchMeshMaterial(_closedGateIconMaterial);
        }

        public virtual void OpenGate()
        {
            _barrier.SetActive(false);
            _gateAnimator.Play("Open");
            _minimapIcon.SwitchMeshMaterial(_openGateIconMaterial);
        }
        #endregion

        public void SetConnection(Room room, RoomGate gate)
        {
            ConnectedRoom = room;
            ConnectedGate = gate;
        }
    }

    public enum GateDirection
    {
        East = 0,
        North = 90,
        West = 180,
        South = 270,
        None = 500
    }
}
