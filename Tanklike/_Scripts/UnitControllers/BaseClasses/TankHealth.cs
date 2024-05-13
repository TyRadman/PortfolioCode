using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Minimap;
using TankLike.Combat;
using TankLike.UI.DamagePopUp;
using TankLike.Cam;

namespace TankLike.UnitControllers 
{
    [RequireComponent(typeof(DamagePopUpAnchor))]
    public abstract class TankHealth : MonoBehaviour, IController, IDamageable
    {
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _currentHealth;
        [SerializeField] protected TankComponents _components;
        [SerializeField] protected bool _canTakeDamage = true;
        [SerializeField] protected TankHealthEffect _effects;
        [SerializeField] protected Vector3 _explosionDecalSize = Vector3.one;
        [SerializeField] protected float _explosionDecalheight = 0.1f;
        public const float DAMAGE_POP_UP_OFFSET = 2f;
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { set; get; }

        protected Transform _body;
        protected Transform _turret;
        protected TankData _stats;
        protected List<DamageDetector> _damageDetectors;

        protected float _damageMultiplier = 0f;

        public System.Action OnHit;
        public System.Action OnDeath { get; set; }

        public Transform Transform => transform;
        public bool Invincible { get; set; }
        public bool IsActive { get; protected set; }

        public virtual void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;
            TankData stats = components.Stats;

            _stats = stats;
            _body = parts.GetBodyPartOfType(BodyPartType.Body).transform;
            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
            _damageDetectors = ((TankBody)parts.GetBodyPartOfType(BodyPartType.Body)).DamageDetectors;
            _components = components;

            _currentHealth = _maxHealth;
        }

        public virtual void Die()
        {
            _components.OnDeath();
            OnDeath?.Invoke(); // only enemies use it so far

            // explosion and camera effects (MUTUAL)
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.DeathExplosion;
            explosion.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            explosion.gameObject.SetActive(true);
            explosion.Play();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);
            var decal = GameManager.Instance.VisualEffectsManager.Explosions.ExplosionDecal;
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            decal.transform.SetPositionAndRotation(transform.position + Vector3.up * _explosionDecalheight, randomRotation);
            decal.transform.localScale = _explosionDecalSize;
            decal.gameObject.SetActive(true);
            decal.Play();
        }

        public virtual void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            if (!_canTakeDamage)
            {
                return;
            }

            OnHit?.Invoke();

            // if it's damage that we're dealing with, not health increment, then we apply the multiplier to it
            if(damage > 0)
            {
                damage += Mathf.RoundToInt(damage * _damageMultiplier);
            }

            _currentHealth -= damage;
            _effects.SetIntensity((float)_currentHealth / (float)_maxHealth);

            ///// display pop up /////
            // if the damage is positive, then it's a heal
            if (PopUpAnchor != null)
            {
                if (damage > 0)
                {
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                        DamagePopUpType.Damage, damage, PopUpAnchor.Anchor);
                }
                else
                {
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                        DamagePopUpType.Heal, damage, PopUpAnchor.Anchor);
                }
            }
            
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }

            if(_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }
        
        public virtual void Heal(int amount)
        {
            _currentHealth += amount;

            _effects.SetIntensity((float)_currentHealth / (float)_maxHealth);

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        public void SetDamageIntakeMultiplier(float value)
        {
            _damageMultiplier = value;
        }

        public void AddToMaxHealth(int amount)
        {
            float t = Mathf.InverseLerp(0, _maxHealth, _currentHealth);
            _maxHealth += amount;
            // adjust current health in accordance
            _currentHealth = (int)Mathf.Lerp(0, _maxHealth, t);
        }

        public void EnableTakingDamage(bool enable)
        {
            _canTakeDamage = enable;
        }

        public void SetMaxHealth(int health, bool refillHealth = true)
        {
            _maxHealth = health;
            
            if(refillHealth) _currentHealth = health;
        }

        public void SetInvincible(bool value)
        {
            Invincible = value;
        }

        public void SwitchDamageDetectorsLayer(int layer)
        {
            _damageDetectors.ForEach(d => d.gameObject.layer = layer);
            //gameObject.layer = layer;
        }

        public float GetHealth()
        {
            return _currentHealth / _maxHealth;
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            _currentHealth = _maxHealth;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
