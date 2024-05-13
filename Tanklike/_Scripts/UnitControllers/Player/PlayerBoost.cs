using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerBoost : MonoBehaviour, IController, IInput, IDisplayedInput
    {
        [Header("Settings")]
        [SerializeField] private int _boostMaxAmount;
        [SerializeField] private float _boostDuration = 2f;
        [SerializeField] private float _boostCooldownDuration;
        [SerializeField] private AnimationCurve _boostCurve;
        [SerializeField] private AbilityConstraint _constraints;
        [field: SerializeField] public float SingleBoostSpeedMultiplier { get; private set; } = 1.75f;
        [field: SerializeField] public float DoubleBoostSpeedMultiplier { get; private set; } = 2.5f;

        [Header("Wiggles")]
        [SerializeField] protected Wiggle _boostWiggle;

        [Header("References")]
        [SerializeField] protected TankAnimation _animation;
        [SerializeField] protected PlayerMovement _movement;
        [SerializeField] protected PlayerComponents _components;

        private int _boostCurrentAmount;
        private BoostState _boostState;
        //private bool _isBoosting;
        private bool _canBoost;
        private List<ParticleSystem> _boostParticles = new List<ParticleSystem>();
        private List<ParticleSystem> _doubleBoostParticles = new List<ParticleSystem>();
        private bool _isCooldown;
        private float _cooldownTimer;
        [Header("Audio")]
        [SerializeField] private Audio _boostAudio;
        public bool IsActive { get; private set; }

        public void Setup(PlayerComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            var body = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);
            _boostParticles = body.BoostParticles;
            _doubleBoostParticles = body.DoubleBoostParticles;
            _boostParticles.ForEach(p => p.Stop());
            _doubleBoostParticles.ForEach(p => p.Stop());

            _boostCurrentAmount = _boostMaxAmount;
            UpdateBoostUI();

            SetUpInput(_components.PlayerIndex);
            UpdateInput(_components.PlayerIndex);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Sprint.name).performed += StartBoost;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            int index = _components.PlayerIndex;
            InputActionMap playerMap = InputManager.GetMap(index, ActionMap.Player);
            playerMap.FindAction(c.Player.Sprint.name).performed -= StartBoost;
        }

        public void UpdateInput(int playerIndex)
        {
            string key = GameManager.Instance.InputManager.GetButtonBindingKey(
                InputManager.Controls.Player.Sprint.name, playerIndex);
            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetBoostKey(key);
        }
        #endregion


        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (!_isCooldown)
            {
                return;
            }

            _cooldownTimer += Time.deltaTime;

            if(_cooldownTimer >= _boostCooldownDuration)
            {
                _cooldownTimer = 0f;

                if(_boostCurrentAmount < _boostMaxAmount)
                {
                    _boostCurrentAmount++;
                    GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateBoostIcons(_boostCurrentAmount);
                }
                else
                {
                    _isCooldown = false;
                }
            }
        }

        private void StartBoost(InputAction.CallbackContext _)
        {
            if (!IsActive || !_canBoost)
            {
                return;
            }

            if (_boostCurrentAmount <= 0)
            {
                return;
            }

            GameManager.Instance.AudioManager.Play(_boostAudio);

            float multiplier = 1f;

            if (_boostState == BoostState.NotBoosting)
            {
                // apply constraints
                _components.Constraints.ApplyConstraints(true, _constraints);

                _boostParticles.ForEach(p => p.Play());
                _boostState = BoostState.NormalBoost;
                multiplier = SingleBoostSpeedMultiplier;
            }
            else if (_boostState == BoostState.NormalBoost)
            {
                _doubleBoostParticles.ForEach(p => p.Play());
                _boostParticles.ForEach(p => p.Stop());
                GameManager.Instance.EffectsUIController.PlaySpeedLinesEffect();
                _boostState = BoostState.DoubleBoost;
                multiplier = DoubleBoostSpeedMultiplier;
            }
            else if (_boostState == BoostState.DoubleBoost)
            {
                multiplier = DoubleBoostSpeedMultiplier;
            }

            _boostCurrentAmount--;
            _isCooldown = true;
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateBoostIcons(_boostCurrentAmount);
            _components.TankWiggler.WiggleBody(_boostWiggle);
            //_animation.PlayBoostAnimation();
            GameManager.Instance.CameraManager.PlayerCameraFollow.SetSpeedMultiplier(multiplier, _components.PlayerIndex);
            _movement.StartBoost(multiplier, _boostDuration, _boostCurve);
        }

        private void OnBoostFinishedHandler()
        {
            _boostParticles.ForEach(p => p.Stop());
            _doubleBoostParticles.ForEach(p => p.Stop());
            _boostState = BoostState.NotBoosting;
            GameManager.Instance.EffectsUIController.StopSpeedLinesEffect();
            // disable constraints
            _components.Constraints.ApplyConstraints(false, _constraints);
        }

        private void UpdateBoostUI()
        {
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetupBoostIcons(_boostCurrentAmount);
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
                _movement.CancelBoost();
                _movement.StartDeceleration();
            }
        }

        public void Enable(bool enable)
        {
            IsActive = enable;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
            UpdateInput(_components.PlayerIndex);
            SetUpInput(_components.PlayerIndex);
            EnableBoost(true);
            _movement.OnBoostFinished += OnBoostFinishedHandler;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
            _movement.OnBoostFinished -= OnBoostFinishedHandler;
            DisposeInput(_components.PlayerIndex);
        }

        public void Dispose()
        {
        }
        #endregion

        private enum BoostState
        {
            NotBoosting = 0,
            NormalBoost = 1,
            DoubleBoost = 2,
        }
    }
}
