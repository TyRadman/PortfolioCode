using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UI;
using TankLike.UnitControllers;
using TankLike.Utils;
using TMPro;
using UnityEngine;

namespace TankLike.UI.SkillTree
{
    public class SwappablesNavigatable : Navigatable
    {
        public enum CellType
        {
            Weapon = 0, HoldDownAction = 1, SuperAbility = 2
        }

        [Header("References")]
        [SerializeField] private List<SwappableGroup> _swappables;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        private PlayerInputActions _playerinput;
        [SerializeField] private SwappableGroup _selectedCell;
        [SerializeField] private int _playerIndex = 0;
        private PlayerComponents _player;

        private void Start()
        {
            _swappables.ForEach(s => s.SkillsSelector.SetUpInput());
            _selectedCell = _swappables[0];
            _selectedCell.Cell.HighLight(true);
        }

        public void SetPlayer(PlayerComponents player)
        {
            _player = player;
        }

        public override void SetUpInput()
        {
            //_playerinput = GameManager.Instance.InputManager.GetInputControls(0);

            ////_playerinput.UI.Navigate_Up.performed += (context) => Navigate(Direction.Up, 0);
            ////_playerinput.UI.Navigate_Down.performed += (context) => Navigate(Direction.Down, 0);
            ////_playerinput.UI.Submit.performed += (context) => Select(0);
            //_playerinput.UI.Cancel.performed += (context) => Close();
        }

        public override void Load(int playerIndex = 0)
        {
            if (_playerIndex >= GameManager.Instance.PlayersManager.GetPlayersCount() - 1) return;

            base.Load(playerIndex);
            print(_playerIndex.ToString().Color(Color.red) + $"At {gameObject.name}");
            _player = GameManager.Instance.PlayersManager.GetPlayer(_playerIndex);
            _selectedCell.HighLight(true);
            LoadSkillsIntoCells();
            _swappables.ForEach(s => LoadSkillsBasedOnSkillType(s));
        }

        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
            IsLastLayer = true;
            _selectedCell.Cell.HighLight(true);
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsActive) return;

            base.Close(playerIndex);
        }

        // loads the skills that the player is currently equipped with
        public void LoadSkillsIntoCells()
        {
            // load the 3 different skills
            _swappables.Find(s => s.CellType == CellType.Weapon).Cell.SetSkill(_player.Shooter.GetWeapon());
            _swappables.Find(s => s.CellType == CellType.HoldDownAction).Cell.SetSkill(_player.OnHold.GetHoldDownSkill());
            _swappables.Find(s => s.CellType == CellType.SuperAbility).Cell.SetSkill(_player.SuperAbilities.GetSuperAbility());
        }

        public override void Navigate(Direction direction)
        {
            if (!IsActive) return;

            if (direction == Direction.Right || direction == Direction.Left) return;

            base.Navigate(direction);
            
            // dehighlight the previous cell
            _selectedCell.HighLight(false);
            // swap the cells
            _selectedCell = _selectedCell.GetNeighborCell(direction);
            // highlight the previous cell
            _selectedCell.HighLight(true);
            // fill in the skill's info
            _skillNameText.text = _selectedCell.Cell.Skill.GetName();
            _skillDescriptionText.text = _selectedCell.Cell.Skill.GetDescription();
        }

        public override void Select()
        {
            if (!IsActive) return;

            base.Select();

            // if we're selecting the already selected cell, then do nothing
            LoadSkillsBasedOnSkillType(_swappables.Find(g => g.Cell.CellHighlightState == SwappableCell.HighlightState.Highlighted));
            // open the skills selector for the selected swappable skill
            _selectedCell.SkillsSelector.Open();
            // dehighlight the currently selected cell on the left to show the transition
            _selectedCell.HighLight(false);
            // since we're opening the skills selector, this is no longer the last window opened
            IsLastLayer = false;
            IsActive = false;
        }

        private void LoadSkillsBasedOnSkillType(SwappableGroup group)
        {
            List<Skill> skills = new List<Skill>();

            switch (group.CellType)
            {
                case CellType.Weapon:
                    {
                        _player.Shooter.GetWeapons().ForEach(w => skills.Add(w));
                        
                        if (_player.Shooter.GetWeapon() == null)
                        {
                            group.SkillsSelector.SetEmpty();
                            return;
                        }

                        group.SkillsSelector.LoadSkillsIntoCells(skills, skills.IndexOf(_player.Shooter.GetWeapon()));
                        return;
                    }
                case CellType.HoldDownAction:
                    {
                        _player.OnHold.GetHoldDownSkills().ForEach(w => skills.Add(w));

                        if(_player.OnHold.GetHoldDownSkill() == null)
                        {
                            group.SkillsSelector.SetEmpty();
                            return;
                        }

                        group.SkillsSelector.LoadSkillsIntoCells(skills, skills.IndexOf(_player.OnHold.GetHoldDownSkill()));
                        return;
                    }
                case CellType.SuperAbility:
                    {
                        _player.SuperAbilities.GetSuperAbilities().ForEach(w => skills.Add(w));

                        if (_player.SuperAbilities.GetSuperAbility() == null)
                        {
                            group.SkillsSelector.SetEmpty();
                            return;
                        }

                        group.SkillsSelector.LoadSkillsIntoCells(skills, skills.IndexOf(_player.SuperAbilities.GetSuperAbility()));
                        return;
                    }
            }
        }

        public void LoadCellWithSkill(Skill skill)
        {
            if (skill is Weapon)
            {
                _swappables.Find(s => s.CellType == CellType.Weapon).Cell.SetSkill(skill);
                _player.Shooter.SetWeapon((Weapon)skill);
            }
            else if (skill is OnHoldSkill)
            {
                _swappables.Find(s => s.CellType == CellType.HoldDownAction).Cell.SetSkill(skill);
                _player.OnHold.SetOnHoldSkill((Ability)skill);
            }
            else if (skill is Ability)
            {
                _swappables.Find(s => s.CellType == CellType.SuperAbility).Cell.SetSkill(skill);
                _player.SuperAbility.SetSuperAbility((Ability)skill);
            }
            else
            {
                Debug.LogError("Wait what?");
            }
        }
    }
}
