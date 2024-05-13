using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

        [Header("Fuel Settings")]
        [SerializeField] private float _jumpFuelConsumptionUnit = 1f;
        [SerializeField] private float _jumpFuelConsumptionRate = 0.1f;


        [Header("Other Settings")]
        [SerializeField] private AbilityConstraint _constraints;
        [SerializeField] private Audio _thrusterAudio;

        [Header("References")]
        [SerializeField] private Transform _playerRenderer;
        [SerializeField] private Image _jumpBar;
        [SerializeField] private Animation _jumpBarAnim;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;

        private PlayerComponents _components;
        private PlayerFuel _fuel;
        private List<ParticleSystem> _thrusters;
        private List<ParticleSystem> _tracks;
        private ParticleSystem _landParticles;

        [SerializeField] private bool _canJump;
        private bool _isJumpPressed;
        private bool _isflying;
        private bool _isUsingFuel;
        private Coroutine _jumpCoroutine;
        private Coroutine _flyCoroutine;
        private Coroutine _fallCoroutine;
        private Vector3 _rendererGroundedPosition;
        private const int JUMP_LAYER = 22;
        private const int PLAYER_LAYER = 11;
        private const int PLAYER_DAMAGEABLE_LAYER = 23;

        public bool IsActive { get; private set; }
        public bool IsJumping { get; private set; }

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
            playerMap.FindAction(c.Player.Jump.name).performed += OnJumpDown;
            playerMap.FindAction(c.Player.Jump.name).canceled += OnJumpUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Jump.name).performed -= OnJumpDown;
            playerMap.FindAction(c.Player.Jump.name).canceled -= OnJumpUp;
        }
        #endregion

        public void OnJumpDown(InputAction.CallbackContext context)
        {
            if (!IsActive || !_canJump) return;

            if (_fuel.HasEnoughFuel(_jumpFuelConsumptionUnit * (_jumpDuration / _jumpFuelConsumptionRate)))
            {
                _isJumpPressed = true;
                _jumpCoroutine = StartCoroutine(JumpRoutine());
            }
        }

        public void OnJumpUp(InputAction.CallbackContext context)
        {
            if (!IsActive) return;

            _isJumpPressed = false;
        }

        private IEnumerator JumpRoutine()
        {
            float timer = 0f;
            float progress = 0f;

            IsJumping = true;
            _isUsingFuel = true;
            _components.Constraints.ApplyConstraints(true, _constraints);
            StartCoroutine(FuelConsumptionRoutine());
            //_components.Health.SetInvincible(true);
            _thrusters.ForEach(e => e.Play());
            _tracks.ForEach(e => e.Stop());
            GameManager.Instance.AudioManager.Play(_thrusterAudio);
            _components.Health.SwitchDamageDetectorsLayer(JUMP_LAYER); //dirty
            //gameObject.layer = JUMP_LAYER;
            // bar animation
            _jumpBarAnim.clip = _showClip;
            _jumpBarAnim.Play();
            _jumpBar.fillAmount = 1f;

            while (timer < _jumpDuration)
            {
                // Move upward until you reach the max jump height
                progress = timer / _jumpDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _JumpCurve.Evaluate(progress) * _jumpMaxHeight, 0f);

                _components.Animation.AnimateMovement(true, 1, 0, 0); // dirty

                timer += Time.deltaTime;
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
                    break;

                _components.Animation.AnimateMovement(true, 1, 0, 0); // dirty
                timer += Time.deltaTime;
                _jumpBar.fillAmount = 1 - timer / _flyDuration;
                yield return null;
            }

            //_jumpBar.fillAmount = 0f;
            _isUsingFuel = false;
            _isflying = false;
            _thrusters.ForEach(e => e.Stop());
            _fallCoroutine = StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            float timer = 0f;
            float progress = 0f;
            // bar animation
            _jumpBarAnim.clip = _hideClip;
            _jumpBarAnim.Play();

            while (timer < _fallDuration)
            {
                // Move downward
                progress = timer / _fallDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _fallCurve.Evaluate(progress) * _jumpMaxHeight, 0f);

                _components.Animation.AnimateMovement(true, 1, 0, 0); // dirty

                timer += Time.deltaTime;
                yield return null;
            }

            _playerRenderer.localPosition = _rendererGroundedPosition;

            IsJumping = false;
            _components.Constraints.ApplyConstraints(false, _constraints);
            //_components.Health.SetInvincible(false);
            _components.Health.SwitchDamageDetectorsLayer(PLAYER_DAMAGEABLE_LAYER); //dirty
            //gameObject.layer = PLAYER_LAYER;
            _tracks.ForEach(e => e.Play());
            _landParticles.Play();
        }

        private void StopJump()
        {
            if (_jumpCoroutine != null) StopCoroutine(_jumpCoroutine);
            if (_flyCoroutine != null) StopCoroutine(_flyCoroutine);

            _isUsingFuel = false;
            _isflying = false;
            _thrusters.ForEach(e => e.Stop());
            StartCoroutine(FallRoutine());
        }

        private IEnumerator FuelConsumptionRoutine()
        {
            float timer = 0f;

            _fuel.AddConsumer(this);

            while (_isUsingFuel)
            {
                timer += Time.deltaTime;

                if (timer >= _jumpFuelConsumptionRate)
                {
                    // If we run out of fuel, we break out of the loop
                    if (!_fuel.HasEnoughFuel(_jumpFuelConsumptionUnit) && _isflying)
                    {
                        StopJump();
                        break;
                    }

                    _fuel.UseFuel(_jumpFuelConsumptionUnit);
                    timer = 0f;
                }

                yield return null;
            }

            _fuel.RemoveConsumer(this);
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
