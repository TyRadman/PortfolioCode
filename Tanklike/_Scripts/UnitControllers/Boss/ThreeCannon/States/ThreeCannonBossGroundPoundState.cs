using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Combat;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_GroundPound", menuName = MENU_PATH + "Three Cannon/Ground Pound State")]
    public class ThreeCannonBossGroundPoundState : BossAttackState
    {
        [SerializeField] private AOEWeapon _groundPoundWeapon;
        [SerializeField] private float _groundPoundAttackDuration = 1;
        [SerializeField] protected LayerMask _groundPoundTargetLayers;

        private Transform _target;
        private Coroutine _attackCoroutine;
        private bool _groundPoundIsActive;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);

            _groundPoundWeapon.SetTargetLayer(_groundPoundTargetLayers);

            _attackController.OnGroundPoundImpact += OnGroundPoundImpactHandler;
        }

        public override void OnEnter()
        {
            Debug.Log("SIDE MACHINE GUNS ATTACK STATE");
            _isActive = true;

            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
            //Debug.Log(_target.gameObject.name);

            //_attackController.Attack(ThreeCannonAttackType.GroundPound, OnAttackFinished);
            _attackCoroutine = _attackController.StartCoroutine(GroundPoundAttackRoutine());
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            _isActive = false;
            if (_attackCoroutine != null)
                _attackController.StopCoroutine(_attackCoroutine);
        }

        public override void OnDispose()
        {
        }

        private void OnGroundPoundImpactHandler()
        {
            if (!_isActive)
                return;

            _groundPoundIsActive = false;
        }

        private void OnAttackFinished()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(BossStateType.Move);
        }

        #region Attack Methods
        private IEnumerator GroundPoundAttackRoutine()
        {
            _groundPoundIsActive = true;

            ((ThreeCannonBossAnimations)_animations).Animator.speed = 1 / _groundPoundAttackDuration;

            Indicator indicator = null;

            if (_groundPoundWeapon.IndicatorType != IndicatorEffects.IndicatorType.None)
            {
                Vector3 indicatorSize = new Vector3(_groundPoundWeapon.ExplosionRadius * 2, 0f, _groundPoundWeapon.ExplosionRadius * 2);
                indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(_groundPoundWeapon.IndicatorType);
                indicator.gameObject.SetActive(true);
                var pos = _attackController.transform.position;
                pos.y = 0.52f;
                indicator.transform.position = pos;
                indicator.transform.rotation = Quaternion.identity;
                indicator.transform.localScale = indicatorSize;
                indicator.Play();
            }

            ((ThreeCannonBossAnimations)_animations).TriggerGroundPoundAnimation();

            while (_groundPoundIsActive)
            {
                yield return null;
            }

            _attackController.GroundPoundParticles.Play();
            _groundPoundWeapon.OnShot(_components, _attackController.transform);

            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.GROUND_POUND);
            if (indicator != null)
                indicator.TurnOff();

            yield return new WaitForSeconds(_groundPoundWeapon.CoolDownTime);

            ((ThreeCannonBossAnimations)_animations).Animator.speed = 1f;

            OnAttackFinished();
        }
        #endregion
    }
}
