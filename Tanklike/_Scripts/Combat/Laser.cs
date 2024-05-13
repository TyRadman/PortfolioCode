using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public class Laser : Ammunition
    {
        [Header("Settings")]
        [SerializeField] private Transform _muzzlePoint;

        [Header("Visuals")]
        [SerializeField] private LineRenderer _beam;
        [SerializeField] protected ParticleSystem _hitParticles;
        [SerializeField] protected ParticleSystem _muzzleFlashParticles;
        private float _duration;
        private float _maxLength;
        private int _damageOverTime;
        private float _damageInterval = 0.1f;
        private float _thickness = 0.3f;

        private float _damageTimer;
        private Coroutine _laserCoroutine;
        private Action<IPoolable> OnRemoveFromActivePoolables;

        private void Awake()
        {
            Activate(false);
        }

        public void SetUp(TankComponents shooter = null, Action<IPoolable> RemoveFromActivePoolables = null)
        {
            //AddTargetTag(MUTUAL_HITTABLE_TAG);
            //AddTargetTag(WALL_TAG);

            OnRemoveFromActivePoolables = RemoveFromActivePoolables;
        }

        public void SetValues(float maxLength, float thickness, int damageOverTime, float damageInterval, float duration)
        {
            _maxLength = maxLength;
            _thickness = thickness;
            _damageOverTime = damageOverTime;
            _duration = duration;
            _damageInterval = damageInterval;
        }

        public void Activate(bool value)
        {
            _beam.enabled = value;
            _isActive = value;

            if (value)
            {
                _hitParticles.Play();
                _muzzleFlashParticles.Play();
                
                if (_laserCoroutine != null)
                {
                    StopCoroutine(_laserCoroutine);
                }

                _laserCoroutine = StartCoroutine(LaserRoutine());
            }
            else
            {
                _beam.SetPosition(0, _muzzlePoint.position);
                _beam.SetPosition(1, _muzzlePoint.position);
                _hitParticles.Stop();
                _muzzleFlashParticles.Stop();
            }

            _damageTimer = _damageInterval;
        }

        private IEnumerator LaserRoutine()
        {
            yield return new WaitForSeconds(_duration);
            OnReleaseToPool(this);
            OnRemoveFromActivePoolables(this);
        }

        private void FixedUpdate()
        {
            if (!_isActive)
            {
                return;
            }

            if (!_beam.enabled)
            {
                return;
            }

            Ray ray = new Ray(_muzzlePoint.position, _muzzlePoint.forward);
            bool cast = Physics.SphereCast(ray, _thickness, out RaycastHit hit, _maxLength, _targetLayerMask);
            Vector3 hitPoint = _muzzlePoint.position + _muzzlePoint.forward * _maxLength;

            // Visualize the sphere's radius.
            Debug.DrawLine(_muzzlePoint.position, _muzzlePoint.position + Vector3.up * _thickness, Color.blue); // Draw the top of the sphere.
            Debug.DrawLine(_muzzlePoint.position, _muzzlePoint.position + Vector3.down * _thickness, Color.blue); // Draw the bottom of the sphere.
            Debug.DrawLine(_muzzlePoint.position, _muzzlePoint.position + Vector3.left * _thickness, Color.blue); // Draw the left side of the sphere.
            Debug.DrawLine(_muzzlePoint.position, _muzzlePoint.position + Vector3.right * _thickness, Color.blue); // Draw the right side of the sphere.
            Debug.DrawLine(_muzzlePoint.position, _muzzlePoint.position + Vector3.forward * _thickness, Color.blue); // Draw the front side of the sphere.
            Debug.DrawLine(_muzzlePoint.position, _muzzlePoint.position + Vector3.back * _thickness, Color.blue); // Draw the back side of the sphere.

            _damageTimer += Time.fixedDeltaTime;


            if (cast && (1 << hit.transform.gameObject.layer & _targetLayerMask) == 1 << hit.transform.gameObject.layer)
            {
                hitPoint = hit.point;

                if (_damageTimer >= _damageInterval)
                {
                    // checks for damagables
                    if (cast && hit.transform.TryGetComponent<IDamageable>(out IDamageable damagable))
                    {
                        if (damagable.Invincible) return;
                        damagable.TakeDamage(_damageOverTime, Vector3.zero, null, hitPoint);
                    }
                    _damageTimer = 0f;
                }
            }

            _beam.SetPosition(0, _muzzlePoint.position);
            _beam.SetPosition(1, hitPoint);

            _hitParticles.transform.position = hitPoint;
        }

        public void SetTargetLayerMask(string tag)
        {
            _targetLayerMask = 0;
            _targetLayerMask |= Constants.MutualHittableLayer;
            _targetLayerMask |= Constants.WallLayer;
            if (tag == TanksTag.Enemy.ToString())
            {
                _targetLayerMask |= 1 << Constants.EnemyDamagableLayer;//LayerMask.NameToLayer("Enemy");
            }
            else
            {
                _targetLayerMask |= 1 << Constants.PlayerDamagableLayer;//LayerMask.NameToLayer("Player");
            }
        }

        #region Pool
        public override void Init(Action<IPoolable> OnRelease)
        {
            base.Init(OnRelease);
        }

        public override void TurnOff()
        {
            base.TurnOff();
            OnReleaseToPool(this);
        }

        public override void OnRequest()
        {
            base.OnRequest();
        }

        public override void OnRelease()
        {
            Activate(false);
            base.OnRelease();

            if (_laserCoroutine != null)
            {
                StopCoroutine(_laserCoroutine);
            }
        }

        public override void Clear()
        {
            base.Clear();
        }
        #endregion
    }
}
