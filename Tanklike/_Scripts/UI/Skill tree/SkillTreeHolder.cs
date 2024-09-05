using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UI;
using TankLike.UI.Inventory;
using TankLike.UnitControllers;
using TankLike.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.SkillTree
{
    using UI.Workshop;

    public class SkillTreeHolder : Navigatable, IInput
    {
        #region Constants
        private const string EMPTY_NAME_TEXT = "Skill name";
        private const string EMPTY_DESCRIPTION_TEXT = "The skill description";
        private const string EMPTY_POINTS_REQUIRED_TEXT = "--";
        private const string RANDOM_NAME_TEXT = "Random Skill";
        private const string RANDOM_DESCRIPTION_TEXT = "Choose one of two random skills";
        private const string RANDOM_POINTS_REQUIRED_TEXT = "1";
        private const int RANDOM_SKILL_POINTS_REQUIRED_AMOUNT = 1;
        private const float SELECTION_HOLD_DURATION = 1f;
        private const float SELECTION_HOLD_EMPTYING_SPEED_MULTIPLIER = 4f;
        #endregion
        [SerializeField] private SkillTreeCell _activeSkillCell;
        [Header("Colors")]
        [SerializeField] private Color _unavailableCellColor;
        [SerializeField] private Color _lockedCellColor;
        [SerializeField] private Color _unlockedCellColor;
        [SerializeField] private SkillTreeCell _centerCell;
        [SerializeField] private List<SkillTreeCell> _cells;
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        [SerializeField] private TextMeshProUGUI _skillPointsRequiredText;
        [SerializeField] private TextMeshProUGUI _playerSkillPointsText;
        [SerializeField] private TextMeshProUGUI _skillStateText;
        [SerializeField] private SkillTreeRandomSkillMenuController _randomSkillsMenu;
        [SerializeField] private SkillTreeSkillsDatabase _mutualSkillsDatabase;
        [SerializeField] private SkillTreeSkillsDatabase _specialSkillsDatabase;
        [SerializeField] private List<SkillProfile> _randomSkills = new List<SkillProfile>();
        private WorkShopTabsNavigatable _workShopNavigatable;
        private SkillTreeCell _lastSelectedCell;
        private bool _canMove = false;
        [SerializeField] private float _coolDown = 0.1f;
        private PlayerComponents _currentPlayer;
        private bool _isHolding = false;

        private void Awake()
        {
            _randomSkillsMenu.Close();
            // add all the skills to a list so that we can remove skills later to avoid duplications
            _mutualSkillsDatabase.Skills.ForEach(s => _randomSkills.Add(s));
            _specialSkillsDatabase.Skills.ForEach(s => _randomSkills.Add(s));
            _workShopNavigatable = GameManager.Instance.WorkShopManager;
        }

        public override void SetUp()
        {
            base.SetUp();

            _cells.ForEach(c => c.SetUp());
            _activeSkillCell = _centerCell;
            Navigate(Direction.Down);
            Navigate(Direction.Up);
            CancelInvoke();
            _canMove = false;
        }

        #region Open, Close and Load
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
            _activeSkillCell.HighLight(true);
            _canMove = true;
            SetUpInput(PlayerIndex);
            GameManager.Instance.InputManager.EnableUIInput();
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(PlayerIndex);
            // load the player's skill
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsActive)
            {
                return;
            }

            base.Close(playerIndex);
            CancelInvoke();
            DisposeInput(playerIndex);
        }

        public override void Load(int playerIndex = 0)
        {
            base.Load(playerIndex);
        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;

            UIMap.FindAction(c.UI.Submit.name).started += StartSelection;
            UIMap.FindAction(c.UI.Submit.name).canceled += EndSelection;
            UIMap.FindAction(c.UI.Submit.name).performed += SelectAction;
        }

        private void StartSelection(InputAction.CallbackContext _)
        {
            if (!_activeSkillCell.IsOnHold)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(SelectionHoldProcess());
        }

        private void EndSelection(InputAction.CallbackContext _)
        {
            if (!_isHolding)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(SelectionHoldDeprocess());
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;

            UIMap.FindAction(c.UI.Submit.name).started -= StartSelection;
            UIMap.FindAction(c.UI.Submit.name).canceled -= EndSelection;
            UIMap.FindAction(c.UI.Submit.name).performed -= SelectAction;

            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.InputManager.EnablePlayerInput();
        }

        public override void NavigateLeft(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Left);
        }
        public override void NavigateRight(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Right);
        }
        public override void NavigateUp(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Up);
        }
        public override void NavigateDown(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Down);
        }

        private void SelectAction(InputAction.CallbackContext _)
        {
            if (_activeSkillCell.IsOnHold)
            {
                return;
            }

            Select();
        }
        #endregion

        #region Input methods

        #region Navigation
        public override void Navigate(Direction direction)
        {
            if (!IsActive || !_canMove)
            {
                return;
            }

            base.Navigate(direction);

            var nextCell = _activeSkillCell.NextCellInputs.Find(c => c.CellDirection == direction);

            if (nextCell == null)
            {
                return;
            }

            _activeSkillCell.HighLight(false);
            _activeSkillCell = nextCell.Cell;
            _activeSkillCell.HighLight(true);


            string name = string.Empty;
            string description = string.Empty;
            string requiredPoints = string.Empty;
            CellState state = _activeSkillCell.CellState;
            int playerPoints = _currentPlayer.Upgrades.GetSkillPoints();

            if (_activeSkillCell.CellType == CellType.Random)
            {
                name = RANDOM_NAME_TEXT;
                description = RANDOM_DESCRIPTION_TEXT;
                requiredPoints = RANDOM_POINTS_REQUIRED_TEXT;
            }
            else if(_activeSkillCell.SkillProfile == null)
            {
                name = EMPTY_NAME_TEXT;
                description = EMPTY_DESCRIPTION_TEXT;
                requiredPoints = EMPTY_POINTS_REQUIRED_TEXT;
            }
            else
            {
                //name = _activeSkillCell.SkillProfile.GetName();
                //description = _activeSkillCell.SkillProfile.GetDescription();
                requiredPoints = _activeSkillCell.SkillProfile.SkillPointsRequired.ToString();
            }

            //SetSkillInfo(name, description, requiredPoints);
            UpdateUI(name, description, requiredPoints, state, playerPoints);
            // cool down stuff
            _canMove = false;
            Invoke(nameof(EnableMovement), _coolDown);
        }
        #endregion

        private IEnumerator SelectionHoldProcess()
        {
            if (!CellIsSelectable())
            {
                yield break;
            }

            _isHolding = true;
            float time = Mathf.Lerp(0f, SELECTION_HOLD_DURATION,
                   Mathf.InverseLerp(0f, SELECTION_HOLD_DURATION, _activeSkillCell.GetFillAmount()));
            _canMove = false;

            while (time < SELECTION_HOLD_DURATION)
            {
                time += Time.deltaTime;
                _activeSkillCell.SetProgressAmount(time / SELECTION_HOLD_DURATION);
                yield return null;
            }

            _isHolding = false;
            _canMove = true;
            Select();
        }

        private IEnumerator SelectionHoldDeprocess()
        {
            if(_activeSkillCell.GetFillAmount() == 0f)
            {
                yield break;
            }

            float time = Mathf.Lerp(0f, SELECTION_HOLD_DURATION, 
                Mathf.InverseLerp(0f, SELECTION_HOLD_DURATION, _activeSkillCell.GetFillAmount()));
            _canMove = false;

            while (time > 0)
            {
                time -= Time.deltaTime * SELECTION_HOLD_EMPTYING_SPEED_MULTIPLIER;
                _activeSkillCell.SetProgressAmount(time / SELECTION_HOLD_DURATION);
                yield return null;
            }

            _canMove = true;
        }

        private bool CellIsSelectable()
        {
            bool isNonSelectable = _activeSkillCell.CellType == CellType.None;
            bool isUnlocked = _activeSkillCell.CellState == CellState.Unlocked;

            if (isNonSelectable || !IsActive || isUnlocked)
            {
                return false;
            }

            bool cellIsUnavailable = _activeSkillCell.CellState == CellState.Unavailable;
            bool playerHasEnoughSkillPoints = _currentPlayer.Upgrades.GetSkillPoints() >= _activeSkillCell.SkillProfile.SkillPointsRequired;

            if (cellIsUnavailable)
            {
                print($"Can't unlock this skill yet. Unlock the previous one first.");
                return false;
            }

            if (!playerHasEnoughSkillPoints)
            {
                return false;
            }

            return true;
        }

        public override void Select()
        {
            if (!CellIsSelectable())
            {
                return;
            }

            _activeSkillCell.PlayOnProgressBarFinishedAnimation();

            switch (_activeSkillCell.CellType)
            {
                case CellType.Predefined:
                    {
                        OnPredefinedSkillSelected();
                        break;
                    }
                case CellType.Random:
                    {
                        OnRandomSkillSelected();
                        break;
                    }
                case CellType.RandomOptionMenu:
                    {
                        OnCellFromRandomMenuSelected();
                        break;
                    }
            }
        }

        private void OnPredefinedSkillSelected()
        {
            SkillProfile skillProfile = _activeSkillCell.SkillProfile;
            // apply the skill
            // TODO: Fix the skill tree
            //skillProfile.SkillHolder.SetUp(_currentPlayer.Upgrades.transform);
            // set the cell's state to unlocked
            _activeSkillCell.ChangeCellState(CellState.Unlocked);
            // deduct the skill points
            _currentPlayer.Upgrades.AddSkillPoints(-skillProfile.SkillPointsRequired);
            // update UI
            UpdateUI("--", _activeSkillCell.CellState, _currentPlayer.Upgrades.GetSkillPoints());

        }

        private void OnRandomSkillSelected()
        {
            // disable input from the workshop manager (changing tabs or exiting the menu)
            _workShopNavigatable.IsActive = false;
            // cache the last selected cell in the skill tree which is the random cell
            _lastSelectedCell = _activeSkillCell;
            // show the menu
            _randomSkillsMenu.Open();
            // remove the highlight from the skill tree cell
            _activeSkillCell.HighLight(false);
            // highlight the first cell in the random skills menu
            _activeSkillCell = _randomSkillsMenu.GetFirstCell();
            _activeSkillCell.HighLight(true);
            // set skill profiles for the cells
            SkillProfile firstSkill = _randomSkills.RandomItem(true);
            SkillProfile secondSkill = _randomSkills.RandomItem(true);
            _randomSkillsMenu.SetSkills(firstSkill, secondSkill);
            // deduct skill points from player
            int skillPointsRequired = 1;
            _currentPlayer.Upgrades.AddSkillPoints(-skillPointsRequired);
        }

        private void OnCellFromRandomMenuSelected()
        {
            // enable the workshop
            _workShopNavigatable.IsActive = true;

            SkillProfile skillProfile = _activeSkillCell.SkillProfile;
            // apply the skill
            // TODO: Skill tree fixes
            //skillProfile.SkillHolder.SetUp(_currentPlayer.Upgrades.transform);
            // deduct the skill points
            _currentPlayer.Upgrades.AddSkillPoints(-skillProfile.SkillPointsRequired);

            // update the cell in the skill tree
            _randomSkillsMenu.Close();
            _activeSkillCell = _lastSelectedCell;
            _activeSkillCell.SetSkillProfile(skillProfile);
            _activeSkillCell.ChangeCellState(CellState.Unlocked);
            _activeSkillCell.HighLight(true);
            _activeSkillCell.SetCellType(CellType.None);
            // return the unselected skill to the pool
            _randomSkills.Add(_randomSkillsMenu.GetOtherCell(_activeSkillCell).SkillProfile);

            UpdateUI("--", _activeSkillCell.CellState, _currentPlayer.Upgrades.GetSkillPoints());
        }
        #endregion

        #region Update UI overloads
        public void UpdateUI(CellState state, int availablePoints)
        {
            _skillStateText.text = state.ToString();

            if (availablePoints >= 0)
            {
                _playerSkillPointsText.text = availablePoints.ToString();
            }

            // coloring
            if (state == CellState.None)
            {
                _skillStateText.text = string.Empty;
            }
            else
            {
                _skillStateText.color = state == CellState.Unavailable ? _unavailableCellColor : state == CellState.Locked ? _lockedCellColor : _unlockedCellColor;
            }

            if (_activeSkillCell.SkillProfile == null)
            {
                return;
            }

            if (availablePoints < _activeSkillCell.SkillProfile.SkillPointsRequired)
            {
                _skillPointsRequiredText.color = _unavailableCellColor;
            }
            else
            {
                _skillPointsRequiredText.color = _lockedCellColor;
            }
        }

        public void UpdateUI(string pointsRequired, CellState state, int availablePoints)
        {
            if (pointsRequired != string.Empty)
            {
                _skillPointsRequiredText.text = pointsRequired;
            }

            UpdateUI(state, availablePoints);
        }

        public void UpdateUI(string name, string description, string requiredPoints, CellState state, int availablePoints)
        {
            if (name != string.Empty)
            {
                _skillNameText.text = name;
            }

            if (description != string.Empty)
            {
                _skillDescriptionText.text = description;
            }

            UpdateUI(requiredPoints, state, availablePoints);
        }
        #endregion

        private void EnableMovement()
        {
            _canMove = true;
        }

        public void SetSkillInfo(string name, string description, string skillPoints)
        {
            _skillNameText.text = name;
            _skillDescriptionText.text = description;
            _skillPointsRequiredText.text = skillPoints;
        }

        public override void DehighlightCells()
        {
            base.DehighlightCells();
            _activeSkillCell.HighLight(false);
        }

        public override void SetActiveCellAsFirstCell()
        {
            base.SetActiveCellAsFirstCell();

            _activeSkillCell = _centerCell;
            _activeSkillCell.HighLight(true);
        }
    }
}
