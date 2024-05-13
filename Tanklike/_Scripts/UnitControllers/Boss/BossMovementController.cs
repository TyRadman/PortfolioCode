using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class BossMovementController : EnemyMovement
    {
        public System.Action OnTargetFaced;

        [Header("Boss")]
        [SerializeField] private float _walkableAreaRadius;

        protected const float ROTATION_CORRECTION_THRESHOLD = 0.99f;

        private bool _targetIsFaced;
        private ThreeCannonBossAnimations _animations;
        public float WalkableAreaRadius => _walkableAreaRadius;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _animations = (ThreeCannonBossAnimations)((BossComponents)components).Animations;
        }

        public void FaceTarget(Transform target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 tankForward = transform.forward.normalized;
          
            // Calculate the dot product and angle between tankForward and direction
            float dot = Vector3.Dot(tankForward, direction);
            float angle = Vector3.SignedAngle(tankForward, direction, Vector3.up);
            float rotationAmount = 0f;

            //Debug.Log(dot);
            // Check if the dot product is within the rotation threshold
            if(dot < ROTATION_CORRECTION_THRESHOLD)
            {
                if (angle > 0f)
                {
                    rotationAmount = 1f;
                }
                else if (angle < 0f)
                {
                    rotationAmount = -1f;
                }
            }
            else
            {
                if (angle > 0f)
                {
                    rotationAmount = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(ROTATION_CORRECTION_THRESHOLD, 1, dot));
                }
                else if (angle < 0f)
                {
                    rotationAmount = Mathf.Lerp(-1f, 0f, Mathf.InverseLerp(ROTATION_CORRECTION_THRESHOLD, 1, dot));
                }

                if (!_targetIsFaced && Mathf.Abs(rotationAmount) < 0.2f)
                {
                    Debug.Log("Target Faced");
                    OnTargetFaced?.Invoke();
                    _targetIsFaced = true;
                }
            }

            transform.Rotate(_turnSpeed * rotationAmount * Time.deltaTime * Vector3.up);
        }

        public void FaceTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 tankForward = transform.forward.normalized;
            // Calculate the dot product and angle between tankForward and direction
            float dot = Vector3.Dot(tankForward, direction);
            float angle = Vector3.SignedAngle(tankForward, direction, Vector3.up);
            float rotationAmount = 0f;

            // Check if the dot product is within the rotation threshold
            if (dot < ROTATION_CORRECTION_THRESHOLD)
            {
                if (angle > 0f)
                {
                    rotationAmount = 1f;
                }
                else if (angle < 0f)
                {
                    rotationAmount = -1f;
                }
            }
            else
            {
                if (angle > 0f)
                {
                    rotationAmount = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(ROTATION_CORRECTION_THRESHOLD, 1, dot));
                }
                else if (angle < 0f)
                {
                    rotationAmount = Mathf.Lerp(-1f, 0f, Mathf.InverseLerp(ROTATION_CORRECTION_THRESHOLD, 1, dot));
                }

                if (!_targetIsFaced && Mathf.Abs(rotationAmount) < 0.2f)
                {
                    OnTargetFaced?.Invoke();
                    _targetIsFaced = true;
                }
            }

            transform.Rotate(_turnSpeed * rotationAmount * Time.deltaTime * Vector3.up);
        }

        public void ResetTargetIsFaced()
        {
            Debug.Log("Reset target is faced");
            _targetIsFaced = false;
        }

        public void StartMoveAnimation(bool value)
        {
            _animations.PlayMoveAnimation(value);
        }

        public override void MoveCharacterController(Vector3 movementInput)
        {
            if (!_canMove || !_isMoving)
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
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSsmoothVelocity, _rotationSpeed);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            //Movement
            _currentMovement = transform.forward * (_tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime);
            _currentMovement.y = GroundGravity;
            _characterController.Move(_currentMovement);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3.zero, _walkableAreaRadius);
        }
    }
}
