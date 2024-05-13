using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_Introduction", menuName = MENU_PATH + "Three Cannon/Introduction State")]
    public class ThreeCannonBossIntroductionState : BossIntroductionState
    {
        BossAttackController _attackController;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);
            _attackController = _components.AttackController;

            _components.AttackController.OnGroundPoundImpact += OnGroundPoundImpactHandler;
        }

        public override void OnEnter()
        {
            Debug.Log("TAUNT STATE");
            _isActive = true;

            _components.StartCoroutine(IntroductionRoutine());
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
        }

        private IEnumerator IntroductionRoutine()
        {
            ((ThreeCannonBossAnimations)_animations).TriggerGroundPoundAnimation();
            yield return new WaitForSeconds(1f);

            _components.AIController.FinishBossIntroduction();       
        }

        private void OnGroundPoundImpactHandler()
        {
            if (!_isActive)
                return;

            _attackController.GroundPoundParticles.Play();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.GROUND_POUND);
        }

    }
}