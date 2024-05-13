using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Elements;
using TankLike.Utils;
using System;

namespace TankLike.Combat
{
    public class Bullet : Ammunition
    {
        [Header("Settings")]
        [SerializeField] protected LayerMask _bumberLayers;
        [SerializeField] protected float _bumberDetectionDistance;
        [SerializeField] protected bool _useGravity;
        [SerializeField] protected float _gravityMultiplier;
        protected bool _limitedDistance;
        protected float _maxDistance = 3f;

        [Header("Visuals")]
        [SerializeField] protected bool _removeTrailOnHit = false;
        [SerializeField] protected GameObject _bullet;
        [SerializeField] protected GameObject _trail;

        protected Rigidbody _rb;
        protected SphereCollider _collider;
        protected Vector3 _movementDir;
        protected float _currentSpeed;
        protected float _deltaTime;
        [SerializeField] protected List<string> _deflectionTags = new List<string>();
        private AmmunationData _bulletData;
        private Indicator _targetIndicator;
        private Coroutine _movementRoutine;

        // deflection 
        [field: SerializeField] public Deflection Deflection { set; get; }
        private DeflectionData _deflectionData;
        [field: SerializeField] public SpeedOverLife SpeedOverLife { set; get; }
        [field: SerializeField] public OnImpact Impact { set; get; }
        [field: SerializeField] public ElementEffect Element { set; get; }

        public const float ROTATION_SPEED = 5f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();

            // if there is no deflection behaviour, then set the deflection to "no deflection"
            if (Deflection == null) Deflection = ScriptableObject.CreateInstance<NoneDeflections>();
        }

        public void SetUp(TankComponents shooter = null)
        {
            if (shooter != null)
            {
                _shooter = shooter;
            }

            SetUpConfigurations();
        }

        public void EnableBullet(bool enable)
        {
            _trail.SetActive(enable);
            gameObject.SetActive(enable);
            _isActive = enable;
            _collider.enabled = enable;
            _bullet.SetActive(enable);
        }

        public void StartBullet(TankComponents shooter, Vector3? dir = null)
        {
            EnableBullet(true);
            SetUp(shooter);

            // movement part (TBR)
            if (dir != null)
            {
                _movementDir = (Vector3)dir;
            }
            else
            {
                _movementDir = transform.forward;
            }

            _movementDir.Normalize();
            StartCoroutine(MovementRoutine());

            // get the defleection data
            if(_deflectionData == null && Deflection != null)
            {
                _deflectionData = Deflection.GetData();
            }
        }

        public void EnableBullet()
        {
            SetUpConfigurations();
            _collider.enabled = true;
            _bullet.SetActive(true);
            _trail.SetActive(true);
        }

        public void SetActive(bool active)
        {
            _isActive = active;
        }

        public void MoveToPointAlongSpline(Vector3 point, float maxHeightReach, float duration)
        {
            if (_movementRoutine != null)
                StopCoroutine(_movementRoutine);
            _movementRoutine = StartCoroutine(MoveToPointAlongSplineRoutine(point, maxHeightReach, duration));
        }

        private IEnumerator MoveToPointAlongSplineRoutine(Vector3 point, float maxHeightReach, float duration)
        {
            Vector3 startPos = transform.position;
            var middlePoint = new Vector3((point.x + startPos.x) / 2f, maxHeightReach + startPos.y, (point.z + startPos.z) / 2f);
            var spline = new Helper.SimpleSpline(startPos, middlePoint, point);

            var elapsedTime = 0f;
            var t = 0f;

            while (t <= 1f)
            {
                var newPos = spline.Evaluate(t);
                var newTang = spline.EvaluateTangent(t);
                transform.position = newPos;
                transform.LookAt(newPos + newTang.normalized);

                elapsedTime += Time.deltaTime;
                t = Mathf.Clamp01(elapsedTime / duration);

                yield return null;
            }
        }

        private IEnumerator MovementRoutine()
        {
            float distanceTraveled = 0f;
            Vector3 gravityVector = Vector3.zero;

            if (_useGravity)
            {
                gravityVector = new Vector3(0f, -_gravityMultiplier, 0f);
            }

            while (_isActive)
            {
                // handle speed
                MoveBulletForFrame(gravityVector, SpeedOverLife.GetSpeed(_currentSpeed, Time.deltaTime));

                if (_limitedDistance)
                {
                    distanceTraveled += Time.deltaTime * SpeedOverLife.GetSpeed(_currentSpeed, Time.deltaTime);
                 
                    if (distanceTraveled >= _maxDistance)
                    {
                        Explode();
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Moves the bullet with given parameters for a frame
        /// </summary>
        public void MoveBulletForFrame(Vector3 gravity, float speed)
        {
            float deltaTime = Time.deltaTime;
            _movementDir = transform.forward;
            _movementDir.Normalize();

            if (_useGravity) _movementDir += gravity * deltaTime;

            transform.position += deltaTime * speed * _movementDir;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive)
            {
                return;
            }

            OnBulletHitObject(other);
            CheckForDeflection(other);
        }

        private void OnBulletHitObject(Collider other)
        {
            if ((1 << other.gameObject.layer & _targetLayerMask) == 1 << other.gameObject.layer)
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
                        Impact.Impact(transform.position, damagable, _damage, _targetLayerMask, this);
                        GameManager.Instance.AudioManager.Play(_impactAudio);

                        // checks for affectables??
                        if (other.GetComponent<IElementTarget>() != null && Element != null)
                        {
                            elementTarget = other.GetComponent<IElementTarget>();
                            // apply effect
                            elementTarget?.TakeElementEffect(Element);
                        }
                    }
                }
                else
                {
                    // we need to get the contact point in the future for more accuracy, but this will do for now REWORK_CHECK
                    Impact.Impact(transform.position, damagable, _damage, _targetLayerMask, this);
                    GameManager.Instance.AudioManager.Play(_impactAudio);

                    // checks for affectables??
                    if (other.GetComponent<IElementTarget>() != null && Element != null)
                    {
                        elementTarget = other.GetComponent<IElementTarget>();
                        // apply effect
                        elementTarget?.TakeElementEffect(Element);
                    }
                }
            }
        }

