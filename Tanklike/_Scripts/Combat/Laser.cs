using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using TankLike.Sound;
    using UnitControllers;
    using Utils;

    public class Laser : Ammunition
    {
        [Header("Settings")]
        [SerializeField] private Transform _muzzlePoint;

        [Header("Visuals")]
        [SerializeField] private LineRenderer _beam;
        [SerializeField] protected ParticleSystem _hitParticles;
        [SerializeField] protected ParticleSystem _muzzleFlashParticles;
        [SerializeField] private float _textureTilingSize = 5;

        [Header("Audio")]
        [SerializeField] private Audio _onShootingAudio;

        private AudioSource _audioSource;

        private Material _material;
        private Action<IPoolable> OnRemoveFromActivePoolables;
        private Coroutine _laserCoroutine;

        private float _duration;
        private float _maxLength;
        private int _damageOverTime;
        private float _damageInterval = 0.1f;
        private float _thickness = 0.3f;
        private float _damageTimer;
        private bool _isDealingDamage = false;


        /// <summary>
        /// The texture's tiling property on the x axis. Determines how stretched the texture will be across the laser
        /// </summary>
        private const float LASER_FADE_OUT_TIME = 1f;
        private const string LENGTH_ID = "_TilingLength";
        private const string ALPHA_CLIP_ID = "_CutoffHeight";
        private const string NOISE_CLIP_ID = "_NoiseMaxIn";

        private void Awake()
        {
            _material = _beam.material;
            _hitParticles.Stop();
            _muzzleFlashParticles.Stop();

            _damageTimer = _damageInterval;
            _isActive = false;
            _beam.enabled = false;
            _beam.SetPosition(0, _muzzlePoint.position);
            _beam.SetPosition(1, _muzzlePoint.position);
        }

        public void SetUp(TankComponents shooter = null, Action<IPoolable> RemoveFromActivePoolables = null)
        {
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

        public void Activate()
        {
            _audioSource = GameManager.Instance.AudioManager.Play(_onShootingAudio);

            _beam.enabled = true;
            _isActive = true;
            _isDealingDamage = true;
            _hitParticles.Play();
            _muzzleFlashParticles.Play();
            SetMaterialDisolveProperties(0);

            if (_laserCoroutine != null)
            {
                StopCoroutine(_laserCoroutine);
            }

            _laserCoroutine = StartCoroutine(LaserCountdownRoutine());

            _damageTimer = _damageInterval;

        }

        private void SetMaterialDisolveProperties(float value)
        {
            _material.SetFloat(ALPHA_CLIP_ID, 1 - value);
            _material.SetFloat(NOISE_CLIP_ID, 1 + value);
        }

        private IEnumerator LaserCountdownRoutine()
        {
            yield return new WaitForSeconds(_duration);
            Deactivate();
        }

        private void FixedUpdate()
        {
            if (!_isDealingDamage || !_beam.enabled)
            {
                return;
            }

            UpdateLaser();
        }

        private void UpdateLaser()
        {
            Vector3 forwardDirection = _muzzlePoint.forward;
            forwardDirection.y = 0f;
            Ray ray = new Ray(_muzzlePoint.position, forwardDirection);
            bool cast = Physics.SphereCast(ray, _thickness, out RaycastHit hit, _maxLength, _targetLayerMask);
            Vector3 hitPoint = _muzzlePoint.position + forwardDirection * _maxLength;
            _damageTimer += Time.fixedDeltaTime;

            if (cast && (1 << hit.transform.gameObject.layer & _targetLayerMask) == 1 << hit.transform.gameObject.layer)
            {
                float distanceToTarget = Vector3.Distance(_muzzlePoint.position, hit.point);
                hitPoint = _muzzlePoint.position + distanceToTarget * forwardDirection;

                // resize the texture on the laser
                Vector2 textureSeizRange = new Vector2(1f, _maxLength / _textureTilingSize);
                float tilingSize = textureSeizRange.Lerp(Mathf.InverseLerp(0f, _maxLength, distanceToTarget));
                _material.SetFloat(LENGTH_ID, tilingSize);

                if (_damageTimer >= _damageInterval)
                {
                    // checks for damagables
                    if (cast && hit.transform.TryGetComponent(out IDamageable damagable))
                    {
                        if (damagable.IsInvincible)
                        {
                            return;
                        }

                        damagable.TakeDamage(_damageOverTime, Vector3.zero, null, hitPoint);
                    }

                    _damageTimer = 0f;
                }
            }

            _beam.SetPosition(0, _muzzlePoint.position);
            _beam.SetPosition(1, hitPoint);

            _hitParticles.transform.position = hitPoint;
        }

        public void Deactivate()
        {
            if(!_isActive)
            {
                return;
            }

            StartCoroutine(DeactivationProcess());
        }

        private IEnumerator DeactivationProcess()
        {
            StopEffects();
            _isDealingDamage = false;
            _damageTimer = _damageInterval;

            float timeElapsed = 0f;
            StopAudio();

            while (timeElapsed < LASER_FADE_OUT_TIME)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / LASER_FADE_OUT_TIME;
                SetMaterialDisolveProperties(t);

                yield return null;
            }

            ResetBeam();
            _isActive = false;

            OnReleaseToPool(this);
            OnRemoveFromActivePoolables(this);
            gameObject.SetActive(false);
        }

        private void StopEffects()
        {
            _hitParticles.Stop();
            _muzzleFlashParticles.Stop();
        }

        private void ResetBeam()
        {
            _beam.enabled = false;
            _beam.SetPosition(0, _muzzlePoint.position);
            _beam.SetPosition(1, _muzzlePoint.position);
        }

        private void StopAudio()
        {
            if (_audioSource == null)
            {
                Debug.LogError($"No audio source attached to {gameObject.name}");
                return;
            }

            _audioSource.loop = false;
            _audioSource.Stop();
            _audioSource.clip = null;
        }

        public void SetTargetLayerMask(string tag)
        {
            _targetLayerMask = 0;
            _targetLayerMask |= Constants.MutualHittableLayer;
            _targetLayerMask |= Constants.WallLayer;

            if (tag == TanksTag.Enemy.ToString())
            {
                _targetLayerMask |= 1 << Constants.EnemyDamagableLayer;
            }
            else
            {
                _targetLayerMask |= 1 << Constants.PlayerDamagableLayer;
            }
        }

        private void OnDisable()
        {
            if (_isActive)
            {
                TurnOff();
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

            if(_audioSource != null)
            {
                _audioSource.Stop();
            }

            OnReleaseToPool(this);
        }

        public override void OnRequest()
        {
            base.OnRequest();
        }

        public override void OnRelease()
        {
            Deactivate();
            GameManager.Instance.SetParentToSpawnables(gameObject);

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
