using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using System;
    using UI.InGame;
    
    public class PlayerAimAssist : MonoBehaviour, IController, IInput
    {
        public bool IsActive { get; private set; }
        public bool IsAiming { get; private set; } = false;

        [SerializeField] private Transform _turret;
        [SerializeField] private LayerMask _layersToDetect;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private float _detectionRange = 20f;
        [SerializeField] private float _aimAssistAngle = 45f;
        [SerializeField] private Vector2 _crosshairRadiusRange;

        private PlayerComponents _components;
        private PlayerCrosshairController _crosshairController;
        private Crosshair _crossHair;
        private Transform _tank;
        private float _angleRad;
        private bool _isInputDown = false;

        public void SetUp(PlayerComponents components)
        {
            _components = components;

            _tank = _components.transform;

            _crossHair = _components.Crosshair.GetCrosshair();

            _crosshairController = _components.Crosshair;

            // convert the angle to a rad for the math
            _angleRad = Mathf.Cos((_aimAssistAngle / 2) * Mathf.Deg2Rad);
        }

        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap gameplayMap = InputManager.GetMap(playerIndex, ActionMap.Player);

            gameplayMap.FindAction(c.Player.Aim.name).performed += OnAimAssistInputDown;
            gameplayMap.FindAction(c.Player.Aim.name).canceled += OnAimAssistInputUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap gameplayMap = InputManager.GetMap(_components.PlayerIndex, ActionMap.Player);
            gameplayMap.FindAction(c.Player.Aim.name).performed -= OnAimAssistInputDown;
            gameplayMap.FindAction(c.Player.Aim.name).canceled -= OnAimAssistInputUp;
        }

        private void OnAimAssistInputDown(InputAction.CallbackContext _)
        {
            _isInputDown = true;
            StartCoroutine(AimAssistProcess());
        }

        private void OnAimAssistInputUp(InputAction.CallbackContext obj)
        {
            _isInputDown = false;
            _crossHair.Visuals.StopAiming();
            _crosshairController.DisableIsAiming();
        }

        private IEnumerator AimAssistProcess()
        {
            while (_isInputDown)
            {
                PerformAimAssist();
                yield return null;
            }
        }

        private void PerformAimAssist()
        {
            if (!IsActive)
            {
                return;
            }

            List<Transform> enemies = GameManager.Instance.EnemiesManager.GetSpawnedEnemies();
            Transform target = GetClosestTargetWithinRange(enemies);

            if(target == null)
            {
                List<Transform> droppers = GameManager.Instance.RoomsManager.CurrentRoom.GetDroppers();
                target = GetClosestTargetWithinRange(droppers);
            }

            if (target == null)
            {
                IsAiming = false;
                _crosshairController.DisableIsAiming();
                _crossHair.Visuals.PlayInActiveAimAnimation();
                return;
            }

            _crossHair.Visuals.PlayActiveAimAnimation();
            _crosshairController.EnableIsAiming();
            _crosshairController.SetAimingPosition(target.position);

            IsAiming = true;
        }

        private Transform GetClosestTargetWithinRange(List<Transform> targets)
        {
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

            Transform targetToLockTo = null;

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
                    targetToLockTo = targets[i];
                    break;
                }
            }

            if(targetToLockTo == null)
            {
                return null;
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

            return targetToLockTo;
        }

        public void EnableAimAssist(bool enable)
        {
            IsAiming = enable;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
            SetUpInput(_components.PlayerIndex);
        }

        public void Deactivate()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);
        }

        public void Dispose()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);
        }

        public void Restart()
        {
            IsActive = false;
            DisposeInput(_components.PlayerIndex);
        }
        #endregion
    }
}
