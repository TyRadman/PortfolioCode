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
        [SerializeField] private float _crosshairSpeed = 100f;
        [SerializeField] private Vector2 _crosshairRadiusRange;
        private Vector2 _originalRadiusRange;
        [Range(0f, 100f)] [SerializeField] private float _crosshairSnapSpeed = 4f;
        [Header("Turret")]
        private Vector3 _offset;
        [Header("References")]
        [SerializeField] private Crosshair _crossHair;
        [SerializeField] private PlayerTurretController _turretController;
        [SerializeField] private Transform _turret;
        public const float MIN_AIM_SENSITIVITY = 2f;
        public const float MAX_AIM_SENSITIVITY = 40f;
        [Header("Aim assist")]
        [SerializeField] private bool _aimAssistActive = true;
        [SerializeField] private float _aimAssistAngle = 45f;
        [Tooltip("How strongly the aim assist influences the crosshair controls")]
        [SerializeField] private float _aimInfluence = 5f;
        [SerializeField] private float _detectionRange = 15f;
        [SerializeField] private LayerMask _layersToDetect;
        [SerializeField] private LayerMask _targetLayer;
        private Transform _tank;
        private float _angleRad;
        private Vector3 _aimAssistPosition;
        [SerializeField] private float _crosshairSpeedMultiplier = 1f;
        private bool _isAiming = false;
        private float _aimAssistTimer = 0f;
        [SerializeField] private float _timeBeforeAimAssist = 1f;
        private bool _isActive = true;
        [SerializeField] private PlayerMovement _movement;
        private bool _aimAssistEnabled = true;
        [SerializeField] private PlayerComponents _components;

        #region Input
        private Vector3 _input;
        #endregion

        private void Awake()
        {
            _tank = transform;
            _isActive = true;
        }

        public void SetUp()
        {
            Activate();

            _originalRadiusRange = _crosshairRadiusRange;
            _crossHair.transform.parent = null;

            // aim assist
            _angleRad = Mathf.Cos((_aimAssistAngle / 2) * Mathf.Deg2Rad);
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
        /// <summary>
        /// Returns true if there is an enemy within the aim assist range
        /// </summary>
        /// <returns></returns>
        private void AimAssistProcess()
        {
            if (!_aimAssistActive || !_aimAssistEnabled)
            {
                return;
            }

            // find all the enemies within sight
            Transform enemy = GetClosestEnemyWithinRange();

            if (enemy == null)
            {
                _isAiming = false;
                return;
            }

            _isAiming = true;
            Vector3 direction = (enemy.position - _tank.position).normalized;
            _aimAssistPosition =  direction * _crosshairRadiusRange.y;
        }


        private Transform GetClosestEnemyWithinRange()
        {
            List<Transform> targets = GameManager.Instance.EnemiesManager.GetSpawnedEnemies();
            targets.AddRange(GameManager.Instance.RoomsManager.CurrentRoom.GetDroppers());

            if (targets.Count == 0)
            {
                return null;
            }

            // filter out far enemies
            targets = targets.FindAll(e => Vector2.Distance(
                new Vector2(e.position.x, e.position.z),
                new Vector2(_tank.position.x, _tank.position.z)) <= _detectionRange);

            if (targets.Count == 0)
            {
                return null;
            }

            targets = targets.OrderBy(e => (e.position - _tank.position).sqrMagnitude).ToList();

            Transform enemyToLockTo = null;

            for (int i = 0; i < targets.Count; i++)
            {
                Vector3 direcitonToEnemy = (targets[i].position - _tank.position).normalized;
                Vector3 tankForward = _turret.forward;

                if (Physics.Raycast(_tank.position, direcitonToEnemy, out RaycastHit hit, _detectionRange, _layersToDetect))
                {
                    // FUTURE REMINDER
                    //  hit.collider.gameObject.layer = 10 // assuming it's an enemy
                    // 1 << 10 = 00000000 00000000 00000100 00000000 // turning the int value into an index in a 32 binary map sort of thing
                    // _targetLayer.value = 00000000 00000000 00000101 00000000 // indices of the wall and enemy layer in a 32 binary map
                    // & = AND in binary
                    // 00000000 00000000 00000100 00000000 AND 00000000 00000000 00000101 00000000
                    // = 00000000 00000000 00000100 00000000
                    // if it equals 0 then there are no mutual layers
                    if ((_targetLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
                    {
                        Debug.DrawRay(_tank.position, direcitonToEnemy * _detectionRange, Color.red);
                        continue;
                    }
                    else
                    {
                        Debug.DrawRay(_tank.position, direcitonToEnemy * _detectionRange, Color.yellow);
                    }
                }

                float dotProduct = Vector3.Dot(tankForward, direcitonToEnemy);

                if (dotProduct >= _angleRad)
                {
                    Debug.DrawRay(_tank.position, direcitonToEnemy * _detectionRange, Color.green);
                    enemyToLockTo = targets[i];
                    break;
                }
            }


            Vector3 direction = _crossHair.transform.position - _tank.position;
            direction.y = 0f;
            direction.Normalize(); // Normalize to get the direction unit vector

            // Calculate perpendicular vectors to the direction for left and right spread
            Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x); // This creates a perpendicular vector on the XZ plane
            perpendicular.Normalize(); // Normalize the perpendicular vector

            // Calculate the left and right direction vectors using the aim assist angle
            float angleRadians = _aimAssistAngle / 2; // Convert angle from degrees to radians
            Vector3 leftDirection = Quaternion.Euler(0, -angleRadians, 0) * direction;
            Vector3 rightDirection = Quaternion.Euler(0, angleRadians, 0) * direction;

            // Draw rays from the tank's position in the left and right directions
            Debug.DrawRay(_tank.position, leftDirection * _detectionRange, Color.blue);
            Debug.DrawRay(_tank.position, rightDirection * _detectionRange, Color.blue);

            return enemyToLockTo;
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
            if (!_isActive)
            {
                return;
            }


            if (_input.magnitude == 0f && !_movement.IsMoving)
            {
                if (_aimAssistTimer >= _timeBeforeAimAssist)
                {
                    AimAssistProcess();
                }
                else
                {
                    _aimAssistTimer += Time.deltaTime;
                }
            }
            else
            {
                _aimAssistTimer = 0f;
                _isAiming = false;
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
                _offset = Vector3.Lerp(_offset, _aimAssistPosition, _aimInfluence * Time.deltaTime);
            }

            // move the cursor
            Vector3 newPosition = _tank.position + _offset;
            // apply movement to the crosshair
            _crossHair.transform.position = Vector3.Lerp(_crossHair.transform.position, newPosition, _crosshairSnapSpeed * Time.deltaTime);
            // apply rotation to the turret 
            _turretController.HandleTurretRotation(_crossHair.transform);
        }

        public Transform GetCrosshair()
        {
            return _crossHair.transform;
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

        public void EnableAimAssist(bool enable)
        {
            _aimAssistActive = enable;
            _isAiming = enable;
        }

        public void Enable(bool enable)
        {
            _isActive = enable;
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
