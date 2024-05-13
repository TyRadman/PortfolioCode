using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankLike.UnitControllers;
using TankLike.Combat;

namespace TankLike.SkillTree
{
    public class SkillTreeCell : MonoBehaviour, ICellSelectable
    {
        [System.Serializable]
        public class NextCell
        {
            public SkillTreeCell Cell;
            public Direction CellDirection;
            public bool IsNextCell = false;
            //public int PlayerIndex = -1;
        }

        [field: SerializeField] public SkillProfile SkillProfile { get; private set; }
        public List<NextCell> NextCellInputs;
        [SerializeField] private List<SkillTreeLine> _lines = new List<SkillTreeLine>();
        public static Color LOCKED_PATH_COLOR = Color.white;
        public static Color UNLOCKED_PATH_COLOR = Color.white;
        public static Color UNAVAILABLE_PATH_COLOR = Color.white;
        
        [Tooltip("Unlocked: the player already has the skill.\nLocked: the player can unlock the skill with skill points.\nUnavailable: the player can unlock the ability only when the ability before it in the skill tree is unlocked.\nNone: can never be unlocked (mostly non-skill cells.)")]
        [field: SerializeField, Header("Modifiers")] public CellState CellState { get; private set; } = CellState.None;
        [field: SerializeField] public CellType CellType { get; private set; } = CellType.Predefined;
        [field: SerializeField] public bool IsOnHold { get; private set; } = true;
        [Header("References")]
        [SerializeField] private GameObject _overlayImage;
        [SerializeField] private Image _cellIconImage;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _coverImage;
        [SerializeField] private Image _progressBar;
        private SkillTreeCell _previousCell;
        [SerializeField] private SkillTreeLine _previousLine;
        [SerializeField] private bool _debug = false;
        [SerializeField] private Animation _animation;

        private void Awake()
        {
            // find all the cells that are coming after this one and set this one as a previous cell for them.
            NextCellInputs.FindAll(c => c.IsNextCell).ForEach(c => c.Cell.SetPreviousCell(this));
        }

        public void SetUp()
        {
            _overlayImage.SetActive(false);

            if (CellState == CellState.Unlocked)
            {
                _lines.ForEach(l => l.PlayLockedAnimation());
            }

            SetIcon(SkillProfile.Skill.GetIcon());

            switch (CellType)
            {
                case CellType.Predefined:
                    {
                        SetUpPredefinedSkill();
                        break;
                    }
                case CellType.Random:
                    {
                        SetUpRandomSkill();
                        break;
                    }
                case CellType.RandomOptionMenu:
                    {
                        SetUpRandomMenuSkill();
                        break;
                    }
            }
        }

        public void SetUpPredefinedSkill()
        {
            if (SkillProfile == null)
            {
                return;
            }

            SetIcon(SkillProfile.Skill.GetIcon());
            _coverImage.enabled = true;
        }

        public void SetUpRandomSkill()
        {
            _overlayImage.SetActive(true);
        }

        public void SetUpRandomMenuSkill()
        {
            RemoveAllOverlays();
        }

        public void RemoveAllOverlays()
        {
            _coverImage.enabled = false;
            _overlayImage.SetActive(false);
        }

        #region Cell State Change
        public void ChangeCellState(CellState newState)
        {
            CellState = newState;

            switch (CellState)
            {
                case CellState.Unavailable:
                    OnUnavailableCell();
                    break;
                case CellState.Locked:
                    OnLockedCell();
                    break;
                case CellState.Unlocked:
                    OnUnlockedCell();
                    break;
            }
        }

        public void OnUnavailableCell()
        {

        }

        public void OnLockedCell()
        {
            _previousLine?.PlayLockedAnimation();
            //_lines.ForEach(l => l.PlayUnlockedAnimation());
        }

        private void OnUnlockedCell()
        {
            RemoveAllOverlays();
            _lines.ForEach(l => l.PlayLockedAnimation());
            _previousLine.PlayUnlockedAnimation();
            NextCellInputs.FindAll(c => c.Cell.CellState == CellState.Unavailable && c.IsNextCell)
                .ForEach(c => c.Cell.ChangeCellState(CellState.Locked));
        }
        #endregion

        #region UI related methods
        public void SetIcon(Sprite icon)
        {
            _cellIconImage.sprite = icon;
        }

        public void HighLight(bool enable)
        {
            _highlightImage.enabled = enable;
        }

        public void SetProgressAmount(float amount)
        {
            _progressBar.fillAmount = amount;
        }

        public float GetFillAmount()
        {
            return _progressBar.fillAmount;
        }

        public void PlayOnProgressBarFinishedAnimation()
        {
            _animation.Play();
        }
        #endregion

        public void SetPreviousCell(SkillTreeCell cell)
        {
            _previousCell = cell;
        }

        public void SetSkillProfile(SkillProfile skillProfile)
        {
            SkillProfile = skillProfile;
            SetUp();
        }

        public void SetCellType(CellType newType)
        {
            CellType = newType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CellState
    {
        Unavailable = 0, Locked = 1, Unlocked = 2, None = 3
    }

    public enum CellType
    {
        None = 0, Predefined = 1, Random = 2, RandomOptionMenu = 3
    }
}