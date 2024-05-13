using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankAnimation : MonoBehaviour, IController
    {
        [SerializeField] protected Animator _tankAnimator;

        //Turret
        protected Animator _turretAnimator;
        protected readonly int _moveForward = Animator.StringToHash("MoveForward");
        protected readonly int _moveBackward = Animator.StringToHash("MoveBackward");
        protected readonly int _turnAmount = Animator.StringToHash("TurnAmount");
        protected readonly int _shootHash = Animator.StringToHash("Shoot");

        //Carrier
        protected readonly int _moveHash = Animator.StringToHash("Move");
        protected readonly int _speedHash = Animator.StringToHash("Speed");
        protected readonly int _rotationHash = Animator.StringToHash("Rotation");
        protected readonly int _speedMultiplierHash = Animator.StringToHash("SpeedMultiplier");
        protected readonly int _leftWheelHash = Animator.StringToHash("LeftWheel");
        protected readonly int _rightWheelHash = Animator.StringToHash("RightWheel");
        protected Animator _carrierAnimator;

        private List<ParticleSystem> _dustParticles = new List<ParticleSystem>();
        [SerializeField] private Vector2 _dustCountRange;

        public bool IsActive { get; private set; }

        public void AnimateMovement(bool move, float lastForwardAmount, float rotation, float speedMultiplier)
        {
            if (_carrierAnimator == null) return;

            _carrierAnimator.SetBool(_moveHash, true);
            //_carrierAnimator.SetFloat(_speedHash, 1);
            //_carrierAnimator.SetFloat(_rotationHash, rotation);
            if (move)
            {
                _carrierAnimator.SetLayerWeight(1, 1);
                _carrierAnimator.SetLayerWeight(2, 1);
                //_carrierAnimator.SetFloat(_speedMultiplierHash, speedMultiplier * lastForwardAmount);
                _carrierAnimator.SetFloat(_leftWheelHash, speedMultiplier * lastForwardAmount);
                _carrierAnimator.SetFloat(_rightWheelHash, speedMultiplier * lastForwardAmount);
            }
            else
            {
                if (rotation > 0)
                {
                    _carrierAnimator.SetLayerWeight(1, 1);
                    _carrierAnimator.SetLayerWeight(2, 1);
                    _carrierAnimator.SetFloat(_leftWheelHash, 1);
                    _carrierAnimator.SetFloat(_rightWheelHash, speedMultiplier * lastForwardAmount);
                }
                else if (rotation < 0)
                {
                    _carrierAnimator.SetLayerWeight(1, 1);
                    _carrierAnimator.SetLayerWeight(2, 1);
                    _carrierAnimator.SetFloat(_leftWheelHash, speedMultiplier * lastForwardAmount);
                    _carrierAnimator.SetFloat(_rightWheelHash, 1);
                }
                else
                {
                    _carrierAnimator.SetFloat(_leftWheelHash, speedMultiplier * lastForwardAmount);
                    _carrierAnimator.SetFloat(_rightWheelHash, speedMultiplier * lastForwardAmount);
                }
            }

        }

        public void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            var carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);
            var turret = (TankTurret)parts.GetBodyPartOfType(BodyPartType.Turret);

            if(carrier != null)
            {
                _carrierAnimator = carrier.Animator;
                _dustParticles = carrier.DustParticles;
            }
            _turretAnimator = turret.Animator;
        }

        public void AnimateTurretMotion(int forwardAmount)
        {
            if (forwardAmount == 1)
                _turretAnimator.SetTrigger(_moveForward);

            else if (forwardAmount == -1)
                _turretAnimator.SetTrigger(_moveBackward);
        }

        public void AnimateTurretTurn(float currentSpeed, int turnAmount)
        {
            //_turretAnimator.SetLayerWeight(2, 1);
            //_turretAnimator.SetFloat(_turnAmount, turnAmount);

            //if (currentSpeed > 0.1)
            //{
            //    _turretAnimator.SetFloat(_turnAmount, turnAmount);
            //}
            //else
            //{
            //    _turretAnimator.SetFloat(_turnAmount, 0);
            //}
        }

        public void EnableDustParticles(bool enable)
        {
            for (int i = 0; i < _dustParticles.Count; i++)
            {
                ParticleSystem dust = _dustParticles[i];
                var emission = dust.emission;
                emission.rateOverDistance = enable ? _dustCountRange.y : _dustCountRange.x;
                //dust.emission = emission;
            }
            //if (enable)
            //{
            //    _dustParticles.ForEach(p => p.Play());
            //}
            //else
            //{
            //    _dustParticles.ForEach(p => p.Stop());
            //}
        }

        public void StopAnimations(bool value)
        {
            if (_carrierAnimator == null || _turretAnimator == null) return;

            if(value)
            {
                _carrierAnimator.speed = 0f;
                _turretAnimator.speed = 0f;
            }
            else
            {
                _carrierAnimator.speed = 1f;
                _turretAnimator.speed = 1f;
            }
        }

        public void PlayBumpAnimation()
        {
            _tankAnimator.Play("Bump");
        }

        public void PlayBoostAnimation()
        {
            _tankAnimator.Play("Sprint");
        }

        public void PlayShootAnimation(float speed = 1f)
        {
            _turretAnimator.speed = speed;
            _turretAnimator.SetLayerWeight(1, 1);
            _turretAnimator.SetTrigger(_shootHash);
        }

        public void StopShootAnimation()
        {
            _turretAnimator.SetLayerWeight(1, 0);
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;

            _turretAnimator.ResetTrigger(_moveForward);
            _turretAnimator.ResetTrigger(_moveBackward);
            _turretAnimator.SetLayerWeight(2, 0);
            _turretAnimator.SetFloat(_turnAmount, 0f);

            _turretAnimator.SetLayerWeight(1, 0);
            _turretAnimator.ResetTrigger(_shootHash);

            if (_carrierAnimator != null)
            {
                _carrierAnimator.SetLayerWeight(1, 0);
                _carrierAnimator.SetLayerWeight(2, 0);
                _carrierAnimator.SetBool(_moveHash, false);
                _carrierAnimator.SetFloat(_leftWheelHash, 0f);
                _carrierAnimator.SetFloat(_rightWheelHash, 0f);
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
