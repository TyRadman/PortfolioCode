using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TankLike.Combat.Destructible
{
    using Environment.LevelGeneration;
    using Sound;
    using UI.DamagePopUp;
    using UnitControllers;
    using ItemsSystem;
    using Environment;

    [RequireComponent(typeof(DamagePopUpAnchor))]
    public class DestructibleDropper : MonoBehaviour, IDamageable, IDropper, IAimAssistTarget
    {
        [field: SerializeField] public DropperTag DropperTag { get; private set; }
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { get; private set; }
        public Transform Transform => transform;
        public bool IsInvincible { get; set; }
        public bool IsDead { get; private set; } = false;
        public Action<Transform> OnTargetDestroyed { get; set; }

        [Header("Stats")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;
        [SerializeField] protected CollectablesDropSettings _dropSettings;
        
        [Header("Audio")]
        [SerializeField] protected Audio _destructionAudio;
        
        protected DestructibleDrop _drops;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void AssignAsTarget(Room room)
        {
            if (room == null)
            {
                Debug.LogError("Room is null");
                return;
            }

            room.Spawnables.AddDropper(transform);
            OnTargetDestroyed += room.Spawnables.RemoveDropper;
        }

        #region IDamagable

        public void TakeDamage(int damage, Vector3 direction, UnitComponents tank, Vector3 bulletPosition, Ammunition damageDealer = null)
        {
            if (IsDead)
            {
                return;
            }

            _currentHealth -= damage;

            GameManager.Instance.DamagePopUpManager.DisplayPopUp(DamagePopUpType.Damage, damage, PopUpAnchor.Anchor);

            if (_currentHealth <= 0)
            {
                Die();
                OnDestructibleDeath(tank);
            }
        }

        public void Die()
        {
            IsDead = true;

            OnTargetDestroyed?.Invoke(transform);
            OnTargetDestroyed = null;
        }

        protected virtual void OnDestructibleDeath(UnitComponents tank)
        {
            GameManager.Instance.AudioManager.Play(_destructionAudio);
        }
        #endregion

        public void SetCollectablesToSpawn(DestructibleDrop drops)
        {
            _drops = drops;
        }

        public void SetDropSettings(CollectablesDropSettings settings)
        {
            _dropSettings = settings;
        }
    }
}
