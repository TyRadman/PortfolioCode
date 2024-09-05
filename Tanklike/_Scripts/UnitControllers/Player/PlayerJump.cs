using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

namespace TankLike.UnitControllers
{
    public class PlayerJump : MonoBehaviour, IController
    {
        [Header("Jump Settings")]
        [SerializeField] private float _jumpMaxHeight;
        [SerializeField] private AnimationCurve _JumpCurve;
        [SerializeField] private AnimationCurve _fallCurve;
        [SerializeField] private float _jumpDuration;
        [SerializeField] private float _flyDuration;
        [SerializeField] private float _fallDuration;
        //[SerializeField] private float _jumpCooldown = 1f;
        [Tooltip("The point during the fall process at which the player becames vulnerable."), Range(0f, 1f)]
        [SerializeField] private float _invincibilityEndTime = 0f;

        [Header("Fuel Settings")]
        [SerializeField] private float _jumpFuelConsumptionPerSecond = 20f;
        [SerializeField] private float _jumpFuelInitialConsumption = 10f;
        [SerializeField] private float _jumpFuelConsumptionRate = 0.1f;

        [Header("Other Settings")]
        [SerializeField] private AbilityConstraint _constraints;
        [SerializeField] private Audio _thrusterAudio;
        [SerializeField] private Wiggle _onBoostingWiggle;

        [Header("References")]
        [SerializeField] private Transform _playerRenderer;
        [SerializeField] private JumpBar _UIBar;

        [SerializeField] private bool _canJump;

        private PlayerComponents _components;
        private PlayerFuel _fuel;
        private List<ParticleSystem> _thrusters;
        private List<ParticleSystem> _tracks;
        private ParticleSystem _landParticles;

        private bool _isJumpPressed;
        private bool _isflying;
        private bool _isUsingFuel;
        private bool _isCooldown;
        private Vector3 _rendererGroundedPosition;
        private Coroutine _jumpCoroutine;
        private Coroutine _flyCoroutine;
        private Coroutine _fallCoroutine;
        private Coroutine _cooldownCoroutine;

        private const int JUMP_LAYER = 22;
        private const int PLAYER_DAMAGEABLE_LAYER = 23;

        public bool IsActive { get; private set; }
        public bool IsJumping { get; private set; }
        public Action OnJumped { get; internal set; }

        public void SetUp(PlayerComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            _components = components;
            _fuel = components.Fuel;

            var carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);
            var body = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);

