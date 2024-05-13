using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Minimap;

namespace TankLike.UnitControllers
{
    public abstract class TankComponents : MonoBehaviour, IController
    {
        [field: SerializeField] public CharacterController CharacterController { private set; get; }
        [field: SerializeField] public TanksTag Tag { private set; get; }
        [field: SerializeField] public TankTurretController TurretController { private set; get; }
        [field: SerializeField] public TankShooter Shooter { private set; get; }
        [field: SerializeField] public TankHealth Health { private set; get; }
        [field: SerializeField] public TankMovement Movement { private set; get; }
        [field: SerializeField] public TankSuperAbilities SuperAbility { private set; get; }
        [field: SerializeField] public TankElementsEffects ElementsEffects { private set; get; }
        [field: SerializeField] public TankVisuals Visuals { private set; get; }
        [field: SerializeField] public TankData Stats { private set; get; }
        [field: SerializeField] public TankAnimation Animation { private set; get; }
        [field: SerializeField] public TankConstraints Constraints { private set; get; }
        [field: SerializeField] public TankAdditionalInfo AdditionalInfo { private set; get; }
        [field: SerializeField] public TankWiggler TankWiggler { private set; get; }
        [field: SerializeField] public TankBodyParts TankBodyParts { private set; get; }

        [SerializeField] protected UnitMinimapIcon _minimapIcon;
        public Action OnTankDeath;

        public bool IsActive { get; set; }

        public virtual void GetComponents()
        {
            CharacterController = GetComponent<CharacterController>();
            Movement = GetComponent<TankMovement>();
            TurretController = GetComponent<TankTurretController>();
            Shooter = GetComponent<TankShooter>();
            Health = GetComponent<TankHealth>();
            SuperAbility = GetComponent<TankSuperAbilities>();
            Animation = GetComponent<TankAnimation>();
            ElementsEffects = GetComponent<TankElementsEffects>();
            Visuals = GetComponent<TankVisuals>();
            Constraints = GetComponent<TankConstraints>();
            AdditionalInfo = GetComponent<TankAdditionalInfo>();
            TankWiggler = GetComponent<TankWiggler>();
            TankBodyParts = GetComponent<TankBodyParts>();
        }

        public virtual void SetUp()
        {
            // Setups for all components
            TankBodyParts.SetUp();
            Movement.SetUp(this);
            Shooter.SetUp(this);
            Health.SetUp(this);
            Animation.SetUp(this);
            Visuals.Setup(this);

            if(TankWiggler != null)
            {
                TankWiggler.SetUp(this);
            }

            if (TurretController != null)
            {
                TurretController.SetUp(this);
            }
        }

        public virtual void OnDeath()
        {
            GameManager.Instance.ScreenFreezer.FreezeScreen(GameManager.Instance.Constants.EnemyDeathFreezeData);
            OnTankDeath?.Invoke();
            ElementsEffects.SetCanGetEffects(false);
            _minimapIcon.KillIcon();
        }

        #region IController
        public virtual void Activate()
        {
            // Activate all components
            ElementsEffects.Activate();
            Shooter.Activate();
            Animation.Activate();
        }

        public virtual void Deactivate()
        {
            Movement.Deactivate();
            Shooter.Deactivate();
        }

        public virtual void Restart()
        {
            // Reset all components
            Movement.Restart();
            ElementsEffects.Restart();
            Visuals.Restart();
            Health.Restart();
            Animation.Restart();
            Shooter.Restart();
        }

        public virtual void Dispose()
        {
            // Dispose all components
            ElementsEffects.Dispose();
            Movement.Dispose();
        }
        #endregion
    }
}
