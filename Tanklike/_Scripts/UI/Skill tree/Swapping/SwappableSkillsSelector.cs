using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;
using TMPro;

namespace TankLike.UI.SkillTree
{
    public class SwappableSkillsSelector : Navigatable
    {
        [Header("References")]
        [SerializeField] private List<SwappableCell> _cells;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        private PlayerInputActions _playerinput;
        [SerializeField] private SwappableCell _selectedCell;
        [SerializeField] private SwappablesNavigatable _swappableNavigator;

        public override void SetUpInput()
        {
            //_playerinput = GameManager.Instance.InputManager.GetInputControls(0);

            //_playerinput.UI.Navigate_Left.performed += (context) => Navigate(Direction.Left, 0);
            //_playerinput.UI.Navigate_Right.performed += (context) => Navigate(Direction.Right, 0);
            //_playerinput.UI.Submit.performed += (context) => Select(0);
            //_playerinput.UI.Cancel.performed += (context) => Close();
            //_playerinput.UI.Disable();
        }

        #region Open and Close
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);

            _selectedCell = _cells.Find(c => c.CellState == SwappableCell.PlayerSelectionState.Active);
            _selectedCell.HighLight(true);
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsActive) return;

            base.Close(playerIndex);
            _swappableNavigator.Open();
            // change the active skill to the last selected one in the swappable navigatable
            _swappableNavigator.LoadCellWithSkill(_cells.Find(c => c.CellState == SwappableCell.PlayerSelectionState.Active).Skill);
            // dehighlight the selected skill
            _selectedCell.HighLight(false);
        }
        #endregion

        // loads the skills that the player has
        public void LoadSkillsIntoCells(List<Skill> skills, int selectedSkillIndex)
        {
            _cells.ForEach(c => c.gameObject.SetActive(false));

            for (int i = 0; i < skills.Count; i++)
            {
                // activates the cell
                _cells[i].gameObject.SetActive(true);
                // fills the cell with data
                _cells[i].SetSkill(skills[i]);

                // find the indices of the next and previous cells. We can't put the values in the inspector because we enable and disable cells
                int previousIndex = i - 1 < 0 ? skills.Count - 1 : i - 1;
                int nextIndex = (i + 1) % skills.Count;
                _cells[i].NextCellInputs.Find(c => c.CellDirection == Direction.Left).Cell = _cells[previousIndex];
                _cells[i].NextCellInputs.Find(c => c.CellDirection == Direction.Right).Cell = _cells[nextIndex];
            }

            _cells[selectedSkillIndex].SetAsPlayerActiveSkill(true);
        }

        public void SetEmpty()
        {
            // set it as empty
        }

        #region Input Methods
        public override void Navigate(Direction direction)
        {
            if (!IsActive || direction == Direction.Down || direction == Direction.Up) return;

            base.Navigate(direction);
            
            // dehighlight the previous cell
            _selectedCell.HighLight(false);
            // swap the cells
            _selectedCell = _selectedCell.GetNeighborCell(direction);
            // highlight the previous cell
            _selectedCell.HighLight(true);
        }

        public override void Select()
        {
            if (!IsActive) return;

            base.Select();

            // if we're selecting the already selected cell, then do nothing
            if (_selectedCell.CellState == SwappableCell.PlayerSelectionState.Active) return;

            // disable all the cells from being activated
            _cells.ForEach(c => c.SetAsPlayerActiveSkill(false));
            // set the selected cell as the active cell
            _selectedCell.SetAsPlayerActiveSkill(true);
            // change it for the player
            SetSkillForPlayer();
        }

        private void SetSkillForPlayer()
        {
            
        }
        #endregion
    }
}
