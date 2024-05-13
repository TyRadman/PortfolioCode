using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.SkillTree
{
    public class SwappableCell : SelectableVisualizedCellUI
    {
        public enum PlayerSelectionState
        {
            Active = 0, NotActive = 1
        }

        public enum HighlightState
        {
            Highlighted = 0, NotHighlighted = 1
        }

        [field: SerializeField] public PlayerSelectionState CellState { set; get; }
        [field: SerializeField] public HighlightState CellHighlightState { get; private set; }
        [field: SerializeField] public Skill Skill { get; private set; }
        [SerializeField] private Image _activeCellImage;

        public void SetSkill(Skill skill)
        {
            Skill = skill;
            _iconImage.sprite = skill.GetIcon();
        }

        public void SetAsPlayerActiveSkill(bool active)
        {
            CellState = active ? PlayerSelectionState.Active : PlayerSelectionState.NotActive;
            _activeCellImage.enabled = active;
        }

        public override void HighLight(bool highlight)
        {
            CellHighlightState = highlight? HighlightState.Highlighted : HighlightState.NotHighlighted;
            _highlightImage.enabled = highlight;
        }

        public SwappableCell GetNeighborCell(Direction direction)
        {
            return NextCellInputs.Find(c => c.CellDirection == direction).Cell as SwappableCell;
        }
    }
}
