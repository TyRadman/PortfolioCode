using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    [RequireComponent(typeof(ShipEffects))]
    public class Stats : MonoBehaviour
    {
        //public float MaxHealth;
        [SerializeField] protected float CurrentHealth;
        public bool IsInWave = false;
        [HideInInspector] public float DamagePerShot = 0f;
        public BulletUser UserTag;


        protected virtual void Awake()
        {

        }

        public virtual void OnShipDestroyed(int _playerIndex = -1)
        {
            AudioManager.i.PlayAudio(GameManager.i.GeneralValues.DeathAudio);
        }

        public virtual void SetDamage(float _damage)
        {
            DamagePerShot = _damage;
        }

        public float GetHealth()
        {
            return CurrentHealth;
        }

        public BulletUser GetUserTag()
        {
            return UserTag;
        }
    }
}