using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using Attributes;
    using System;
    using UnitControllers;
    using Utils;

    public class SkillTreeBranchBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class SkillTreeCellBuildData
        {
            public UpgradeTypes UpgradeType;
            public Direction Direction;
            public Sprite Icon;
            public Color Color;
            public Sprite OutlineIcon;
        }

        [SerializeField] private SkillTreeCell _cellPrefab;
        [SerializeField] private float _distanceBetweenCells;
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private int _cellsPerBranch;
        [SerializeField] private SkillTreeCell _centerCell;
        [SerializeField, InSelf] private SkillTreeHolder _skillTreeHolder;
        [SerializeField, InSelf] private SkillTreeAdditionalCellsGenerator _additionalCellsGenerator;

        private List<SkillTreeCell> _cells = new List<SkillTreeCell>();

        private Dictionary<Direction, Vector2> _directions = new Dictionary<Direction, Vector2>()
        {
            {Direction.Up, Vector2.up},
            {Direction.Down, Vector2.down},
            {Direction.Left, Vector2.left},
            {Direction.Right, Vector2.right},
        };

        [field: SerializeField] public List<SkillTreeCellBuildData> CellsData = new List<SkillTreeCellBuildData>();

        public List<SkillTreeCell> BuildBranches()
        {
            for (int i = 0; i < CellsData.Count; i++)
            {
                BuildBranch(CellsData[i]);
            }
        
            return _cells;
        }

        private void BuildBranch(SkillTreeCellBuildData data)
        {
            SkillTreeCell previousCell = _centerCell;
            List<SkillTreeCell> cells = new List<SkillTreeCell>();
            Direction directionTag = data.Direction;

            for (int i = 0; i < _cellsPerBranch; i++)
            {
                SkillTreeCell cell = Instantiate(_cellPrefab, _cellsParent);
                cells.Add(cell);
                cell.UpgradeType = data.UpgradeType;
                cell.RectTransform.localPosition = previousCell.RectTransform.localPosition.Vector2() + _directions[data.Direction] * _distanceBetweenCells;
                cell.SetSkillPointCost(1);
                cell.HoldValue = 1.25f;
                cell.SetUpLockIcon(data.Icon);
                cell.SetOutline(data.OutlineIcon);
                cell.SetState(CellState.Unavailable);

                previousCell.AddNextCell(cell);
                _skillTreeHolder.CreateLineBetweenCells(previousCell, cell);

                previousCell.AddConnectedCell(directionTag, cell);
                cell.AddConnectedCell(Helper.GetOppositeDirection(directionTag), previousCell);

                previousCell = cell;
            }

            cells[0].SetState(CellState.Locked);
            _cells.AddRange(cells);

            switch(data.Direction)
            {
                case Direction.Left:
                    _additionalCellsGenerator.SetLeftBranch(cells);
                    break;
                case Direction.Right:
                    _additionalCellsGenerator.SetRightBranch(cells);
                    break;
                case Direction.Up:
                    _additionalCellsGenerator.SetUpBranch(cells);
                    break;
                case Direction.Down:
                    _additionalCellsGenerator.SetDownBranch(cells);
                    break;
            }
        }

        internal float GetCellsDistance()
        {
            return _distanceBetweenCells;
        }
    }
}
