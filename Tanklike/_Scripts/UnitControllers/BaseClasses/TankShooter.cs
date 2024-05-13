using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;
using TankLike.Elements;
using TankLike.Utils;
using TankLike.Misc;
using TankLike.Sound;
using TankLike.Cam;

namespace TankLike.UnitControllers
{
    public abstract class TankShooter : MonoBehaviour, IController
    {
        public System.Action OnTargetHit;
        public System.Action OnShoot;
        public System.Action OnShootFinished;
        protected float _coolDownTime = 3f;
        [SerializeField] protected float _shootAnimationDelay = 0f;
        [SerializeField] protected bool _canShoot = true;
        [SerializeField] protected TankComponents _components;
        [SerializeField] protected Audio _onShootAudio;
        [SerializeField] protected LayerMask _targetLayers;

        protected List<Transform> _shootingPoints;
        protected Animator _turretAnimator;

        public List<Transform> ShootingPoints => _shootingPoints;

        public bool IsActive { get; protected set; }

        // custom shots
        [SerializeField] private Weapon _customShot;
        // either normal or custom
        private System.Action<Transform, float> ShootingMethod;
        [SerializeField] protected Weapon _startingWeapon;
        protected Weapon _currentWeapon;
        private List<Weapon> _normalShots = new List<Weapon>();

        protected List<IPoolable> _activePoolables = new List<IPoolable>();

        private WaitForSeconds _shootWaitForSeconds;

        protected virtual void Awake()
        {
            ShootingMethod += DefaultShot;
            _shootWaitForSeconds = new WaitForSeconds(_shootAnimationDelay);
        }

        #region Shooting Method Overload
        public virtual void Shoot(bool hasCoolDown = true, Transform shootingPoint = null, float angle = 0f)
        {
            if (!_canShoot && hasCoolDown) return;

            if (shootingPoint == null) shootingPoint = _shootingPoints[0];

            OnShoot?.Invoke();
            _components.Animation.PlayShootAnimation();
            //PlayShootAudio();

            StartCoroutine(ShootingRoutine(shootingPoint, angle));

            if (hasCoolDown)
            {
                _canShoot = false;
                EnableShooting();
            }
        }

        private IEnumerator ShootingRoutine(Transform shootingPoint = null, float angle = 0f)
        {
            yield return _shootWaitForSeconds;
            ShootingMethod(shootingPoint, angle);
        }

        /// <summary>
        /// Shoots using the shots of a custom weapon
        /// </summary>
        /// <param name="weapon">The custom weapon</param>
        /// <param name="angle">The angle of the shot in degrees</param>
        /// <param name="hasCoolDown">True if you want the shot to trigger the down</param>
        public virtual void Shoot(Weapon weapon, float angle, bool hasCoolDown = true)
        {
            if (!_canShoot && hasCoolDown) return;

            weapon.OnShot(_components, null, angle);
            _components.Animation.PlayShootAnimation(2f); //dirty, get the speed from the weapon

            if (hasCoolDown)
            {
                _canShoot = false;
                EnableShooting();
            }
        }
        #endregion

        #region Effects
        public void ShowShootingEffects(ParticleSystemHandler muzzleEffect, Transform shootingPoint = null)
        {
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            

            if(shootingPoint != null)
            {
                position = shootingPoint.position;
                rotation = shootingPoint.rotation;
            }
            else
            {
                if (_shootingPoints.Count > 0)
                {
                    rotation = _shootingPoints[0].rotation;
                    position = _shootingPoints[0].position;
                }
            }

            muzzleEffect.transform.SetPositionAndRotation(position, rotation);
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();
        }
        #endregion

        #region Enable Shooting
        // this method is virtual so that the player can override it and enable shooting through a coroutine which will allow them to update the BarImage of the weapon icon
        protected virtual void EnableShooting()
        {
            Invoke(nameof(EnableShootingInvoke), _coolDownTime);
        }

        private void EnableShootingInvoke()
        {
            _canShoot = true;
        }
        #endregion

        public virtual void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            var turret = (TankTurret)parts.GetBodyPartOfType(BodyPartType.Turret);
            _shootingPoints = new List<Transform>();
            SetupShootingPoints(turret.ShootingPoints);
            _turretAnimator = turret.Animator;

            if (_startingWeapon == null)
            {
                Debug.LogWarning("Starting weapon is null for " + gameObject.name);
                return;
            }

            SetWeapon(_startingWeapon);
        }

        private void SetupShootingPoints(Transform[] shootingPoints)
        {
            _shootingPoints.Clear();
            _shootingPoints.AddRange(shootingPoints);
        }

        public void EnableShooting(bool canShoot)
        {
            _canShoot = canShoot;
        }

        #region Setting Bullet Overloads
        public void SetElement(ElementEffect element)
        {
            //_element = element;
        }

        public void SetDeflection(Deflection deflection)
        {
            //_deflection = deflection;
        }
        #endregion

        public virtual void DefaultShot(Transform shootingPoint = null, float angle = 0)
        {
            if (_currentWeapon == null)
            {
                return;
            }

            _currentWeapon.OnShot(_components, shootingPoint, angle);
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
        }

        #region Custom Shooting
        private void CustomShot(Transform shootingPoint = null, float angle = 0)
        {
            if (_customShot == null) return;

            OnShoot?.Invoke();
            _customShot.OnShot(_components, shootingPoint, angle);
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);

