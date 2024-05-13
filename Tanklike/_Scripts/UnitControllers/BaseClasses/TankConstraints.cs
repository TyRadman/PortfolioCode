using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankConstraints : MonoBehaviour, IController
    {
        [SerializeField] private PlayerComponents _components;
        private List<AbilityConstraint> _currentConstraints = new List<AbilityConstraint>();
        private AbilityConstraint _currentConstraint = AbilityConstraint.None;

        public bool IsActive { get; private set; }

        public void ApplyConstraints(bool enable, AbilityConstraint constraints)
        {
            if (enable)
            {
                _currentConstraints.Add(constraints);
            }
            else
            {
                _currentConstraints.Remove(constraints);
            }

            //Debug.Log("Constraints count -> " + _currentConstraints.Count);

            _currentConstraint = GetCurrentConstraint();

            _components.Movement.EnableMovement((_currentConstraint & AbilityConstraint.Movement) == 0);
            _components.Crosshair.Enable((_currentConstraint & AbilityConstraint.Rotation) == 0);
            _components.Shooter.EnableShooting((_currentConstraint & AbilityConstraint.Shooting) == 0);
            _components.Health.EnableTakingDamage((_currentConstraint & AbilityConstraint.TakingDamage) == 0);
            _components.PlayerBoost.EnableBoost((_currentConstraint & AbilityConstraint.Boost) == 0);
            _components.OnHold.Enable((_currentConstraint & AbilityConstraint.HoldDownAction) == 0);
            _components.Crosshair.EnableAimAssist((_currentConstraint & AbilityConstraint.AimAssist) == 0);
            _components.SuperAbilities.EnableAbility((_currentConstraint & AbilityConstraint.SuperAbility) == 0);
            _components.Jump.EnableJump((_currentConstraint & AbilityConstraint.Jump) == 0);
            _components.Tools.EnableToolsUsage((_currentConstraint & AbilityConstraint.Tools) == 0);
        }

        private AbilityConstraint GetCurrentConstraint()
        {
            AbilityConstraint con = AbilityConstraint.None;

            for (int i = 0; i < _currentConstraints.Count; i++)
            {
                con |= _currentConstraints[i];
            }

            return con;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
        }

        public void Dispose()
        {
        }
        #endregion
    }

    [System.Flags]
    public enum AbilityConstraint
    {
        None = 0,
        Movement = 1,
        Rotation = 2,
        Shooting = 4,
        TakingDamage = 8,
        Boost = 16,
        HoldDownAction = 32,
        AimAssist = 64,
        SuperAbility = 128,
        Jump = 256,
        Tools = 512
    }
}