            _thrusters = carrier.ThrustersParticles;
            _tracks = carrier.TracksParticles;
            _landParticles = body.LandParticles;
            _rendererGroundedPosition = _playerRenderer.localPosition;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Jump.name).performed += OnJumpInputDown;
            playerMap.FindAction(c.Player.Jump.name).canceled += OnJumpUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Jump.name).performed -= OnJumpInputDown;
            playerMap.FindAction(c.Player.Jump.name).canceled -= OnJumpUp;
        }
        #endregion

        public void OnJumpInputDown(InputAction.CallbackContext context)
        {
            if (!IsActive || !_canJump || _isCooldown)
            {
                return;
            }

            if (_fuel.HasEnoughFuel(_jumpFuelInitialConsumption))
            {
                OnJumpStarted();
                OnJumped?.Invoke();
                _jumpCoroutine = StartCoroutine(JumpRoutine());
                StartCoroutine(FuelConsumptionRoutine());
            }
        }

        public void OnJumpUp(InputAction.CallbackContext context)
        {
            if (!IsActive)
            {
                return;
            }

            _isJumpPressed = false;
        }

        private void OnJumpStarted()
        {
            _isJumpPressed = true;

            PerformForwardWiggle();

            IsJumping = true;
            _components.Constraints.ApplyConstraints(true, _constraints);
            _thrusters.ForEach(e => e.Play());
            _tracks.ForEach(e => e.Stop());
            GameManager.Instance.AudioManager.Play(_thrusterAudio);
            _components.Health.SwitchDamageDetectorsLayer(JUMP_LAYER); //dirty

            _UIBar.PlayShowAnimation();
            _UIBar.SetAmount(1f);

        }

        private void PerformForwardWiggle()
        {
            if (!_components.PlayerBoost.IsBoosting)
            {
                return;
            }

            _components.TankWiggler.WiggleBody(_onBoostingWiggle);
        }

        private IEnumerator JumpRoutine()
        {
            float timer = 0f;

            while (timer < _jumpDuration)
            {
                timer += Time.deltaTime;
                // Move upward until you reach the max jump height
                float progress = timer / _jumpDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _JumpCurve.Evaluate(progress) * _jumpMaxHeight, 0f);

                _components.Animation.AnimateMovement(true, 1, 0, 0); // dirty

                yield return null;
            }

            _flyCoroutine = StartCoroutine(FlyRoutine());
        }

        private IEnumerator FlyRoutine()
        {
            float timer = 0f;
            _isflying = true;

            while (timer <= _flyDuration)
            {
                if (!_isJumpPressed)
                {
                    break;
                }

                _components.Animation.AnimateMovement(true, 1, 0, 0); // dirty
                timer += Time.deltaTime;
                _UIBar.SetAmount(1 - timer / _flyDuration);
                yield return null;
            }

            _isUsingFuel = false;
            _isflying = false;
            _thrusters.ForEach(e => e.Stop());
            _fallCoroutine = StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            float timer = 0f;
            float progress = 0f;
            bool isVulnerable = false;

            _UIBar.PlayHideAnimation();

            while (timer < _fallDuration)
            {
                // Move downward
                progress = timer / _fallDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _fallCurve.Evaluate(progress) * _jumpMaxHeight, 0f);

                _components.Animation.AnimateMovement(true, 1, 0, 0); // dirty

                if(progress >= _invincibilityEndTime && !isVulnerable)
                {
                    _components.Health.SwitchDamageDetectorsLayer(PLAYER_DAMAGEABLE_LAYER);
                    isVulnerable = true;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _playerRenderer.localPosition = _rendererGroundedPosition;

            IsJumping = false;
            _components.Constraints.ApplyConstraints(false, _constraints);
            //_components.Health.SetInvincible(false);
            //gameObject.layer = PLAYER_LAYER;
            _tracks.ForEach(e => e.Play());
            _landParticles.Play();

            _isCooldown = false;

        }

        private void StopJump()
        {
            this.StopCoroutineSafe(_jumpCoroutine);
            this.StopCoroutineSafe(_flyCoroutine);

            _isUsingFuel = false;
            _isflying = false;
            _thrusters.ForEach(e => e.Stop());
            _fallCoroutine = StartCoroutine(FallRoutine());
        }

        private void ResetJump()
        {
            StopAllJumpCoroutines();

            _playerRenderer.localPosition = _rendererGroundedPosition;

            IsJumping = false;
            _isUsingFuel = false;
            _isflying = false;

            _components.Constraints.ApplyConstraints(false, _constraints);
            _components.Health.SwitchDamageDetectorsLayer(PLAYER_DAMAGEABLE_LAYER);
                                                                                   
            _thrusters.ForEach(e => e.Stop());

            _UIBar.PlayHideAnimation();
        }

        private void StopAllJumpCoroutines()
        {
            this.StopCoroutineSafe(_jumpCoroutine);
            this.StopCoroutineSafe(_flyCoroutine);
            this.StopCoroutineSafe(_fallCoroutine);
            this.StopCoroutineSafe(_cooldownCoroutine);
        }

        private IEnumerator FuelConsumptionRoutine()
        {
            _isUsingFuel = true;

            _fuel.UseFuel(_jumpFuelInitialConsumption);

            yield return new WaitForSeconds(_jumpDuration);

            while (_isUsingFuel)
            {
                // If we run out of fuel, we break out of the loop
                if (!_fuel.HasEnoughFuel() && _isflying)
                {
                    StopJump();
                    break;
                }
              
                _fuel.UseFuel(_jumpFuelInitialConsumption * Time.deltaTime);

                yield return null;
            }
        }

        public void EnableJump(bool value)
        {
            _canJump = value;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
            EnableJump(true);
            SetUpInput(_components.PlayerIndex);
        }

        public void Deactivate()
        {
            IsActive = false;
            ResetJump();
        }

        public void Restart()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);

            StopAllJumpCoroutines();
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
