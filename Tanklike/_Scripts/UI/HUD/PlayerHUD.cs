using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;
        [Header("Avatar")]
        [SerializeField] private Image _avatarImage;        
        [Header("Health")]
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private ResizableBar _healthBar;
        [Header("Active Skills")]
        [SerializeField] private HUDSkillIcon _superAbility;
        [SerializeField] private HUDSkillIcon _weaponAbility;
        [SerializeField] private HUDSkillIcon _holdDownAbility;
        [SerializeField] private HUDSkillIcon _boostAbility;
        [Header("Tools")]
        [SerializeField] private ToolIcon _selectedToolIcon;
        [SerializeField] private ToolIcon _previousToolIcon;
        [SerializeField] private ToolIcon _nextToolIcon;
        [Header("Experience")]
        [SerializeField] private Image _experienceFillImage;
        [SerializeField] private TextMeshProUGUI _levelText;
        [Header("Fuel")]
        [SerializeField] private Image _fuelFillImage;
        [Header("Energy")]
        [SerializeField] private Image _energyFillImage;
        [SerializeField] private TextMeshProUGUI _energyKeyText;
        [Header("Wealth")]
        [SerializeField] private TextMeshProUGUI _coinsText;
        [Header("Inventory")]
        [SerializeField] private TextMeshProUGUI _inventoryKeyText;
        [Header("Extra")]
        [SerializeField] private Sprite _emptyToolSprite;

        private List<Image> _boostIcons = new List<Image>();

        public void Enable(bool value)
        {
            _parent.SetActive(value);
        }

        #region Health
        public void SetupHealthBar(int maxHealth)
        {
            if (_healthFillImage == null)
            {
                return;
            }

            // TODO: must store this value somewhere
            float highestHealthPossible = 600f;
            _healthBar.SetMaxSize(maxHealth / highestHealthPossible, true);
        }

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (_healthFillImage == null)
            {
                return;
            }

            _healthBar.SetValue((float)currentHealth / (float)maxHealth);
            //_healthFillImage.fillAmount = (float)currentHealth / (float)maxHealth;
        }
        #endregion

        #region Experience
        public void ResetExperienceBar()
        {
            if (_experienceFillImage == null) return;

            _experienceFillImage.fillAmount = 0f;
        }

        public void UpdateExperienceBar(int currentExperience, int maxExperience)
        {
            if (_experienceFillImage == null) return;

            _experienceFillImage.fillAmount = (float)currentExperience / (float)maxExperience;
        }

        public void UpdateLevelText(int currentLevel)
        {
            _levelText.text = currentLevel.ToString();
        }
        #endregion

        #region Skills
        #region Super Ability
        public void SetSuperAbilityInfo(Sprite icon, string key)
        {
            _superAbility.SetIconSprite(icon);
            _superAbility.SetKey(key);
        }

        public void SetSuperAbilityChargeAmount(float amount, int playerIndex)
        {
            _superAbility.SetFillAmount(amount);

            _superAbility.PlayAnimation(amount == 0f);
        }
        #endregion

        #region Weapon
        public void SetWeaponInfo(Sprite icon, string key)
        {
            _weaponAbility.SetIconSprite(icon);
            _weaponAbility.SetKey(key);
        }

        public void SetWeaponChargeAmount(float amount, int playerIndex)
        {
            _weaponAbility.SetFillAmount(amount);
        }
        #endregion

        #region Hold down
        public void SetHoldDownInfo(Sprite icon, string key)
        {
            _holdDownAbility.SetIconSprite(icon);
            _holdDownAbility.SetKey(key);
        }

        public void SetHoldDownChargeAmount(float amount, int playerIndex)
        {
            _holdDownAbility.SetFillAmount(amount);
            _holdDownAbility.PlayAnimation(amount <= 0f);
        }
        #endregion

        #region Boost
        public void SetBoostInfo(Sprite icon, string key)
        {
            _boostAbility.SetIconSprite(icon);
            _boostAbility.SetKey(key);
        }
        #endregion
        #endregion

        #region Tools
        public void UpdateTools(Tool activeTool, Tool previousTool, Tool nextTool)
        {
            _selectedToolIcon.SetUp(activeTool);

            if (previousTool != null) _previousToolIcon.SetUp(previousTool);
            else _previousToolIcon.ResetIcon(_emptyToolSprite);

            if (nextTool != null) _nextToolIcon.SetUp(nextTool);
            else _nextToolIcon.ResetIcon(_emptyToolSprite);
        }

        public void UpdateActiveTool(string amountText)
        {
            int amount = int.Parse(amountText);
            _selectedToolIcon.SetAmount(amount);
        }

        public void SetToolsKeys(string selected, string previous, string next)
        {
            _selectedToolIcon.SetKey(selected);
            _previousToolIcon.SetKey(previous);
            _nextToolIcon.SetKey(next);
        }

        public void SetActiveToolIconAsEmpty()
        {
            _selectedToolIcon.ResetIcon(_emptyToolSprite);
        }

        #endregion

        #region Wealth
        public void UpdateCoinsText(int amount)
        {
            if (_coinsText == null) return;

            _coinsText.text = amount.ToString();
        }
        #endregion

        #region Inventory
        public void SetInventoryKey(string key)
        {
            _inventoryKeyText.text = key;
        }
        #endregion

        #region Fuel
        public void UpdateFuelBar(float currentFuel, float maxFuel)
        {
            if (_fuelFillImage == null) return;

            _fuelFillImage.fillAmount = currentFuel / maxFuel;
        }
        #endregion

        #region Energy
        public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
        {
            if (_energyFillImage == null) return;

            _energyFillImage.fillAmount = currentEnergy / maxEnergy;
        }

        public void SetEnergyKey(string key)
        {
            _energyKeyText.text = key;
        }
        #endregion

        #region Avatar
        public void SetPlayerAvatar(Sprite avatar)
        {
            _avatarImage.sprite = avatar;
        }
        #endregion
    }
}
