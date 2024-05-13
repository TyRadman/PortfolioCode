using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Sound;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerMovement : TankMovement, IInput
    {
        public System.Action OnBoostFinished;
        [SerializeField] private LayerMask _bumpingLayers;

        [Header("Testing")]
        [SerializeField] private bool _newControls;
        [SerializeField] private bool _rotationWhileMovingOnly;
        [SerializeField] private bool _staminaSprint;

        [Header("Bumping")]
        [SerializeField] private float _obstacleDetectionDistance = 1f;
        [SerializeField] private float _bumpDragDistance = 0.1f;

        [SerializeField] private PlayerJump _playerJump;
        private Vector3 _lastMovementInput;
        private Vector3 _moveDir;
        private bool _useGravity;
        private Coroutine _boostCoroutine;
        private CollisionEventPublisher _bumper;
        private bool _isTouchingWall;
        private bool _decelerate;
        [SerializeField] private Audio _bumpAudio;
        public System.Action<float, Transform> OnMovement;
        [field: SerializeField] public bool IsMoving { get; private set; } = false;
        public Vector3 LastMovementInput => _lastMovementInput;

        private Vector2 _inputVector;
        private PlayerComponents _playerComponents;
        private bool _isBoosting;


        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _playerComponents = (PlayerComponents)components;
            TankBodyParts parts = components.TankBodyParts;

            SetUpInput(((PlayerComponents)_components).PlayerIndex);
            SetBumper(((TankBody)parts.GetBodyPartOfType(BodyPartType.Body)).Bumper);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction("Movement").performed += OnPlayerMoveInputPerformed;
            playerMap.FindAction("Movement").canceled += OnPlayerMoveInputCanceled;
        }

        public void DisposeInput(int playerIndex)
        {
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction("Movement").performed -= OnPlayerMoveInputPerformed;
            playerMap.FindAction("Movement").canceled -= OnPlayerMoveInputCanceled;
        }
        #endregion

        private void OnPlayerMoveInputPerformed(InputAction.CallbackContext ctx)
        {
            _inputVector = ctx.ReadValue<Vector2>();
        }

        private void OnPlayerMoveInputCanceled(InputAction.CallbackContext ctx)
        {
            _inputVector = Vector2.zero;
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            OnMovement?.Invoke(CurrentSpeed, transform);
            HandleInput();
            HandleMovement();
        }

        public void StartDeceleration()
        {
            _decelerate = true;
        }

        private void HandleMovement()
        {
            if (!_canMove || _decelerate)
            {
                ApplyDeceleration();
            }
            else
            {
                if (!_isBoosting)
                {
                    ApplyWiggle();
                    ApplyMomentum();
                }
            }

            ApplyRotation();
            ApplyMovement();
            ApplyAnimation();
        }

        private void ApplyDeceleration()
        {
            CurrentSpeed -= Time.deltaTime * _decelerationDefault;
            if (CurrentSpeed <= 0)
            {
                CurrentSpeed = 0f;
            }

            if (CurrentSpeed > 0.9f && _forwardAmount == 1)
            {
                if (!_playerJump.IsJumping)
                {
                    _components.TankWiggler.WiggleBody(_forwardWiggle);
                }

                _forwardAmount = 0;
            }

            if (CurrentSpeed <= 1)
            {
                _decelerate = false;
            }
        }

        private void ApplyMovement()
        {
            _currentMovement = _body.forward * (_tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime);
            _currentMovement.y = 0f;
            HandleGravity();
            _characterController.Move(_currentMovement);
        }

        private void ApplyRotation()
        {
            if (_movementInput.magnitude > 0)
            {
                float targetAngle = Mathf.Atan2(_lastMovementInput.x, _lastMovementInput.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(_body.eulerAngles.y, targetAngle, ref _turnSsmoothVelocity, _rotationSpeed);

                _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _moveDir.y = 0;

                _body.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
        }

        private void ApplyAnimation()
        {
            _animation.AnimateMovement(_lastMovementInput.magnitude != 0, 1, 0, CurrentSpeed * _speedMultiplier);
            _animation.AnimateTurretTurn(CurrentSpeed, _turnAmount);
        }

        private void ApplyWiggle()
        {
            if (_movementInput.magnitude > 0)
            {
                if (CurrentSpeed == 0f && _forwardAmount == 0)
                {
                    if (!_playerJump.IsJumping)
                    {
                        _components.TankWiggler.WiggleBody(_backwardWiggle);
                    }

                    _forwardAmount = 1;
                }
            }
            else
            {
                if (CurrentSpeed >= 1f && _forwardAmount == 1)
                {
                    if (!_playerJump.IsJumping)
                    {
                        _components.TankWiggler.WiggleBody(_forwardWiggle);
                    }

                    _forwardAmount = 0;
                }
            }

            if (CurrentSpeed == 0f && _forwardAmount != 0)
            {
                _forwardAmount = 0;
            }
            else if (CurrentSpeed == 1f && _forwardAmount != 1)
            {
                _forwardAmount = 1;
            }
        }
        //private void ApplyWiggle()
        //{
        //    if (_movementInput.magnitude > 0)
        //    {
        //        if (CurrentSpeed == 0f && _forwardAmount == 0)
        //        {
        //            if (!_playerJump.IsJumping)
        //            {
        //                _components.TankWiggler.WiggleBody(_backwardWiggle);
        //            }

        //            _forwardAmount = 1;
        //        }
        //    }
        //    else
        //    {
        //        if (CurrentSpeed == 1f && _forwardAmount == 1)
        //        {
        //            if (!_playerJump.IsJumping)
        //            {
        //                _components.TankWiggler.WiggleBody(_forwardWiggle);
        //            }

        //            _forwardAmount = 0;
        //        }
        //    }

        //    if (CurrentSpeed == 0f && _forwardAmount != 0)
        //    {
        //        _forwardAmount = 0;
        //    }
        //    else if (CurrentSpeed == 1f && _forwardAmount != 1)
        //    {
        //        _forwardAmount = 1;
        //    }
        //}

        public void SetBodyRotation(Quaternion rotation)
        {
            _body.localRotation = rotation;
        }

        private void HandleInput()
        {
            // set IsMoving to true if there is any input from the player or if the player is sprinting
            IsMoving = _inputVector.magnitude > 0f;

            // check if there are any constraints on the tank's movement or rotaiton
            if (!_canMove)
            {
                _movementInput = Vector3.zero;
                _inputVector.y = 0f;
                return;
            }

            _movementInput = new Vector3(_inputVector.x, 0, _inputVector.y);

            _movementInput.Normalize();

            if (_movementInput.magnitude > 0)
            {
                _lastMovementInput = _movementInput;
            }
        }

        public void RotatePlayer(float direction)
        {
            transform.Rotate(_rotationSpeed * direction * Time.deltaTime * Vector3.up);
        }

        private void ApplyMomentum()
        {
            if (_movementInput.magnitude > 0)
            {
                CurrentSpeed += Time.deltaTime * _accelerationDefault;
            }
            else
            {
                CurrentSpeed -= Time.deltaTime * _decelerationDefault;
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, 1f);
        }

        private void HandleGravity()
        {
            if (!_useGravity)
            {
                return;
            }

            if (_characterController.isGrounded)
            {
                _currentMovement.y = GroundGravity;
            }
            else
            {
                _currentMovement.y = Gravity;
            }
        }

        public void EnableGravity(bool value)
        {
            _useGravity = value;
        }

        public void StartBoost(float boostMultiplier, float boostDuration, AnimationCurve boostCurve)
        {
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0.5f;

            if (_boostCoroutine != null)
            {
                StopCoroutine(_boostCoroutine);
            }

            _boostCoroutine = StartCoroutine(BoostMovementRoutine(boostMultiplier, boostDuration, boostCurve));
        }

        private IEnumerator BoostMovementRoutine(float boostMultiplier, float boostDuration, AnimationCurve boostCurve)
        {
            _bumper.gameObject.SetActive(true);

            _isBoosting = true;
            float timer = 0f;
            _forwardAmount = 1;
            _lastForwardAmount = 1;
            bool stopBoost = false;
            float accelerationTime = 0.25f;
            
            while(timer < accelerationTime)
            {
                timer += Time.deltaTime;
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, 1f, timer / accelerationTime);
                yield return null;
            }

            timer = 0f;
            CurrentSpeed = 1f;

            while (timer < boostDuration && !stopBoost)
            {
                float t = boostCurve.Evaluate(timer / boostDuration);

                CurrentSpeed = 1f + t * boostMultiplier;

                timer += Time.deltaTime;
                yield return null;
            }

            StopBoost();
        }

        public void CancelBoost()
        {
            if (_boostCoroutine != null)
            {
                StopCoroutine(_boostCoroutine);
            }

            StopBoost();
        }

        private void StopBoost()
        {
            _isBoosting = false;
            _bumper.gameObject.SetActive(false);
            OnBoostFinished?.Invoke();
            GameManager.Instance.CameraManager.PlayerCameraFollow.ResetSpeedMultiplier(_playerComponents.PlayerIndex);
        }


        public void SetBumper(CollisionEventPublisher bumper)
        {
            _bumper = bumper;
            _bumper.gameObject.SetActive(false);
        }

        private void OnBumpHandler(Collider other)
        {
            if (_isTouchingWall)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                _isTouchingWall = true;
            }
        }

        private IEnumerator BumpingRoutine()
        {
            if (_boostCoroutine != null)
            {
                StopCoroutine(_boostCoroutine);
            }

            GameManager.Instance.AudioManager.Play(_bumpAudio);
            IsActive = false;
            StopBoost();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.HIT);
            _currentMovement = _lastForwardAmount * _body.forward * _bumpDragDistance;
            _currentMovement.y = 0f;
            HandleGravity();
            _characterController.Move(_currentMovement); 
            _currentMovement = Vector3.zero;
            _turnAmount = 0;
            CurrentSpeed = 0f;
            _animation.AnimateTurretMotion(-_forwardAmount);
            _forwardAmount = 0;
            _animation.AnimateMovement(_forwardAmount != 0, _lastForwardAmount, _turnAmount, CurrentSpeed * _speedMultiplier);
            _animation.PlayBumpAnimation();
            yield return new WaitForSeconds(0.5f);
            IsActive = true;
        }

        #region IController
        public override void Activate()
        {
            base.Activate();
            SetUpInput(((PlayerComponents)_components).PlayerIndex);
            EnableGravity(true);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            StopBoost();
            if (_boostCoroutine != null)
                StopCoroutine(_boostCoroutine);
        }

        public override void Dispose()
        {
            base.Dispose();
            DisposeInput(((PlayerComponents)_components).PlayerIndex);
        }

        public override void Restart()
        {
            base.Restart();
            IsActive = false;
            _bumper.OnTriggerEnterEvent -= OnBumpHandler;
        }
        #endregion
    }
}
