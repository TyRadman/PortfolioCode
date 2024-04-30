using SpaceWar.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PlayerComponents : ShipComponenets, IController
    {
        public PlayerInteraction Interactions;
        public PlayerInputManager InputManager;
        public PlayerStats Stats;
        public PlayerShipInput Input;
        public PlayerMovement Movement;
        public PlayerAbilities Abilities;
        public PlayerShooting ThePlayerShooting;

        [SerializeField] private ParticleSystem[] m_LevelUpParticles;
        public PlayerCoins Coins;
        public new PlayerHealth Health;
        public PlayerShipInfo ShipInfo;
        [field: SerializeField] public ShipConstraints Constraints { get; private set; }
        public int PlayerIndex = -1;
        public bool IsAlive = false;

        public override void GetComponents()
        {
            base.GetComponents();
            Interactions = GetComponent<PlayerInteraction>();
            InputManager = GetComponent<PlayerInputManager>();
            Input = GetComponent<PlayerShipInput>();
            Stats = GetComponent<PlayerStats>();
            Movement = GetComponent<PlayerMovement>();
            Abilities = GetComponent<PlayerAbilities>();
            ThePlayerShooting = GetComponent<PlayerShooting>();
            Coins = GetComponent<PlayerCoins>();
            Health = GetComponent<PlayerHealth>();
            Constraints = GetComponent<ShipConstraints>();
        }

        #region IController
        public void SetUp()
        {
            CreateLevelUpParticles();

            InputManager.SetUp(this);
            Interactions.SetUp(this);
            Movement.SetUp(this);
            Abilities.SetUp(this);
            ThePlayerShooting.SetUp(this);
            Health.SetUp(this);
            Constraints.SetUp(this);
        }

        public void Activate()
        {
            InputManager.Activate();
            Interactions.Activate();
            Movement.Activate();
            Abilities.Activate();
            ThePlayerShooting.Activate();
            Health.Activate();
            Constraints.Activate();
        }

        public void Deactivate()
        {
            InputManager.Deactivate();
            Interactions.Deactivate();
            Movement.Deactivate();
            Abilities.Deactivate();
            ThePlayerShooting.Deactivate();
            Health.Deactivate();
            Constraints.Deactivate();
        }

        public void Dispose()
        {
            InputManager.Dispose();
            Interactions.Dispose();
            Movement.Dispose();
            Abilities.Dispose();
            ThePlayerShooting.Dispose();
            Health.Dispose();
            Constraints.Dispose();
        }
        #endregion

        // if we ever need to implement multiple particles at once then this feature must be added to the pooling system, otherwise, it's an unnecessary pain in the ass
        #region Level Up Particles
        private void CreateLevelUpParticles()
        {
            var particles = GameManager.i.GeneralValues.LevelUpParticles;
            m_LevelUpParticles = new ParticleSystem[particles.Length];

            for (int i = 0; i < particles.Length; i++)
            {
                m_LevelUpParticles[i] = Instantiate(particles[i], transform).GetComponent<ParticleSystem>();
            }
        }

        public void PlayLevelUpParticles()
        {
            for (int i = 0; i < m_LevelUpParticles.Length; i++)
            {
                m_LevelUpParticles[i].Play();
            }
        }
        #endregion
    }
}