        private void CheckForDeflection(Collider other)
        {
            if (_deflectionTags.Exists(t => other.CompareTag(t)) && Deflection != null)
            {
                // handle deflection
                Deflection.Deflect(_collider, ref _movementDir, _bumberDetectionDistance, _bumberLayers, other, _deflectionData, this);
            }
        }

        public void Explode()
        {
            IDamageable damagable = null;
            Impact.Impact(transform.position, damagable, _damage, _targetLayerMask, this);
        }

        public void DisableBullet()
        {
            _collider.enabled = false;
            //apply impact effect here. What about the explosion effect?
            _isActive = false;
            PlayImpactEffects();
            ResetConfigurations();
            _bullet.SetActive(false);

            if (_removeTrailOnHit)
            {
                _trail.SetActive(false);
            }

            if (_movementRoutine != null)
            {
                StopCoroutine(_movementRoutine);
            }

            if (_targetIndicator != null)
            {
                _targetIndicator.TurnOff();
            }

            if (OnReleaseToPool != null)
            {
                Invoke(nameof(PlayEffects), 2f);
            }
            else
            {
                Invoke(nameof(DeactivateBullet), 2f);
            }
        }

        public void PlayImpactEffects()
        {
            var impact = GameManager.Instance.VisualEffectsManager.Bullets.GetImpact(_bulletData.GUID);
            impact.transform.position = transform.position;
            var rotation = Quaternion.LookRotation(transform.forward);
            impact.transform.rotation = rotation;
            impact.gameObject.SetActive(true);
            impact.Play();
        }

        private void DeactivateBullet()
        {
            gameObject.SetActive(false);
        }

        private void PlayEffects()
        {
            GameManager.Instance.BulletsManager.RemoveBullet(this);
            OnReleaseToPool(this);
        }

        public void SetTargetIndicator(Indicator indicator)
        {
            _targetIndicator = indicator;
        }

        public TankComponents GetShooter()
        {
            return _shooter;
        }

        public void SetShooter(TankComponents shooter)
        {
            _shooter = shooter;
        }

        #region Set Up Methods
        // for the configurator (not a part of the game
        public void SetUpDeflection(Deflection deflection)
        {
            Deflection = deflection;
        }

        public void SetUpConfigurations()
        {
            if (Deflection != null)
            {
                Deflection.SetUp(this);
            }
        }

        public void SetUpBulletdata(AmmunationData bulletData)
        {
            _bulletData = bulletData;
        }

        public void SetTargetLayerMask(string tag)
        {
            _targetLayerMask = 0;
            _targetLayerMask |= Constants.MutualHittableLayer;
            _targetLayerMask |= Constants.GroundLayer;
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

        public void SetTargetLayerMask(LayerMask mask)
        {
            _targetLayerMask = mask;
        }

        private void ResetConfigurations()
        {
            SpeedOverLife.Reset();

            if(Deflection != null)
            {
                Deflection.Reset(_deflectionData, this);
                Deflection = null;
            }
        }
        
        /// <summary>
        /// Sets up the values for the bullet
        /// </summary>
        /// <param name="speed">The speed at which the bullet travels</param>
        /// <param name="damage">The damage the bullet inflicts to its target</param>
        /// <param name="maxDistance">The max distance the bullet travels. If it's set to 0, then it's assumed to be infinite</param>
        public void SetValues(float speed, int damage, float maxDistance)
        {
            _limitedDistance = maxDistance > 0;
            _maxDistance = maxDistance;
            _currentSpeed = speed;
            _damage = damage;
        }
        #endregion

        #region Set Configuration methods
        public void SetElement(ElementEffect element)
        {
            Element = element;
        }
        #endregion

        #region Pool
        public override void Init(Action<IPoolable> onRelease)
        {
            base.Init(onRelease);
        }

        public override void TurnOff()
        {
            base.TurnOff();
            OnReleaseToPool(this);
        }

        public override void OnRequest()
        {
            base.OnRequest();
            GameManager.Instance.SetParentToRoomSpawnables(gameObject);
            GameManager.Instance.BulletsManager.AddBullet(this);
        }

        public override void OnRelease()
        {
            EnableBullet(false);
        }

        public override void Clear()
        {
            base.Clear();
        }
        #endregion
    }
}