            // return the shooting method to default
            ShootingMethod -= CustomShot;
            ShootingMethod += DefaultShot;
            _customShot = null;
        }

        public void SetCustomShot(Weapon shot)
        {
            _customShot = shot;
            ShootingMethod -= DefaultShot;
            ShootingMethod += CustomShot;
        }
        #endregion

        public void SpawnBullet(Bullet bullet, Transform shootingPoint = null, float angle = 0)
        {
            // create the bullet
            bullet.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (shootingPoint != null)
            {
                Vector3 eulerRotation = shootingPoint.eulerAngles;
                eulerRotation.x = 0;
                rotation = Quaternion.Euler(eulerRotation);
                position = shootingPoint.position;
            }
            else
            {
                if (_shootingPoints.Count > 0)
                {
                    Vector3 eulerRotation = _shootingPoints[0].eulerAngles;
                    eulerRotation.x = 0;
                    rotation = Quaternion.Euler(eulerRotation);
                    position = _shootingPoints[0].position;
                }
            }
            
            if(angle != 0) rotation *= Quaternion.Euler(0f, angle, 0f);

            bullet.transform.SetPositionAndRotation(position, rotation);
            bullet.StartBullet(_components);
            bullet.SetTargetLayerMask(Helper.GetOpposingTag(gameObject.tag));
        }

        public void ShootLaser(Laser laser, Transform shootingPoint = null, float angle = 0)
        {
            // create the bullet
            laser.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (_shootingPoints != null)
            {
                rotation = _shootingPoints[0].rotation;
                position = _shootingPoints[0].position;
            }

            if (shootingPoint != null)
            {
                rotation = shootingPoint.rotation;
                position = shootingPoint.position;           
            }

            if (angle != 0) rotation *= Quaternion.Euler(0f, angle, 0f);

            laser.transform.SetPositionAndRotation(position, rotation);
            laser.transform.parent = shootingPoint == null ?  _shootingPoints[0] : shootingPoint;
            laser.SetUp(_components, RemoveFromActivePoolables);
            laser.SetTargetLayerMask(Helper.GetOpposingTag(gameObject.tag));
            // activate laser
            laser.Activate(true);
            _activePoolables.Add(laser);
        }

        #region Extra
        private void RemoveFromActivePoolables(IPoolable poolable)
        {
            if (_activePoolables.Contains(poolable))
                _activePoolables.Remove(poolable);
        }

        protected virtual void OnShootExitHandler()
        {
            OnShootFinished?.Invoke();
        }

        public bool IsCanShoot()
        {
            return _canShoot;
        }     

        public Weapon GetWeapon()
        {
            return _currentWeapon;
        }

        public List<Weapon> GetWeapons()
        {
            return _normalShots;
        }

        public void SetWeaponDamage(int damage)
        {
            if(_startingWeapon != null)
                _startingWeapon.SetDamage(damage);
        }

        public virtual void SetWeapon(Weapon weapon)
        {
            if (weapon == null)
            {
                weapon = _currentWeapon;
            }

            var wep = Instantiate(weapon);
            _currentWeapon = wep;
            _coolDownTime = _currentWeapon.CoolDownTime;
            _currentWeapon.SetTargetLayer(_targetLayers);

            if (!_normalShots.Exists(w => w == weapon))
            {
                _normalShots.Add(weapon);
            }

            // set the audio if the bullet has special audios
            if (_currentWeapon.ShotAudio != null) _onShootAudio = _currentWeapon.ShotAudio;
        }

        public virtual void AddWeapon(Weapon weapon)
        {
            _normalShots.Add(weapon);
        }

        public void SetWeaponSpeed(float bulletSpeed)
        {
            _startingWeapon.SetSpeed(bulletSpeed);
        }

        public void SetLaserDuration(float duration)
        {
            if(_currentWeapon is not LaserWeapon)
            {
                Debug.LogError($"{gameObject.name} doesn't have a laser weapon");
            }

            ((LaserWeapon)_currentWeapon).SetDuration(duration);
        }
        #endregion

        public void PlayShootAudio()
        {
            GameManager.Instance.AudioManager.Play(_onShootAudio);
        }

        #region IController
        public virtual void Activate()
        {
            //var pubs = _turretAnimator.GetBehaviours<AnimatorEventPublisher>();

            //foreach (AnimatorEventPublisher publisher in pubs)
            //{
            //    if (publisher.StateName == "Shoot")
            //    {
            //        publisher.OnExit += OnShootExitHandler;
            //    }
            //}
        }

        public virtual void Deactivate()
        {
            //var pubs = _turretAnimator.GetBehaviours<AnimatorEventPublisher>();

            //foreach (AnimatorEventPublisher publisher in pubs)
            //{
            //    if (publisher.StateName == "Shoot")
            //    {
            //        publisher.OnExit -= OnShootExitHandler;
            //    }
            //}
        }

        public virtual void Restart()
        {
            _canShoot = true;

            //var pubs = _turretAnimator.GetBehaviours<AnimatorEventPublisher>();

            //foreach (AnimatorEventPublisher publisher in pubs)
            //{
            //    if (publisher.StateName == "Shoot")
            //    {
            //        publisher.OnExit -= OnShootExitHandler;
            //    }
            //}
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}

public enum TanksTag
{
    Player, Enemy
}
