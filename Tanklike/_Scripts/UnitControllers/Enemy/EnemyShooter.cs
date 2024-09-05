using System.Collections;
using System.Collections.Generic;
using TankLike.Misc;
using UnityEngine;
using static TankLike.IndicatorEffects;
using static TankLike.PlayersManager;

namespace TankLike.UnitControllers
{
    public class EnemyShooter : TankShooter
    {
        public System.Action OnTelegraphFinished;
        public System.Action OnAttackFinished;

        [Header("Telegraphing")]
        [SerializeField] protected float _telegraphDuration = 1f;
        [SerializeField] protected float _telegraphOffset = 0.3f;

        [SerializeField] protected LayerMask _obstacleLayers;

        protected Coroutine _telegraphCoroutine;
        protected Coroutine _attackCoroutine;
        protected PlayerTransforms _currentTarget;

        public bool IsWayToTargetBlocked(Transform target)
        {
            bool rightBlocked;
            bool leftBlocked;

            //right-ray
            var rightDir = (target.position - (transform.position + transform.right));
            rightDir.y = 0.5f;
            rightDir.Normalize();

            float rightDist = Vector3.Distance(transform.position + transform.right, target.position);
            if (Physics.Raycast(transform.position + transform.right, rightDir, rightDist, _obstacleLayers))
            {
                rightBlocked = true;
                Debug.DrawRay(transform.position + transform.right, rightDir * rightDist, Color.red);
            }
            else
            {

                rightBlocked = false;
                Debug.DrawRay(transform.position + transform.right, rightDir * rightDist, Color.yellow);
            }

            //left-ray
            var leftDir = (target.position - (transform.position - transform.right));
            leftDir.y = 0.5f;
            leftDir.Normalize();

            float leftDist = Vector3.Distance(transform.position - transform.right, target.position);
            if (Physics.Raycast(transform.position - transform.right, leftDir, leftDist, _obstacleLayers))
            {
                leftBlocked = true;
                Debug.DrawRay(transform.position - transform.right, leftDir * leftDist, Color.red);
            }
            else
            {

                leftBlocked = false;
                Debug.DrawRay(transform.position - transform.right, leftDir * leftDist, Color.yellow);
            }

            return rightBlocked || leftBlocked;
        }

        public void SetCurrentTarget(PlayerTransforms target)
        {
            _currentTarget = target;
        }

        public PlayerTransforms GetCurrentTarget()
        {
            return _currentTarget;
        }

        public void UnsetCurrentTarget()
        {
            _currentTarget = null;
        }

        public virtual void TelegraphAttack()
        {
            if (_telegraphCoroutine != null)
                StopCoroutine(_telegraphCoroutine);
            _telegraphCoroutine = StartCoroutine(TelegraphRoutine());
        }

        protected virtual IEnumerator TelegraphRoutine()
        {
            ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Telegraphs.EnemyTelegraph;
            vfx.transform.parent = _shootingPoints[0];
            vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            vfx.transform.position += vfx.transform.forward * _telegraphOffset;
            vfx.gameObject.SetActive(true);
            vfx.Play(vfx.Particles.main.duration / _telegraphDuration);
            _activePoolables.Add(vfx);

            yield return new WaitForSeconds(_telegraphDuration);

            OnTelegraphFinished?.Invoke();
            vfx.TurnOff();
            _activePoolables.Remove(vfx);
        }

        public void StartAttackRoutine(int attacksAmount, float breakBetweenAttacks)
        {
            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);
            _attackCoroutine = StartCoroutine(AttackRoutine(attacksAmount, breakBetweenAttacks));
        }

        private IEnumerator AttackRoutine(int attacksAmount, float breakBetweenAttacks)
        {
            int attackCounter = 0;
            WaitForSeconds breakWaitDuration = new WaitForSeconds(breakBetweenAttacks);

            while (attackCounter < attacksAmount)
            {
                Shoot();
                attackCounter++;
                yield return breakWaitDuration;
            }

            OnAttackFinished?.Invoke();
        }

        public void StartAttackRoutine(float duration)
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }

            _attackCoroutine = StartCoroutine(AttackRoutine(duration));
        }

        private IEnumerator AttackRoutine(float duration)
        {
            Shoot();
            yield return new WaitForSeconds(duration);
            OnAttackFinished?.Invoke();
        }

        public void SetTelegraphSpeed(float speed)
        {
            _telegraphDuration = speed;
        }

        #region IController
        public override void Restart()
        {
            base.Restart();
            if (_telegraphCoroutine != null)
            {
                StopCoroutine(_telegraphCoroutine);
            }

            if (_activePoolables.Count > 0)
            {
                _activePoolables.ForEach(e => e.TurnOff());
            }

            _activePoolables.Clear();

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
        }
        #endregion
    }
}
