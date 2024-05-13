using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Combat
{
    public class Rocket : Ammunition
    {
        [Header("Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _maxHeightReach;
        [SerializeField] private float _impactRadius;

        [Header("Visuals")]
        [SerializeField] private MeshRenderer _rocketMesh;
        [SerializeField] private ParticleSystem _explosionParticles;
        [SerializeField] private ParticleSystem _trailParticles;

        private bool _hitTarget;

        public void SetUp(float duration, TankComponents shooter = null, System.Action<IPoolable> RemoveFromActivePoolables = null)
        {
            if (shooter != null)
            {
                _shooter = shooter;
            }

            //AddTargetTag(MUTUAL_HITTABLE_TAG);
            //AddTargetTag(WALL_TAG);
        }
   
        public void Fire(Vector3 startPoint, Vector3 endPoint, float range, System.Action<IDamageable, Vector3?, bool> OnAttackHit, Indicator targetIndicator)
        {
            StartCoroutine(Fly(endPoint, startPoint));
        }

        private IEnumerator Fly(Vector3 endPoint, Vector3 startPoint)
        {
            _trailParticles.Stop(true);
            _trailParticles.Play(true);

            var middlePoint = new Vector3((endPoint.x + startPoint.x) / 2f, _maxHeightReach + startPoint.y, (endPoint.z + startPoint.z) / 2f);

            var spline = new Helper.SimpleSpline(startPoint, middlePoint, endPoint);

            var distanceMaxSqr = new Vector3(startPoint.x - endPoint.x, 0f, startPoint.z - endPoint.z).sqrMagnitude;
            var distanceMax = Mathf.Sqrt(distanceMaxSqr);

            var t = 0f;

            while (t <= 1f)
            {
                var newPos = spline.Evaluate(t);
                var newTang = spline.EvaluateTangent(t);
                transform.position = newPos;
                transform.LookAt(newPos + newTang.normalized);

                t += (_speed * Time.deltaTime) / distanceMax;

                // If hit a target while flying
                if (_hitTarget)
                {
                    break;
                }

                yield return null;
            }

            if (_hitTarget)
            {
                yield break;
            }

            OnCollision();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive)
            {
                return;
            }

            if (_targetTags.Exists(t => other.CompareTag(t)))
            {
                IDamageable damagable = null;
                IElementTarget elementTarget = null;

                // checks for damagables
                if (other.GetComponent<IDamageable>() != null)
                {
                    damagable = other.GetComponent<IDamageable>();
                    if (!damagable.Invincible)
                    {
                        // we need to get the contact point in the future for more accuracy, but this will do for now REWORK_CHECK
                        //_impact.Impact(transform.position, damagable, _damage, _targetLayerMask, this);
                        GameManager.Instance.AudioManager.Play(_impactAudio);        
                    }
                }
                else
                {
                    // we need to get the contact point in the future for more accuracy, but this will do for now REWORK_CHECK
                    //Impact.Impact(transform.position, damagable, _damage, _targetLayerMask, this);
                    GameManager.Instance.AudioManager.Play(_impactAudio);
                }
            }

            Debug.Log("BULLET HIT TARGET!");
            Debug.Log("ON TRIGGER ENTER ROCKET = " + other.gameObject.name);

            StopAllCoroutines();
            OnCollision();
        }

        private void OnCollision()
        {
            _trailParticles.Stop(true);
            _rocketMesh.enabled = false;
            _hitTarget = true;
            _isActive = false;

            Collider[] targets = Physics.OverlapSphere(transform.position, _impactRadius, _targetLayerMask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        if (damagable.Invincible) continue;

                        damagable.TakeDamage(_damage, Vector3.zero, _shooter, transform.position);
                    }
                }
            }

            //if (targetIndicator != null)
            //    targetIndicator.SetActive(false);

            //StartCoroutine(CollisionEffectsRoutine());
        }

        //private IEnumerator CollisionEffectsRoutine()
        //{
        //    //audioSource.Stop();
        //    //audioSource.Play();
        //    //explosionParticles.Stop(true);
        //    //explosionParticles.Play(true);

        //    //yield return new WaitForSeconds(explosionDuration);

        //    //if (ReleaseToPool != null)
        //    //    ReleaseToPool(this);
        //    //else
        //    //    Destroy(gameObject);


        //}

        //public void PlayImpactEffects()
        //{
        //    var impact = GameManager.Instance.VisualEffectsManager.Bullets.GetImpact(_bulletData.GUID);
        //    impact.transform.position = transform.position;
        //    var rotation = Quaternion.LookRotation(transform.forward);
        //    impact.transform.rotation = rotation;
        //    impact.gameObject.SetActive(true);
        //    impact.Play();
        //}

        //private void PlayEffects()
        //{
        //    GameManager.Instance.BulletsManager.RemoveBullet(this);
        //    OnReleaseToPool(this);
        //}

        #region Pool
        public void OnRelease()
        {
            gameObject.SetActive(false);
            GameManager.Instance.SetParentToSpawnables(gameObject);

            //if (_laserCoroutine != null)
            //    StopCoroutine(_laserCoroutine);
        }

        #endregion
    }
}
