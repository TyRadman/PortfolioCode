using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TankLike.Combat;
using TankLike.Utils;
using TankLike.Misc;
using TankLike.Cam;
using static TankLike.PlayersManager;

namespace TankLike.UnitControllers
{
    public enum ThreeCannonAttackType
    {
        MainMachineGun,
        SideMachineGuns,
        GroundPound,
        RocketLauncher,
    }

    public class BossAttackController : MonoBehaviour, IController
    {
        public Action OnAttackFinished;
        public Action OnGroundPoundTriggered;
        public Action OnGroundPoundImpact;

        [field: SerializeField, Header("Machine Gun Attack")] public Transform MachineGunShootingPoint { get; private set; }
        [field: SerializeField] public Transform MachineGunTransform { get; private set; }

        [field: SerializeField, Header("Side Machine Guns Attack")] public Transform RightMachineGunShootingPoint { get; private set; }
        [field: SerializeField] public Transform LeftMachineGunShootingPoint { get; private set; }
        [field: SerializeField] public Transform RightMachineGunTransform { get; private set; }
        [field: SerializeField] public Transform LeftMachineGunTransform { get; private set; }
        [field: SerializeField] public Transform RightTurretTransform { get; private set; }
        [field: SerializeField] public Transform LeftTurretTransform { get; private set; }

        [field: SerializeField, Header("Ground Pound")] public ParticleSystem GroundPoundParticles { get; private set; }
        [field: SerializeField] public CollisionEventPublisher GroundPoundTrigger { get; private set; }

        [field: SerializeField, Header("Rocket Launcher Attack")] public Transform RocketLauncherShootingPoint { get; private set; }


        private BossComponents _components;
        private EnemyShooter _shooter;
        private ThreeCannonBossAnimations _animations;
        private PlayerTransforms _currentTarget;
        private Coroutine _attackCoroutine;
        private Coroutine _machineGunAnimationCoroutine;
        private Coroutine _sideMachineGunsAnimationCoroutine;
        private Vector3 _roomCenter;
        private Vector3 _roomSize;
        public bool IsActive { get; private set; }

        public void SetUp(BossComponents components)
        {
            _components = components;
            _roomCenter = components.RoomCenter;
            _roomSize = components.RoomSize;
            _shooter = (EnemyShooter)_components.Shooter;
            _animations = (ThreeCannonBossAnimations)_components.Animations;

            //_groundPoundWeapon.SetTargetLayer(_groundPoundTargetLayers);

            GroundPoundTrigger.OnTriggerEnterEvent += OnGroundPoundTriggerEnter;
            EnableGroundPoundTrigger(false);

            var pubs = _animations.Animator.GetBehaviours<AnimatorEventPublisher>();

            foreach (AnimatorEventPublisher publisher in pubs)
            {
                if (publisher.StateName == "Ground Pound")
                {
                    publisher.OnSpecialEvent += GroundPoundImpactHandler;
                }
            }
        }

        public void SetTarget(PlayerTransforms target)
        {
            _currentTarget = target;
        }

        public void UnsetTarget()
        {
            _currentTarget = null;
        }

        public void Attack(ThreeCannonAttackType attackType, Action OnAttackCompleted)
        {
            OnAttackFinished = OnAttackCompleted;
        }

        #region Main Machine Gun Attack
        public void SetMachineGunValues(float shootingDuration)
        {
            //_machineGunShootingDuration = shootingDuration;
        }
        #endregion

        #region Side Machine Guns Attack
        public void SetSideMachineGunsValues(float shootingDuration)
        {
            //_sideMachineGunsShootingDuration = shootingDuration;
        }
        #endregion

        #region Ground Pound Attack
        public void EnableGroundPoundTrigger(bool value)
        {
            GroundPoundTrigger.EnableCollider(value);
        }

        private void OnGroundPoundTriggerEnter(Collider target)
        {
            Debug.Log("Perform Ground Pound");
            OnGroundPoundTriggered?.Invoke();
            EnableGroundPoundTrigger(false);
        }

        private void GroundPoundImpactHandler()
        {
            OnGroundPoundImpact?.Invoke();
        }
        #endregion

        #region Rocket Launcher Attack
      
        #endregion

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

            //Stop all the coroutines
            StopAllCoroutines();

            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            if (_machineGunAnimationCoroutine != null)
                StopCoroutine(_attackCoroutine);

            if (_sideMachineGunsAnimationCoroutine != null)
                StopCoroutine(_attackCoroutine);

            //Unsubscribe to all events
            GroundPoundTrigger.OnTriggerEnterEvent -= OnGroundPoundTriggerEnter;

            var pubs = _animations.Animator.GetBehaviours<AnimatorEventPublisher>();

            foreach (AnimatorEventPublisher publisher in pubs)
            {
                if (publisher.StateName == "Ground Pound")
                {
                    publisher.OnSpecialEvent -= GroundPoundImpactHandler;
                }
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
