using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Inventory
{
    public class ContentToTabConnector : SelectableCell
    {
        [SerializeField] private TabsManager _tabs;

        public override void MoveSelection(Direction direction, ref SelectableEntityUI cell, int playerIndex = 0)
        {
            //_tabs.MoveToTabHead();
        }
    }
}
