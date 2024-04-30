using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyShieldAbilityParent : EnemyAbilityStyle
    {
        public Vector2 ShieldDurationRange;
        protected float ShieldDuration;
        public ShieldType Type;

        #region Defaults
        public virtual void Perform(Shield_EnemyAbility _ability)
        {
            // turn off the colliders of the ship
            _ability.ShipEffects.EnableColliders(false);
        }

        public virtual void ForceStop(Shield_EnemyAbility _ability)
        {

        }
        #endregion

        public virtual void SetShieldValue(float _difficulty, Shield_EnemyAbility _ability)
        {
            ShieldDuration = ShieldDurationRange.Lerp(_difficulty);
            _ability.m_ShieldAbility.SetShieldDuration(ShieldDuration);
            // disables the damage detector if the shield doesn't allow it
        }

        public enum ShieldType
        {
            Normal = 0, Health = 1
        }
    }
}