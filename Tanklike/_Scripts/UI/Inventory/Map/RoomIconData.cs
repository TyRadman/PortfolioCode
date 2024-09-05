using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using UnityEngine;

namespace TankLike.UI.Map
{
    public class RoomIconData
    {
        public Room Room;
        public List<GameObject> GateIcons = new List<GameObject>();
        public GameObject RoomIcon;
        public GameObject QuestionMarkIcon;
        public bool IsRevealed = false;

        public void SetUp()
        {
            GateIcons.ForEach(i => i.SetActive(false));
            RoomIcon.SetActive(false);
            QuestionMarkIcon.SetActive(false);
        }

        public void Reveal()
        {
            IsRevealed = true;
            GateIcons.ForEach(i => i.SetActive(true));
            RoomIcon.SetActive(true);
        }

        public Vector3 GetRoomIconLocalPosition()
        {
            return RoomIcon.GetComponent<RectTransform>().localPosition;
        }
    }
}
