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
        public System.Action OnShootStarted;
        public System.Action OnShootFinished;
        protected float _coolDownTime = 3f;

        [Header("Settings")]
        [SerializeField] protected bool _canShoot;
        [SerializeField] protected float _shootAnimationDelay = 0f;
        [SerializeField] protected LayerMask _targetLayers;

        [Header("References")]
        [SerializeField] protected WeaponHolder _startWeaponHolder;
        [SerializeField] private Weapon _customShot;
        [SerializeField] protected Audio _onShootAudio;

        protected List<Transform> _shootingPoints;
        protected Animator _turretAnimator;
        protected Transform _turret;
        public List<Transform> ShootingPoints => _shootingPoints;

        public bool IsActive { get; protected set; }

        // custom shots
        // either normal or custom
        private System.Action<Transform, float> ShootingMethod;

        protected TankComponents _components;
        protected Weapon _currentWeapon;
        protected WeaponHolder _currentWeaponHolder;
        private List<Weapon> _normalShots = new List<Weapon>();

        protected List<IPoolable> _activePoolables = new List<IPoolable>();

        private WaitForSeconds _shootWaitForSeconds;

        protected virtual void Awake()
        {
            _shootWaitForSeconds = new WaitForSeconds(_shootAnimationDelay);
        }

        #region Shooting Method Overload
        public virtual void Shoot(bool hasCoolDown = true, Transform shootingPoint = null, float angle = 0f)
        {
            if (!_canShoot && hasCoolDown)
            {
                return;
            }

            if (shootingPoint == null)
            {
                shootingPoint = _shootingPoints[0];
            }

            // TODO: remove it after fixing the air drone shooter
            OnShootStarted?.Invoke();
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

            weapon.OnShot(null, angle);
            _components.Animation.PlayShootAnimation(2f); //dirty, get the speed from the weapon

            if (hasCoolDown)
            {
                _canShoot = false;
                EnableShooting();
            }
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
            _turret = turret.transform;

            _components = components;
            _shootingPoints = new List<Transform>();
            SetupShootingPoints(turret.ShootingPoints);
            _turretAnimator = turret.Animator;

            ShootingMethod = DefaultShot;

            if (_startWeaponHolder == null)
            {
                Debug.LogWarning("Starting weapon is null for " + gameObject.name);
                return;
            }

            _canShoot = false;

            SetWeapon(_startWeaponHolder);
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

            _currentWeapon.OnShot(shootingPoint, angle);
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
        }

        #region Custom Shooting
        private void CustomShot(Transform shootingPoint = null, float angle = 0)
        {
            if (_customShot == null) return;

            OnShootStarted?.Invoke();
            _customShot.OnShot(shootingPoint, angle);
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


        #region Extra
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
            if (_startWeaponHolder != null)
            {
                _startWeaponHolder.Weapon.SetDamage(damage);
            }
        }

        public virtual void SetWeapon(WeaponHolder weaponHolder)
        {
            if (weaponHolder == null)
            {
                weaponHolder = _currentWeaponHolder;
            }

            Weapon weapon = Instantiate(weaponHolder.Weapon);
            _currentWeapon = weapon;
            _coolDownTime = _currentWeapon.CoolDownTime;
            _currentWeapon.SetTargetLayer(_targetLayers);
            _currentWeapon.SetUp(_components);

            if (!_normalShots.Exists(w => w == weapon))
            {
                _normalShots.Add(weapon);
            }

            // set the audio if the bullet has special audios
            if (_currentWeapon.ShotAudio != null)
            {
                _onShootAudio = _currentWeapon.ShotAudio;
            }
        }

        public virtual void AddWeapon(Weapon weapon)
        {
            _normalShots.Add(weapon);
        }

        public void SetWeaponSpeed(float bulletSpeed)
        {
            _startWeaponHolder.Weapon.SetSpeed(bulletSpeed);
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
            _canShoot = true;
        }

        public virtual void Deactivate()
        {
            _canShoot = false;
        }

        public virtual void Restart()
        {
            _canShoot = true;
            _currentWeapon.DisposeWeapon();
        }

        public virtual void Dispose()
        {
            ShootingMethod -= DefaultShot;
            _currentWeapon.DisposeWeapon();
        }
        #endregion
    }
}

public enum TanksTag
{
    Player, Enemy
}
