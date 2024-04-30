using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipComponenets : MonoBehaviour
    {
        [field: SerializeField] public virtual ShipHealth Health { set; get; }
        [HideInInspector] public ShipEffects Effects;
        private List<DamageDetector> m_DamageDetectors = new List<DamageDetector>();
        [HideInInspector] public Stats ShipStats;
        public Shooter ShipShooter;

        protected virtual void Awake()
        {
            Effects = GetComponent<ShipEffects>();
            ShipStats = GetComponent<Stats>();
            ShipShooter = GetComponent<Shooter>();
            //Health = GetComponent<PlayerHealth>();
        }

        private void Start()
        {
            // in case it didn't have damage detectors already cached
            if (m_DamageDetectors.Count == 0)
            {
                m_DamageDetectors.Add(GetComponent<DamageDetector>());
            }
        }

        public virtual void GetComponents()
        {

        }

        public void TurnOnDamageDetectors(bool _on)
        {
            m_DamageDetectors.ForEach(d => d.enabled = _on);
        }

        public void EnableColliders(bool _on)
        {
            m_DamageDetectors.ForEach(d => d.DetectorCollider.enabled = _on);
        }

        // when we want to spawn the ship again for whatever reason
        public virtual void ResetValues()
        {
            Health.SetCurrentHealthToMaxHealth();
            ShipShooter.EnableShooting();
        }
    }
}