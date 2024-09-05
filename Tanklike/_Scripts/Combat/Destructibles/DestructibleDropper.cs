using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.LevelGeneration;
using TankLike.Sound;
using TankLike.UI.DamagePopUp;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Combat.Destructible
{
    [RequireComponent(typeof(DamagePopUpAnchor))]
    public class DestructibleDropper : MonoBehaviour, IDamageable, IDropper
    {
        [Header("Stats")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;
        [field: SerializeField] public DestructableTag Tag { get; private set; }
        [field: SerializeField] public List<DropChance> CollecatblesToSpawn { set; get; } = new List<DropChance>();
        [Header("Audio")]
        [SerializeField] protected Audio _destructionAudio;
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { get; private set; }

        public Transform Transform => transform;
        public bool IsInvincible { get; set; }

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        #region IDamagable
        public void Die()
        {

        }

        public void TakeDamage(int damage, Vector3 direction, TankComponents tank, Vector3 bulletPosition)
        {
            _currentHealth -= damage;
            GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                DamagePopUpType.Damage, damage, PopUpAnchor.Anchor);

            if (_currentHealth <= 0)
            {
                OnDeath(tank);
            }
        }

        protected virtual void OnDeath(TankComponents tank)
        {
            GameManager.Instance.AudioManager.Play(_destructionAudio);
            GameManager.Instance.RoomsManager.CurrentRoom.RemoveDropper(transform);
        }
        #endregion

        public void SetCollectablesToSpawn(DestructibleDrop drops)
        {
            int dropsCount = drops.DropsCountRange.RandomValue();

            for (int i = 0; i < dropsCount; i++)
            {
                float chance = Random.value;
                List<DropChance> tags = new List<DropChance>();
                drops.Drops.FindAll(c => c.OriginalChance > chance).ForEach(d => tags.Add(d));

                if (tags.Count == 0)
                {
                    tags.Add(drops.Drops.FindAll(d => d.OriginalChance > 0).RandomItem());
                }

                CollecatblesToSpawn.Add(tags.RandomItem());
            }
        }
    }
}
