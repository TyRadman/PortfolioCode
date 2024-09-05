using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Minimap;
using TankLike.Combat;
using TankLike.UI.DamagePopUp;
using TankLike.Cam;
using TankLike.ScreenFreeze;

namespace TankLike.UnitControllers 
{
    [RequireComponent(typeof(DamagePopUpAnchor))]
    public abstract class TankHealth : MonoBehaviour, IController, IDamageable
    {
        [Header("Settings")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected bool _canTakeDamage = true;
        [field: SerializeField] public bool IsInvincible { get; set; } = false;
        public bool IsDead { get; private set; } = true;

        [Header("References")]
        [SerializeField] protected TankHealthEffect _damageEffects;
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { get; protected set;  }
        [field: SerializeField] public ScreenFreezeData DeathScreenFreeze { get; protected set; }

        public const float DAMAGE_POP_UP_OFFSET = 2f;

        protected TankComponents _components;
        protected Transform _body;
        protected Transform _turret;
        protected UnitData _stats;
        protected List<DamageDetector> _damageDetectors;
        protected int _currentHealth;

        protected float _damageMultiplier = 0f;

        public System.Action OnHit { get; set; }
        public System.Action<TankComponents> OnDeath { get; set; }

        [HideInInspector] public Transform Transform => transform;
        public bool IsActive { get; protected set; }

        public virtual void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;
            UnitData stats = components.Stats;

            IsDead = false;
            _stats = stats;
            _body = parts.GetBodyPartOfType(BodyPartType.Body).transform;
            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
            _damageDetectors = ((TankBody)parts.GetBodyPartOfType(BodyPartType.Body)).DamageDetectors;
            _components = components;
            IsInvincible = false;
            _currentHealth = _maxHealth;
        }

        public virtual void Die()
        {
            if(IsDead)
            {
                return;
            }

            IsDead = true;
            GameManager.Instance.ScreenFreezer.FreezeScreen(DeathScreenFreeze);
            _components.VisualEffects.PlayDeathEffects();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);
            OnDeath?.Invoke(_components);
        }

        public virtual void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            if (IsDead)
            {
                return;
            }

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
            _damageEffects.SetIntensity((float)_currentHealth / (float)_maxHealth);

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

            _damageEffects.SetIntensity((float)_currentHealth / (float)_maxHealth);

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

            if (refillHealth)
            {
                _currentHealth = health;
            }
        }

        public void SetInvincible(bool value)
        {
            IsInvincible = value;
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

        public void ReplenishFullHealth()
        {
            Heal(_maxHealth - _currentHealth);
        }

        #region IController
        public virtual void Activate()
        {
            IsDead = false;
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            IsDead = false;
            _currentHealth = _maxHealth;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
