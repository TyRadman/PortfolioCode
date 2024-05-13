using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Inventory
{
    [System.Serializable]
    public class TabReferenceUI
    {
        public string TabName;
        public TabUI Tab;
        public GameObject Content;
        public Navigatable Navigator;
        public bool CanMoveToContent = true;
    }
}
