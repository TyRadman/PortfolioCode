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
        [field: SerializeField] public UnitData Stats { private set; get; }
        [field: SerializeField] public TankAnimation Animation { private set; get; }
        [field: SerializeField] public TankConstraints Constraints { private set; get; }
        [field: SerializeField] public TankAdditionalInfo AdditionalInfo { private set; get; }
        [field: SerializeField] public TankWiggler TankWiggler { private set; get; }
        [field: SerializeField] public TankBodyParts TankBodyParts { private set; get; }
        [field: SerializeField] public UnitVisualEffects VisualEffects { private set; get; }

        [SerializeField] protected UnitMinimapIcon _minimapIcon;
        
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
            VisualEffects = GetComponent<UnitVisualEffects>();
        }

        public virtual void SetUp()
        {
            // Setups for all components
            TankBodyParts.SetUp(Stats);
            Movement.SetUp(this);
            Shooter.SetUp(this);
            Health.SetUp(this);
            Animation.SetUp(this);
            Visuals.Setup(this);
            
            if(ElementsEffects != null)
            {
                ElementsEffects.Setup(this);
            }

            if(TankWiggler != null)
            {
                TankWiggler.SetUp(this);
            }

            if (TurretController != null)
            {
                TurretController.SetUp(this);
            }           
            
            if (VisualEffects != null)
            {
                VisualEffects.SetUp(this);
            }
            

            Health.OnDeath += OnDeathHandler;
        }

        /// <summary>
        /// Plays effects, handles minimap icon, and freezes the screen
        /// </summary>
        public virtual void OnDeathHandler(TankComponents tank)
        {
            // TODO: check this when working on elements
            //ElementsEffects.SetCanGetEffects(false);

            if (_minimapIcon != null)
            {
                _minimapIcon.KillIcon();
            }
            else
            {
                Debug.LogError($"No minimap on {gameObject.name}");
            }
        }

        #region IController
        public virtual void Activate()
        {
            // TODO: check this when working on elements
            //ElementsEffects.Activate();
            Shooter.Activate();
            Animation.Activate();
            VisualEffects.Activate();
        }

        public virtual void Deactivate()
        {
            Movement.Deactivate();
            Shooter.Deactivate();
            VisualEffects.Deactivate();
        }

        public virtual void Restart()
        {
            Movement.Restart();
            // TODO: check this when working on elements
            //ElementsEffects.Restart();
            Visuals.Restart();
            Health.Restart();
            Animation.Restart();
            Shooter.Restart();
            VisualEffects.Restart();
        }

        public virtual void Dispose()
        {
            // TODO: check this when working on elements
            //ElementsEffects.Dispose();
            Movement.Dispose();
            VisualEffects.Dispose();
        }
        #endregion
    }
}
