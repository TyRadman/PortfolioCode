using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.UI.InGame;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// Controls the cross hair of the player
    /// </summary>
    public class PlayerCrosshairController : MonoBehaviour, IController, IInput
    {
        public bool IsActive { get; private set; }

        [Header("Crosshair")]
        [SerializeField] private Vector2 _crosshairRadiusRange;
        [SerializeField] private float _crosshairSpeed = 100f;
        [Range(0f, 100f)] 
        [SerializeField] private float _crosshairSnapSpeed = 4f;

        [Header("Turret")]
        private Vector3 _offset;

        [Header("References")]
        [SerializeField] private PlayerTurretController _turretController;
        [SerializeField] private Crosshair _crossHair;
        [SerializeField] private Transform _turret;

        [Header("Aim assist")]
        [SerializeField] private float _aimAssistAngle = 45f;
        [SerializeField] private bool _aimAssistActive = true;

        [Tooltip("How strongly the aim assist influences the crosshair controls")]
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerComponents _components;
        [SerializeField] private float _aimInfluence = 5f;
        [SerializeField] private float _detectionRange = 15f;
        [SerializeField] private float _timeBeforeAimAssist = 1f;
        [SerializeField] private float _crosshairSpeedMultiplier = 1f;

        private Transform _tank;
        private Vector2 _originalRadiusRange;
        private bool _isAiming = false;

        public const float MIN_AIM_SENSITIVITY = 2f;
        public const float MAX_AIM_SENSITIVITY = 40f;

        #region Input
        private Vector3 _input;
        #endregion

        private void Awake()
        {
            _tank = transform;
        }

        public void SetUp()
        {
            _originalRadiusRange = _crosshairRadiusRange;
            _crossHair.SetUp();
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.TurretRotation.name).performed += UpdateInput;
            playerMap.FindAction(c.Player.TurretRotation.name).canceled += ResetInput;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.TurretRotation.name).performed -= UpdateInput;
            playerMap.FindAction(c.Player.TurretRotation.name).canceled -= ResetInput;
        }
        #endregion

        private void Update()
        {
            MoveCrosshair();
            HandleMovement();
        }

        #region Aim Assist Process
        public void EnableIsAiming()
        {
            _isAiming = true;
        }

        public void DisableIsAiming()
        {
            _isAiming = false;
        }

        private Vector3 _aimPosition;
        public void SetAimingPosition(Vector3 position)
        {
            _aimPosition = position;
        }
        #endregion

        private void UpdateInput(InputAction.CallbackContext ctx)
        {
            _input = ctx.ReadValue<Vector2>();
        }

        private void ResetInput(InputAction.CallbackContext ctx)
        {
            _input = Vector3.zero;
        }

        private void MoveCrosshair()
        {
            if (!IsActive)
            {
                return;
            }

            // convert the input into a vector3
            Vector3 inputDirection = new Vector3(_input.x, 0f, _input.y);
            // the offset to be applied to the cursor's position
            Vector3 lookDirection = inputDirection * _crosshairSpeed * _crosshairSpeedMultiplier * Time.deltaTime;
            // add the aim assist influence to the aiming process

            // cache the turret forward direction
            Vector3 turretForward = _turret.forward;
            // add the offset to the total offset that will apply to the cursor's 
            _offset += lookDirection;

            // if the offset added to the crosshair is less than the minimum range
            if (_offset.magnitude < _crosshairRadiusRange.x)
            {
                float angle;
                float firstAngle = Vector3.SignedAngle(inputDirection, turretForward, Vector3.up);

                if (firstAngle > 0)
                {
                    angle = Mathf.Atan2(_offset.z, _offset.x) + (lookDirection.magnitude / _crosshairRadiusRange.x);
                }
                else
                {
                    angle = Mathf.Atan2(_offset.z, _offset.x) - (lookDirection.magnitude / _crosshairRadiusRange.x);
                }

                _offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _crosshairRadiusRange.x;
            }
            // if the offset to be added to the crosshair position is higher than the maximum range
            else if (_offset.magnitude > _crosshairRadiusRange.y)
            {
                // Clamp to the maximum radius
                _offset = _offset.normalized * _crosshairRadiusRange.y;
            }
        }

        private void HandleMovement()
        {
            if (_isAiming)
            {
                _crossHair.transform.position = Vector3.Lerp(_crossHair.transform.position, _aimPosition, _crosshairSnapSpeed * Time.deltaTime);
            }
            else
            {
                // move the cursor
                Vector3 newPosition = _tank.position + _offset;
                // apply movement to the crosshair
                _crossHair.transform.position = Vector3.Lerp(_crossHair.transform.position, newPosition, _crosshairSnapSpeed * Time.deltaTime);
                // apply rotation to the turret 
            }

            _turretController.HandleTurretRotation(_crossHair.transform);
        }

        public Transform GetCrosshairTransform()
        {
            return _crossHair.transform;
        }

        public Crosshair GetCrosshair()
        {
            return _crossHair;
        }

        public void SetAimRange(Vector2 radiusRange)
        {
            _crosshairRadiusRange = radiusRange;
        }

        public void ResetAimRange()
        {
            _crosshairRadiusRange = _originalRadiusRange;
        }

        public void EnableCrosshair(bool enable)
        {
            _crossHair.Enable(enable);
        }

        public void SetCrosshairSpeedMultiplier(float crosshairSpeedMultiplier)
        {
            _crosshairSpeedMultiplier = crosshairSpeedMultiplier;
        }

        public void ResetCrosshairSpeedMultiplier()
        {
            _crosshairSpeedMultiplier = 1f;
        }

        public void SetAimSensitivity(float sensitivity)
        {
            _crosshairSpeed = sensitivity;
        }

        public void Enable(bool enable)
        {
            IsActive = enable;
        }

        public void SetColor(Color color)
        {
            _crossHair.SetColor(color);
        }

        public void PlayShootingAnimation()
        {
            _crossHair.Visuals.PlayShootAnimation();
        }

        public void PlayReloadAnimation()
        {
            _crossHair.Visuals.PlayOnShotReloadAnimation();
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
            SetUpInput(_components.PlayerIndex);
            _crossHair.gameObject.SetActive(true);
            EnableCrosshair(true);
        }

        public void Deactivate()
        {
            IsActive = false;
            EnableCrosshair(false);
        }

        public void Restart()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);
            EnableCrosshair(false);
        }

        public void Dispose()
        {

        }
        #endregion
    }
}
