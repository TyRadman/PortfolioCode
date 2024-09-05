using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.Environment;
using UnityEngine;

namespace TankLike.UI.Map
{
    /// <summary>
    /// Responsible for drawing the map icons for the rooms
    /// </summary>
    public class LevelMapDisplayer : MonoBehaviour
    {
        [SerializeField] private GameObject _roomIconPrefab;
        [SerializeField] private GameObject _gateIconPrefab;
        [SerializeField] private GameObject _questionMarkPrefab;
        [SerializeField] private GameObject _bossGateIcon;
        [SerializeField] private GameObject _shopIconPrefab;
        [SerializeField] private GameObject _workshopIconPrefab;
        [SerializeField] private Transform _iconsParent;
        [SerializeField] private List<RoomIconData> _icons = new List<RoomIconData>();
        [SerializeField] private RectTransform _playerIcon;

        private int _roomSpacing = 0;
        private Vector2Int _levelCenterPoint;

        public void CreateLevelMap(Room[,] roomsGrid)
        {
            _levelCenterPoint = Vector2Int.zero;
            List<Room> rooms = new List<Room>();

            // cache rooms in a list
            for (int i = 0; i < roomsGrid.GetLength(0); i++)
            {
                for (int j = 0; j < roomsGrid.GetLength(1); j++)
                {
                    Room room = roomsGrid[i, j];

                    if (room != null)
                    {
                        rooms.Add(room);
                        _levelCenterPoint += room.Location;
                    }
                }
            }

            _levelCenterPoint /= rooms.Count;
            //Room centerRoom = rooms.Find(r => r.Location == center);
            _roomSpacing = (int)(_roomIconPrefab.GetComponent<RectTransform>().rect.width +
                _gateIconPrefab.GetComponent<RectTransform>().rect.height);

            // draw the rooms
            for (int i = 0; i < rooms.Count; i++)
            {
                GenerateRoom(rooms[i]);
            }

            // enable and reveal the active room where the player starts
            Room startRoom = GameManager.Instance.RoomsManager.CurrentRoom;
            _icons.Find(i => i.Room == startRoom).Reveal();

            // set the player's icon as the last child so that it shows in the UI
            _playerIcon.SetAsLastSibling();
        }

        private void GenerateRoom(Room room)
        {
            RoomIconData data = new RoomIconData();
            _icons.Add(data);
            data.Room = room;

            RectTransform icon = Instantiate(_roomIconPrefab).GetComponent<RectTransform>();
            data.RoomIcon = icon.gameObject;
            icon.parent = _iconsParent;
            Vector2Int position = (room.Location - _levelCenterPoint) * _roomSpacing;
            icon.localPosition = new Vector3(position.x, position.y, 0f);
            Transform questionMark = Instantiate(_questionMarkPrefab, _iconsParent).transform;
            data.QuestionMarkIcon = questionMark.gameObject;
            questionMark.parent = icon;
            questionMark.localPosition = Vector3.zero;

            if (room.RoomType == RoomType.Shop)
            {
                Transform shopIcon = Instantiate(_shopIconPrefab, _iconsParent).transform;
                //data.QuestionMarkIcon = questionMark.gameObject;
                shopIcon.parent = icon;
                shopIcon.localPosition = Vector3.zero;
            }
            else if (room.RoomType == RoomType.Workshop)
            {
                Transform workshopIcon = Instantiate(_workshopIconPrefab, _iconsParent).transform;
                //data.QuestionMarkIcon = questionMark.gameObject;
                workshopIcon.parent = icon;
                workshopIcon.localPosition = Vector3.zero;
            }

            // draw the gates
            List<GateInfo> gates = room.GatesInfo.Gates.FindAll(g => g.IsConnected);

            for (int j = 0; j < gates.Count; j++)
            {
                RectTransform gateIcon;

                if (room.RoomType == RoomType.BossGate && gates[j].Direction == GateDirection.North)
                {
                    gateIcon = Instantiate(_bossGateIcon).GetComponent<RectTransform>();
                    gateIcon.parent = icon;
                    gateIcon.localPosition = Vector3.zero;
                    gateIcon.eulerAngles += Vector3.forward * ((int)gates[j].Direction - 90);
                    gateIcon.localPosition += gateIcon.up * gateIcon.rect.height / 2;
                    gateIcon.parent = _iconsParent;
                    gateIcon.SetAsLastSibling();
                }
                else
                {
                    gateIcon = Instantiate(_gateIconPrefab).GetComponent<RectTransform>();
                    gateIcon.parent = icon;
                    gateIcon.localPosition = Vector3.zero;
                    gateIcon.eulerAngles += Vector3.forward * ((int)gates[j].Direction - 90);
                    gateIcon.localPosition += gateIcon.up * gateIcon.rect.height;
                }

                data.GateIcons.Add(gateIcon.gameObject);
            }

            data.SetUp();

            GameManager.Instance.RoomsManager.OnRoomEntered += RevealRoom;
        }

        /// <summary>
        /// Called by every room when the player enters it
        /// </summary>
        public void RevealRoom(Room room)
        {
            // cache the icon corresponding to the active room
            RoomIconData roomIcon = _icons.Find(i => i.Room == room);

            // if the room has already been revealed, then return
            if (roomIcon.IsRevealed)
            {
                return;
            }

            roomIcon.Reveal();
        }

        public void OnMapOpened()
        {
            // position the player icon where they currently are
            _playerIcon.localPosition =
                _icons.Find(i => i.Room == GameManager.Instance.RoomsManager.CurrentRoom).
                GetRoomIconLocalPosition();
        }
    }
}
