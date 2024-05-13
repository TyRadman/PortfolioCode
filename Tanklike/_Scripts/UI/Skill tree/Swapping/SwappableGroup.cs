using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UnitControllers;
using UnityEngine;
using static TankLike.UI.SkillTree.SwappablesNavigatable;

namespace TankLike.UI.SkillTree
{
    public class SwappableGroup : GroupSelectableCell
    {
        [field: SerializeField] public SwappableCell Cell;
        public CellType CellType;
        public SwappableSkillsSelector SkillsSelector;
        //[field: SerializeField] public SwappablesNavigatable.CellType SelectedCellType { get; private set; }

        public override void HighLight(bool highlight)
        {
            Cell.HighLight(highlight);

            // text pulse animation maybe?
        }

        public SwappableGroup GetNeighborCell(Direction direction)
        {
            return Cell.NextCellInputs.Find(c => c.CellDirection == direction).Cell as SwappableGroup;
        }
    }
}
