using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public abstract class TankMovement : MonoBehaviour, IController
    {
        [Header("Movement")]
        [SerializeField] protected float _movementMaxSpeedDefault;
        [SerializeField] protected float _accelerationDefault;
        [SerializeField] protected float _decelerationDefault;
        protected float _tempMaxMovementSpeed;
        protected float _tempAcceleration;
        protected float _tempDeceleration;
        [Header("Wiggles")]
        [SerializeField] protected Wiggle _forwardWiggle;
        [SerializeField] protected Wiggle _backwardWiggle;
        [Header("Rotation")]
        [SerializeField] protected float _rotationSpeed = 0.2f;
        [Tooltip("Used for enemies when obstacles are detected (with transform.Rotate)")]
        [SerializeField] protected float _turnSpeed = 180f;
        [Header("Gravity")]
        [SerializeField] protected float GroundGravity;
        [SerializeField] protected float Gravity;
        
        protected CharacterController _characterController;
        protected TankComponents _components;
        protected Transform _body;
        protected Transform _turret;
        protected TankAnimation _animation;
        // extras
        [SerializeField] protected float _speedMultiplier = 1;

        protected int _forwardAmount;
        protected int _turnAmount;
        protected int _lastForwardAmount;
        protected Vector3 _currentMovement;
        [field: SerializeField] public float CurrentSpeed { get; protected set; }
        protected Vector3 _movementInput;
        protected float _turnSsmoothVelocity;

        [SerializeField] protected bool _canMove = true;
        [SerializeField] protected bool _canRotate = true;

        protected List<ParticleSystem> _tracksParticles = new List<ParticleSystem>();

        public bool IsActive { get; protected set; }
        public Vector3 MovementInput => _movementInput;

        public virtual void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;
            _forwardAmount = 0;

            _body = parts.GetBodyPartOfType(BodyPartType.Body).transform;
            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
            var carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);
            if(carrier != null)
            {
                if (carrier.TracksParticles != null)
                    _tracksParticles = carrier.TracksParticles;
            }
           
            _components = components;
            _characterController = components.CharacterController;
            _animation = components.Animation;

            _tempMaxMovementSpeed = _movementMaxSpeedDefault;
            _tempAcceleration = _accelerationDefault;
            _tempDeceleration = _decelerationDefault;
        }

        public virtual void MoveCharacterController(Vector3 movementInput)
        {
            if (!_canMove)
            {
                return;
            }

            //Momentum
            if ((_forwardAmount > 0 && _lastForwardAmount >= 0) || (_forwardAmount < 0 && _lastForwardAmount <= 0))
            {
                CurrentSpeed += Time.deltaTime * _accelerationDefault * _accelerationDefault;
            }
            else if (_forwardAmount == 0 || (_forwardAmount > 0 && _lastForwardAmount < 0) || (_forwardAmount < 0 && _lastForwardAmount > 0))
            {
                CurrentSpeed -= Time.deltaTime * _decelerationDefault * _decelerationDefault;
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, 1f);

            Vector3 lastMovementInput = movementInput;

            //Rotation
            if (movementInput.magnitude > 0)
            {
                float targetAngle = Mathf.Atan2(lastMovementInput.x, lastMovementInput.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(_body.eulerAngles.y, targetAngle, ref _turnSsmoothVelocity, _rotationSpeed);
                _body.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            //Movement
            _currentMovement = _body.forward * (_tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime);
            _currentMovement.y = 0f;
            _characterController.Move(_currentMovement);

            //Animation
            _animation.AnimateMovement(lastMovementInput.magnitude != 0, 1, 0, CurrentSpeed * _speedMultiplier);
        }

        public void ObstacleMovement()
        {
            if (!_canMove)
            {
                return;
            }

            //Momentum
            if ((_forwardAmount > 0 && _lastForwardAmount >= 0) || (_forwardAmount < 0 && _lastForwardAmount <= 0))
            {
                CurrentSpeed += Time.deltaTime * _accelerationDefault * _accelerationDefault;
            }
            else if (_forwardAmount == 0 || (_forwardAmount > 0 && _lastForwardAmount < 0) || (_forwardAmount < 0 && _lastForwardAmount > 0))
            {
                CurrentSpeed -= Time.deltaTime * _decelerationDefault * _decelerationDefault;
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, 1f);

            //Movement
            _currentMovement = _lastForwardAmount * _body.forward * (_tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime);
            _currentMovement.y = 0f;

            _characterController.Move(_currentMovement);

            //Animation
            _animation.AnimateMovement(_forwardAmount != 0, _lastForwardAmount, _turnAmount, CurrentSpeed * _speedMultiplier); ;

            ////Rotation
            _body.Rotate(Vector3.up * _turnAmount * _turnSpeed * Time.deltaTime);       
        }

        public void MoveCharacterController(float speed)
        {
            //Movement
            _currentMovement += speed * Time.deltaTime * transform.forward;
            _characterController.Move(_currentMovement * Time.deltaTime);

            //Animation
            _animation.AnimateMovement(_forwardAmount != 0, _forwardAmount, _turnAmount, _speedMultiplier);
        }

        public void MoveCharacterController(Vector3 direction, float speed)
        {
            //Movement
            _currentMovement += speed * Time.deltaTime * direction;
            _characterController.Move(_currentMovement * Time.deltaTime);

            //Animation
            _animation.AnimateMovement(_forwardAmount != 0, _forwardAmount, _turnAmount, _speedMultiplier);
        }

        public void ResetMovement()
        {
            _currentMovement = Vector3.zero;
        }

        #region External Methods
        public void MultiplySpeed(float value)
        {
            _tempMaxMovementSpeed *= value;
        }

        public void SetSpeedMultiplier(float value)
        {
            _speedMultiplier = value;
        }

        public void EnableMovement(bool canMove)
        {
            _canMove = canMove;

            //if (canMove)
            //{
            //    _currentMovement = Vector3.zero;
            //    _currentSpeed = 0f;
            //}
        }

        public virtual void StopMovement()
        {
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0f;
        }

        public void SetSpeed(float speed)
        {
            _movementMaxSpeedDefault = speed;
            _tempMaxMovementSpeed = _movementMaxSpeedDefault;
        }

        public float GetSpeed()
        {
            return _movementMaxSpeedDefault;
        }

        public float GetMultipliedSpeed()
        {
            return _movementMaxSpeedDefault * CurrentSpeed;
        }

        public void EnableRotation(bool enable)
        {
            _canRotate = enable;
        }

        public float GetRotationSpeed()
        {
            return _rotationSpeed;
        }
        #endregion

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
            _characterController.enabled = true;
            _tracksParticles.ForEach(t => t.Play());
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            _characterController.enabled = false;
            _tracksParticles.ForEach(t => t.Stop());
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0f;
            _forwardAmount = 0;
            _turnAmount = 0;
        }

        public virtual void Restart()
        {
            IsActive = false;

            _canMove = true;
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0f;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
