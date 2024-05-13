using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    public class GroupSelectableCell : SelectableEntityUI
    {
        [field: SerializeField] public SelectableEntityUI LastSelectableCell { get; private set; }
        [SerializeField] private bool _storeLastSelectedCell = false;

        public override void HighLight(bool highlight)
        {
            LastSelectableCell.HighLight(highlight);
        }

        public override void MoveSelection(Direction direction, ref SelectableEntityUI cell, int playerIndex = 0)
        {
            if (LastSelectableCell.Type == SelectableType.Cell)
            {
                LastSelectableCell.HighLight(true);
                cell = LastSelectableCell;
            }
            else LastSelectableCell.MoveSelection(direction, ref cell, playerIndex);
        }

        public void SetLastCell(SelectableCell cell)
        {
            if (!_storeLastSelectedCell) return;

            LastSelectableCell = cell;
        }
    }
}
