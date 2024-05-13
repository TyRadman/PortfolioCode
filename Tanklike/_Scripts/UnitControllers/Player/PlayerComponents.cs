using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.ItemsSystem;
using TankLike.Combat;
using TankLike.Sound;

namespace TankLike.UnitControllers
{
    public class PlayerComponents : TankComponents
    {
        public GameplaySettings PlayerSettings;
        [field: SerializeField] public PlayerUpgrades Upgrades { get; private set; }
        [field: SerializeField] public PlayerExperience Experience { get; private set; }
        [field: SerializeField] public PlayerTools Tools { get; private set; }
        [field: SerializeField] public PlayerOverHeat Overheat { get; private set; }
        [field: SerializeField] public PlayerSuperAbilities SuperAbilities { get; private set; }
        [field: SerializeField] public PlayerSuperAbilityRecharger SuperRecharger { get; private set; }
        [field: SerializeField] public PlayerInteractions PlayerInteractions { get; private set; }
        [field: SerializeField] public PlayerBoost PlayerBoost { get; private set; }
        [field: SerializeField] public PlayerHoldAction OnHold { get; private set; }
        [field: SerializeField] public PlayerJump Jump { get; private set; }
        [field: SerializeField] public PlayerCrosshairController Crosshair { get; private set; }
        [field: SerializeField] public PlayerUIController UIController { get; private set; }
        [field: SerializeField] public PlayerPredictedPosition PredictedPosition { set; get; }
        [field: SerializeField] public PlayerFuel Fuel { set; get; }
        [field: SerializeField] public bool IsAlive { set; get; }
        
       
        [Header("Audio")]
        [SerializeField] private Audio _onCollectedAudio;

        public int PlayerIndex = 0;

        public override void GetComponents()
        {
            base.GetComponents();
            Tools = GetComponent<PlayerTools>();
            Experience = GetComponent<PlayerExperience>();
            Upgrades = GetComponent<PlayerUpgrades>();
            Overheat = GetComponent<PlayerOverHeat>();
            SuperAbilities = GetComponent<PlayerSuperAbilities>();
            PlayerInteractions = GetComponent<PlayerInteractions>();
            PlayerBoost = GetComponent<PlayerBoost>();
            OnHold = GetComponent<PlayerHoldAction>();
            Jump = GetComponent<PlayerJump>();
            Crosshair = GetComponent<PlayerCrosshairController>();
            UIController = GetComponent<PlayerUIController>();
            PredictedPosition = GetComponent<PlayerPredictedPosition>();
            Fuel = GetComponent<PlayerFuel>();
        }

        public override void SetUp()
        {
            base.SetUp();
            SuperAbilities.SetUp(this);
            Upgrades.SetUp(Experience);
            PlayerInteractions.SetUp();
            Overheat.SetUp(this);
            Experience.SetUp();
            PlayerBoost.Setup(this);
            Jump.SetUp(this);
            Crosshair.SetUp();
            OnHold.SetUp();
            UIController.SetUp(this);
            PredictedPosition.SetUp(this);
            Tools.SetUp(this);
            Fuel.Setup(this);

            Activate(); //for now
            Health.OnDeath += OnDeathHandler;
        }

        public void SetUpSettings()
        {
            ApplySettings(PlayerSettings);
        }

        public void OnRevived()
        {
            IsAlive = true;
            Activate();
        }

        private void OnDeathHandler()
        {

            Restart();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Collectable collectable))
            {
                //print($"{other.name} has collectable script at {Time.time}");

                if (collectable.CanBeCollected)
                {
                    //print($"Collected {other.name} at {Time.time}");
                    collectable.OnCollected(this);
                    GameManager.Instance.AudioManager.Play(_onCollectedAudio);
                    GameManager.Instance.ReportManager.ReportCollection(collectable, PlayerIndex);
                }
            }
        }

        public void ApplySettings(GameplaySettings settings)
        {
            // apply settings here
            Crosshair.SetAimSensitivity(settings.AimSensitivity);
        }

        #region IController
        public override void Activate()
        {
            base.Activate();
            Upgrades.Activate();
            Experience.Activate();
            Tools.Activate();
            Overheat.Activate();
            SuperAbilities.Activate();
            SuperRecharger.Activate();
            PlayerInteractions.Activate();
            PlayerBoost.Activate();
            OnHold.Activate();
            Jump.Activate();
            Crosshair.Activate();
            UIController.Activate();
            PredictedPosition.Activate();
            ((PlayerMovement)Movement).Activate();
            Tools.Activate();
            Fuel.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Upgrades.Deactivate();
            Experience.Deactivate();
            Tools.Deactivate();
            Overheat.Deactivate();
            SuperAbilities.Deactivate();
            SuperRecharger.Deactivate();
            PlayerInteractions.Deactivate();
            PlayerBoost.Deactivate();
            OnHold.Deactivate();
            Jump.Deactivate();
            Crosshair.Deactivate();
            UIController.Deactivate();
            PredictedPosition.Deactivate();
            ((PlayerMovement)Movement).Deactivate();
            Tools.Deactivate();
            Fuel.Deactivate();
        }

        public override void Dispose()
        {
            base.Dispose();
            Upgrades.Dispose();
            Experience.Dispose();
            Tools.Dispose();
            Overheat.Dispose();
            SuperAbilities.Dispose();
            SuperRecharger.Dispose();
            PlayerInteractions.Dispose();
            PlayerBoost.Dispose();
            OnHold.Dispose();
            Jump.Dispose();
            Crosshair.Dispose();
            UIController.Dispose();
            PredictedPosition.Dispose();
            ((PlayerMovement)Movement).Dispose();
            Tools.Dispose();
            Fuel.Dispose();
        }

        public override void Restart()
        {
            base.Restart();
            Upgrades.Restart();
            Experience.Restart();
            Tools.Restart();
            Overheat.Restart();
            SuperAbilities.Restart();
            SuperRecharger.Restart();
            PlayerInteractions.Restart();
            PlayerBoost.Restart();
            OnHold.Restart();
            Jump.Restart();
            Crosshair.Restart();
            UIController.Restart();
            PredictedPosition.Restart();
            ((PlayerMovement)Movement).Restart();
            Tools.Restart();
            Fuel.Restart();
        }
        #endregion
    }
}