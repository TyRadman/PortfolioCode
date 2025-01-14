using System.Collections;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using UnitControllers;
    using UI.HUD;

    [CreateAssetMenu(fileName = PREFIX + "Special_RefillHP", menuName = Directories.SPECIAL_UPGRADES + "Player01/ Refill HP")]
    public class AutoHPRefillUpgrade : SkillUpgrade
    {
        [Header(SPECIAL_VALUES_HEADER)]
        [SerializeField] private float _healthThresholdToFill = 0.5f;
        [SerializeField] private int _healthFillAmountPerSecond = 5;
        [SerializeField] private StatModifierType _statModifierType;

        private StatIconReference _statIcon;
        private PlayerHealth _health;
        private PlayerStatsModifiersDisplayer _statsDisplayer;
        private Coroutine _refillCoroutine;
        private bool _isRefilling = false;

        private readonly WaitForSeconds _healingWait = new WaitForSeconds(1f);

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);

            if (player is not PlayerComponents playerComponents)
            {
                Debug.Log("PlayerComponents not found");
                return;
            }

            _health = player.GetUnitComponent<PlayerHealth>();
            _statsDisplayer = GameManager.Instance.HUDController.PlayerHUDs[player.PlayerIndex].StatModifiersDisplayer;
            _statIcon = GameManager.Instance.StatIconReferenceDB.GetStatIconReference(_statModifierType);

            _isRefilling = false;
        }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            SkillProperties healthThreshold = new SkillProperties()
            {
                IsComparisonValue = false,
                Name = "Activation Threshold",
                Value = (_healthThresholdToFill * 100f).ToString(),
                DisplayColor = Colors.LightOrange,
                UnitString = PropertyUnits.PERCENTAGE
            };

            _properties.Add(healthThreshold);

            SkillProperties healthToRestore = new SkillProperties()
            {
                IsComparisonValue = false,
                Name = "Regeneration Rate",
                Value = (_healthFillAmountPerSecond).ToString(),
                DisplayColor = Colors.Green,
                UnitString = PropertyUnits.HP_PER_SECOND
            };

            _properties.Add(healthToRestore);

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _health.OnHealthValueChanged += OnHealthValueChanged;
            _health.OnDeath += OnPlayerDeath;
        }

        private void OnHealthValueChanged(float currentHealth, float maxHealth)
        {
            bool startRefilling = (float)currentHealth / (float)maxHealth < _healthThresholdToFill && !_isRefilling;

            if (startRefilling)
            {
                _statsDisplayer.AddIcon(_statIcon);
                _refillCoroutine = GameManager.Instance.CoroutineManager.StartExternalCoroutine(RefillHealthRoutine());
            }
        }

        private IEnumerator RefillHealthRoutine()
        {
            _isRefilling = true;

            while (_health.GetHealthAmount01() < _healthThresholdToFill)
            {
                float remainingDifference = _healthThresholdToFill - _health.GetHealthAmount01();
                float healAmountPercentage = _healthFillAmountPerSecond / _health.GetMaxHealth();
                float healAmount = Mathf.Min(healAmountPercentage, remainingDifference);

                _health.Heal(Mathf.CeilToInt(healAmount * _health.GetMaxHealth()));

                yield return _healingWait;
            }

            if(_health.GetHealthAmount01() != _healthThresholdToFill)
            {
                _health.SetHealthPercentage(_healthThresholdToFill, false);
            }

            _statsDisplayer.RemoveIcon(_statIcon);
            _isRefilling = false;
        }

        private void OnPlayerDeath(TankComponents tank)
        {
            _health.OnHealthValueChanged -= OnHealthValueChanged;

            if (_refillCoroutine != null)
            {
                GameManager.Instance.CoroutineManager.StopExternalCoroutine(_refillCoroutine);
            }
        }
    }
}
