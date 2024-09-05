using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerBoost : MonoBehaviour, IController, IInput, IDisplayedInput
    {
        public bool IsActive { get; private set; }
        public bool IsBoosting { get; private set; }
        public Action OnBoostStart;
        public Action OnBoostUpdate;
        public Action OnBoostEnd;

        [Header("Settings")]
        [SerializeField] private int _boostMaxAmount;
        [SerializeField] private AnimationCurve _boostCurve;
        [SerializeField] private AnimationCurve _accelerationCurve;
        [SerializeField] private AbilityConstraint _constraints;
        [SerializeField] private float _startBoostSpeed = 0.5f;
        [field: SerializeField] public float BoostSpeedMultiplier { get; private set; } = 1.75f;

        [Header("Wiggles")]
        [SerializeField] protected Wiggle _boostWiggle;

        [Header("References")]
        [SerializeField] protected TankAnimation _animation;
        [SerializeField] protected PlayerMovement _movement;
        [SerializeField] protected PlayerComponents _components;
        [SerializeField] private TankBumper _bumper;

        [Header("Audio")]
        [SerializeField] private Audio _boostAudio;

        [Header("Fuel")]
        [SerializeField] private float _currentStartFuelConsumption;
        [SerializeField] private float _consumptionRate;

        private float _startFuelConsumption;
        private float _boostFuelConsumption;
        private float _currentSpeedMultiplier;

        public float DistanceTravelled { get; private set; } = 0f;
        private bool _isBoostingInputOn = false;
        private bool _canBoost;
        private List<ParticleSystem> _boostParticles = new List<ParticleSystem>();
        private PlayerFuel _fuel;
        private Coroutine _boostCoroutine;

        private const float DECELERATION_DURATION = 0.3f;
        private const float LINES_EFFECT_START_TIME = 0.25f;


        public void Setup(PlayerComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;
            _fuel = components.Fuel;

            var body = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);
            _boostParticles = body.BoostParticles;
            _boostParticles.ForEach(p => p.Stop());
            _bumper.DisableBump();

            SetUpInput(_components.PlayerIndex);
            UpdateInputDisplay(_components.PlayerIndex);

            _bumper.gameObject.SetActive(false);
            _bumper.gameObject.SetActive(true);

            ResetAllBoostFuelConsumptionRates();
            ResetSpeedMultiplier();
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Boost.name).started += StartBoost;
            playerMap.FindAction(c.Player.Boost.name).canceled += StopBoost;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            int index = _components.PlayerIndex;
            InputActionMap playerMap = InputManager.GetMap(index, ActionMap.Player);
            playerMap.FindAction(c.Player.Boost.name).started -= StartBoost;
            playerMap.FindAction(c.Player.Boost.name).canceled -= StopBoost;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            //string key = GameManager.Instance.InputManager.GetButtonBindingKey(
            //    InputManager.Controls.Player.Boost.name, playerIndex);
            //GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetBoostKey(key);
        }
        #endregion

        private void StartBoost(InputAction.CallbackContext _)
        {
            if (!_fuel.HasEnoughFuel(_startFuelConsumption))
            {
                return;
            }

            OnBoostStart?.Invoke();

            if (!IsActive || !_canBoost)
            {
                return;
            }

            if (IsBoosting)
            {
                RecoverBoost();
                return;
            }

            GameManager.Instance.AudioManager.Play(_boostAudio);
            _isBoostingInputOn = true;

            // apply constraints
            _components.Constraints.ApplyConstraints(true, _constraints);

            _boostParticles.ForEach(p => p.Play());

            _components.TankWiggler.WiggleBody(_boostWiggle);
            GameManager.Instance.CameraManager.PlayerCameraFollow.SetSpeedMultiplier(_currentSpeedMultiplier, _components.PlayerIndex);

            _movement.StopMovement();
            _movement.SetCurrentSpeed(0.5f);

            if (_boostCoroutine != null)
            {
                StopCoroutine(_boostCoroutine);
            }

            _boostCoroutine = StartCoroutine(BoostStartProcess());
        }

        private void RecoverBoost()
        {
            StopAllCoroutines();
            _isBoostingInputOn = true;
            StartCoroutine(BoostUpdateProcess());
        }

        #region Boost Process
        private IEnumerator BoostStartProcess()
        {
            DistanceTravelled = 0f;
            _bumper.EnableBump();
            IsBoosting = true;
            float timer = 0f;
            float accelerationTime = 0.25f;

            if (!_fuel.HasEnoughFuel(_startFuelConsumption))
            {
                yield break;
            }

            _fuel.UseFuel(_startFuelConsumption);

            // acceleration process
            while (timer < accelerationTime)
            {
                float dt = Time.deltaTime;
                timer += dt;

                OnBoostUpdate?.Invoke();

                float tankSpeed = _movement.GetMultipliedSpeed();
                DistanceTravelled += tankSpeed * dt;

                float t = _startBoostSpeed + _accelerationCurve.Evaluate(timer / accelerationTime) * (_currentSpeedMultiplier - _startBoostSpeed);
                _movement.SetCurrentSpeed(t);
                yield return null;
            }

            StartCoroutine(BoostUpdateProcess());
        }

        private IEnumerator BoostUpdateProcess()
        {
            if (!_fuel.HasEnoughFuel())
            {
                StartCoroutine(BoostEndProcess());
                yield break;
            }

            float linesEffectTimer = 0f;
            float tankSpeed = _movement.GetMultipliedSpeed();

            while (_isBoostingInputOn && _canBoost)
            {
                float dt = Time.deltaTime;

                DistanceTravelled += tankSpeed * dt;

                OnBoostUpdate?.Invoke();

                if (!_fuel.HasEnoughFuel() || !_isBoostingInputOn)
                {
                    break;
                }

                if (linesEffectTimer < LINES_EFFECT_START_TIME)
                {
                    linesEffectTimer += dt;

                    if (linesEffectTimer >= LINES_EFFECT_START_TIME)
                    {
                        GameManager.Instance.EffectsUIController.PlaySpeedLinesEffect();
                    }
                }

                float speed = _currentSpeedMultiplier * _bumper.GetBumperMultiplier();
                _movement.SetCurrentSpeed(speed);
                tankSpeed = _movement.GetMultipliedSpeed();
                _fuel.UseFuel(_boostFuelConsumption * dt);
                yield return null;
            }

            StartCoroutine(BoostEndProcess());
        }

        private IEnumerator BoostEndProcess()
        {
            float timer = 0f;

            float startSpeed = _movement.CurrentSpeed;

            while (timer < DECELERATION_DURATION)
            {
                float dt = Time.deltaTime;
                timer += dt;

                OnBoostUpdate?.Invoke();

                float tankSpeed = _movement.GetMultipliedSpeed();
                DistanceTravelled += tankSpeed * dt;

                float t = timer / DECELERATION_DURATION;
                float speed = Mathf.Lerp(startSpeed, 1f, t);
                _movement.SetCurrentSpeed(speed);
                yield return null;
            }

            OnBoostEnd?.Invoke();
            StopBoost();
        }
        #endregion

        private void StopBoost(InputAction.CallbackContext _)
        {
            _isBoostingInputOn = false;
        }

        private void StopBoost()
        {
            IsBoosting = false;
            _bumper.DisableBump();
            OnBoostFinishedHandler();
            GameManager.Instance.CameraManager.PlayerCameraFollow.ResetSpeedMultiplier(_components.PlayerIndex);
        }

        private void OnBoostFinishedHandler()
        {
            _boostParticles.ForEach(p => p.Stop());
            GameManager.Instance.EffectsUIController.StopSpeedLinesEffect();
            // disable constraints
            _components.Constraints.ApplyConstraints(false, _constraints);
        }

        public void EnableBoost(bool enable)
        {
            if(enable == _canBoost)
            {
                return;
            }

            _canBoost = enable;

            if (!enable)
            {
                _isBoostingInputOn = false;
                _movement.StartDeceleration();
            }
        }

        public void ResetDistanceCalculator()
        {
            DistanceTravelled = 0f;
        }

        #region Fuel Consumption Rate Modifiers
        /// <summary>
        /// Multipliers the start and update fuel consumption rate values by a provided value.
        /// </summary>
        /// <param name="multiplier"></param>
        public void MultiplyAllBoostFuelConsumption(float multiplier)
        {
            _startFuelConsumption *= multiplier;
            _boostFuelConsumption *= multiplier;
        }

        /// <summary>
        /// Multipliers the start fuel consumption rate values by a provided value.
        /// </summary>
        /// <param name="multiplier"></param>
        public void MultiplyStartBoostConsumptionRate(float multiplier)
        {
            _startFuelConsumption *= multiplier;
        }

        /// <summary>
        /// Multipliers the update fuel consumption rate values by a provided value.
        /// </summary>
        /// <param name="multiplier"></param>
        public void MultiplyBoostConsumptionRate(float multiplier)
        {
            _boostFuelConsumption *= multiplier;
        }

        public void MultiplySpeedMultiplier(float multiplier)
        {
            _currentSpeedMultiplier *= multiplier;
        }

        public void ResetSpeedMultiplier()
        {
            _currentSpeedMultiplier = BoostSpeedMultiplier;
        }

        public void ResetAllBoostFuelConsumptionRates()
        {
            _startFuelConsumption = _currentStartFuelConsumption;
            _boostFuelConsumption = _consumptionRate;
        }

        public void ResetStartBoostFuelConsumptionRate()
        {
            _startFuelConsumption = _currentStartFuelConsumption;
        }

        public void ResetBoostFuelConsumptionRate()
        {
            _boostFuelConsumption = _consumptionRate;
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;
            UpdateInputDisplay(_components.PlayerIndex);
            SetUpInput(_components.PlayerIndex);
            EnableBoost(true);
        }

        public void Deactivate()
        {
            IsActive = false;
            StopBoost();

            if (_boostCoroutine != null)
            {
                StopCoroutine(_boostCoroutine);
            }

            _bumper.ResetWallsCount();
        }

        public void Restart()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
