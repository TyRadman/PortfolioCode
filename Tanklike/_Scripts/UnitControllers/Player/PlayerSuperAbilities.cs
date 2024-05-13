using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankLike.Utils;
using TankLike.Misc;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    public class PlayerSuperAbilities : TankSuperAbilities, IInput, IDisplayedInput
    {
        [SerializeField] private PlayerSuperAbilityRecharger _abilityRecharger;
        private float _abilityUsageDuration;
        private bool _isAbilityReady = false;

        private Coroutine _holdCoroutine;
        private bool _isHolding = false;
        [SerializeField] private bool _playEffects;
        [SerializeField] private SuperAbilityHolder _superAbilityPrefab;
        [SerializeField] private ParticleSystem _superReadyEffect;
        private PlayerComponents _playerComponents;

        public void SetUp(PlayerComponents components)
        {
            _playerComponents = components;
            AddSkill(_superAbilityPrefab);
            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].SetSuperAbilityChargeAmount(1f, 0);
            base.Start();
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Special_Hold.name).performed += OnHoldDown;
            playerMap.FindAction(c.Player.Special_Hold.name).canceled += OnHoldUp;
            playerMap.FindAction(c.Player.Shoot.name).performed += ShootSuper;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Special_Hold.name).performed -= OnHoldDown;
            playerMap.FindAction(c.Player.Special_Hold.name).canceled -= OnHoldUp;
            playerMap.FindAction(c.Player.Shoot.name).performed -= ShootSuper;
        }

        public void UpdateInput(int playerIndex)
        {
            // set the key for the ability input on the HUD
            string key = GameManager.Instance.InputManager.GetButtonBindingKey(
                InputManager.Controls.Player.Special.name, _playerComponents.PlayerIndex);
            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].SetSuperAbilityInfo(_tankSuperAbility.Ability.GetIcon(), key);
        }
        #endregion

        private void PlayVFX()
        {
            ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Buffs.SuperAbility;
            vfx.transform.parent = transform;
            vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            vfx.gameObject.SetActive(true);
            vfx.Play();
            _superReadyEffect.Stop(true);
        }

        #region Hold Inputs
        private void OnHoldDown(InputAction.CallbackContext _)
        {
            if (!_canUseAbility || !_isAbilityReady)
            {
                return;
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            // check for constraints and apply them
            _tankSuperAbility.Ability.OnAbilityHoldStart();
            _holdCoroutine = StartCoroutine(HoldRoutine());
        }

        private IEnumerator HoldRoutine()
        {
            _isHolding = true;
            _components.Shooter.EnableShooting(false);

            // while the player is holding the button down and the super hasn't been fully charged for more than the cancel duration
            while (_isHolding)
            {
                _tankSuperAbility.Ability.OnAbilityHoldUpdate();
                yield return null;
            }
        }

        private void OnHoldUp(InputAction.CallbackContext _)
        {
            if (!_isHolding)
            {
                return;
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            _isHolding = false;
            _components.Shooter.EnableShooting(true);
            _tankSuperAbility.Ability.OnAbilityCancelled();
        }

        public void ShootSuper(InputAction.CallbackContext _)
        {
            if (!_canUseAbility || !_isAbilityReady || !_isHolding)
            {
                return;
            }

            _isHolding = false;

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            UseAbility();
            _components.Shooter.EnableShooting(true);
        }
        #endregion

        public override void AddSkill(SuperAbilityHolder ability)
        {
            base.AddSkill(ability);
            UpdateInput(_playerComponents.PlayerIndex);
            _abilityUsageDuration = ability.Ability.GetDuration();
            _isAbilityReady = false;
            // set up ability recharging methods
            _abilityRecharger.SetUpRechargeMethods(ability.RechargeInfo);
        }

        public override void UseAbility()
        {
            if (!_canUseAbility || !_isAbilityReady)
            {
                return;
            }

            // disable the ability so that it can't be used until the cooldown time runs out
            _isAbilityReady = false;
            // activate the ability
            base.UseAbility();
            // start cooldown counter
            StartCoroutine(AbilityPerformanceTimer());
            PlayVFX();
            OnSuperPerformed?.Invoke();
        }

        /// <summary>
        /// Sets a timer for how long it takes the ability to be fully performed before allow the player to recharge it again
        /// </summary>
        private IEnumerator AbilityPerformanceTimer()
        {
            float time = 0f;

            // count down process
            while (time < _abilityUsageDuration)
            {
                time += Time.deltaTime;
                GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].SetSuperAbilityChargeAmount(time / _abilityUsageDuration, 0);
                yield return null;
            }

            // enable recharging
            _abilityRecharger.EnableRecharging();
        }

        public void EnableAbilityUsage()
        {
            _isAbilityReady = true;
            _superReadyEffect.Play();
            // UI stuff
        }

        public Ability GetSuperAbility()
        {
            return _tankSuperAbility.Ability;
        }

        public List<Ability> GetSuperAbilities()
        {
            List<Ability> list = new List<Ability>();
            _superAbilities.ForEach(a => list.Add(a.Ability));
            return list;
        }

        #region IController
        public override void Activate()
        {
            base.Activate();
            SetUpInput(_playerComponents.PlayerIndex);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Restart()
        {
            base.Restart();

            DisposeInput(_playerComponents.PlayerIndex);

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }

    [System.Flags]
    public enum AbilityRechargingMethod
    {
        None = 0,
        OnEnemyHit = 1,
        OverTime = 2,
        OnPlayerHit = 4
    }
}