using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Combat;
    using TankLike.Utils;

    public class PlayerBoostAbility : MonoBehaviour, IController, IInput, IDisplayedInput
    {
        [SerializeField] private BoostAbilityHolder _abilityToAdd;
        public bool IsActive { get; set; }
        private PlayerComponents _components;
        private Ability _currentAbility;
        private BoostAbilityHolder _currentAbilityHolder;
        private float _updateDistance = 1f;
        private PlayerBoost _boost;
        private bool _isDoubleBoostActivated = false;

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.DoubleBoost.name).performed += DoubleBoost;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.DoubleBoost.name).performed -= DoubleBoost;
        }
        #endregion

        private void DoubleBoost(InputAction.CallbackContext _)
        {
            _isDoubleBoostActivated = true;
        }

        public void Setup(PlayerComponents components)
        {
            _components = components;
            _boost = _components.PlayerBoost;

            if (_components.AbilityData != null)
            {
                AddAbility(_components.AbilityData.GetBoostAbility());
            }
            else
            {
                AddAbility(_abilityToAdd);
            }
        }

        public void AddAbility(BoostAbilityHolder ability)
        {
            // set up the ability
            _currentAbilityHolder = Instantiate(ability);
            _currentAbility = Instantiate(_currentAbilityHolder.Ability);
            _currentAbilityHolder.Ability = _currentAbility;
            _currentAbilityHolder.Ability.SetUp(_components);
            _updateDistance = _currentAbilityHolder.DistancePerActivation;

            // set up the UI
            UpdateInputDisplay(_components.PlayerIndex);

            // subscribe the ability to the playerBoost events
            _components.PlayerBoost.OnBoostStart += StartAbility;
            _components.PlayerBoost.OnBoostUpdate += UpdateAbility;
            _components.PlayerBoost.OnBoostEnd += EndAbility;
        }

        private void StartAbility()
        {
            if(!_isDoubleBoostActivated)
            {
                return;
            }

            ApplyBoostModifiers();
            _currentAbility.PerformAbility();
        }

        private void UpdateAbility()
        {
            if (!_isDoubleBoostActivated)
            {
                return;
            }

            if (_boost.DistanceTravelled > _updateDistance)
            {
                _boost.ResetDistanceCalculator();
                _currentAbility.PerformAbility();
            }
        }

        private void EndAbility()
        {
            ResetBoostModifiers();
            _isDoubleBoostActivated = false;
        }

        private void ApplyBoostModifiers()
        {
            _boost.MultiplyStartBoostConsumptionRate(_currentAbilityHolder.FuelStartConsumptionRateMultiplier);
            _boost.MultiplyBoostConsumptionRate(_currentAbilityHolder.FuelConsumptionRateMultiplier);
            _boost.MultiplySpeedMultiplier(_currentAbilityHolder.SpeedMultiplier);
        }

        private void ResetBoostModifiers()
        {
            _boost.ResetAllBoostFuelConsumptionRates();
            _boost.ResetSpeedMultiplier();
        }


        #region IController
        public void Activate()
        {
            SetUpInput(_components.PlayerIndex);
        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {

        }

        public void Restart()
        {
            DisposeInput(_components.PlayerIndex);
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            // set the input key for the boost ability
            int boostActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(
                InputManager.Controls.Player.Boost.name, _components.PlayerIndex);

            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetBoostInfo(_currentAbilityHolder.GetIcon(), Helper.GetInputIcon(boostActionIconIndex));
        }
        #endregion
    }
}
