using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerEnergy : MonoBehaviour, IController, IInput, IDisplayedInput
    {
        [Header("Settings")]
        [SerializeField] private float _maxEnergy;
        [SerializeField] private float _healRequiredEnergy = 20f;
        [SerializeField] private float _healChargeDuration = 1.5f;
        [SerializeField] private int _healAmount = 20;

        [Header("Constraints")]
        [SerializeField] private AbilityConstraint _constraints;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _chargeEffect;
        [SerializeField] private ParticleSystem _blastEffect;

        [Header("Audio")]
        [SerializeField] private Audio _healBlastAudio;

        private PlayerComponents _components;
        private float _currentEnergy;
        private Coroutine _holdCoroutine;

        public bool IsActive { get; private set; }

        public void Setup(PlayerComponents components)
        {
            _components = components;
            _currentEnergy = 0f;

            SetUpInput(_components.PlayerIndex);
            UpdateInputDisplay(_components.PlayerIndex);

            UpdateEnergyUI();
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Energy.name).performed += OnHoldDown;
            playerMap.FindAction(c.Player.Energy.name).canceled += OnHoldUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(_components.PlayerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Energy.name).performed -= OnHoldDown;
            playerMap.FindAction(c.Player.Energy.name).canceled -= OnHoldUp;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            int actionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.Player.Energy.name, playerIndex);

            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetEnergyKey(Helper.GetInputIcon(actionIconIndex));
        }
        #endregion

        private void OnHoldDown(InputAction.CallbackContext context)
        {
            if (!IsActive || _currentEnergy < _healRequiredEnergy)
            {
                return;
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            _holdCoroutine = StartCoroutine(HoldRoutine());
        }

        private void OnHoldUp(InputAction.CallbackContext context)
        {
            _chargeEffect.Stop();
            _components.Constraints.ApplyConstraints(false, _constraints);

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }
        }

        private IEnumerator HoldRoutine()
        {
            _components.Constraints.ApplyConstraints(true, _constraints);

            float timer = 0f;
            _chargeEffect.Play();

            while (_currentEnergy >= _healRequiredEnergy)
            {
                timer += Time.deltaTime;

                if(timer >= _healChargeDuration)
                {
                    Heal();
                    timer = 0f;
                }

                yield return null;
            }

            _chargeEffect.Stop();
            _components.Constraints.ApplyConstraints(false, _constraints);
        }

        private void Heal()
        {
            _components.Health.Heal(_healAmount);
            _blastEffect.Play();
            GameManager.Instance.AudioManager.Play(_healBlastAudio);
            _currentEnergy -= _healRequiredEnergy;
            UpdateEnergyUI();
        }

        public void AddEnergy(float amount)
        {
            _currentEnergy += amount;
            _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);

            UpdateEnergyUI();
        }

        public void SetEnergyAmount(float amount)
        {
            _currentEnergy = amount;
            _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);
            UpdateEnergyUI();
        }

        public void MaxFillEnergy()
        {
            _currentEnergy = _maxEnergy;
            UpdateEnergyUI();
        }

        private void UpdateEnergyUI()
        {
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateEnergyBar(_currentEnergy, _maxEnergy);
        }

        public float GetCurrentEnergy()
        {
            return _currentEnergy;
        }

        public float GetMaxEnergy()
        {
            return _maxEnergy;
        }

        public float GetEnergyPercentage()
        {
            return _currentEnergy / _maxEnergy;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
