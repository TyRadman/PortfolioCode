using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    public interface IDamageable
    {
        bool IsInvincible { get; }
        bool IsDead { get; }
        Transform Transform { get; }
        void TakeDamage(int damage, Vector3 direction, UnitComponents shooter, Vector3 bulletPosition, Ammunition damageDealer = null);
        void Die();
        DamagePopUpAnchor PopUpAnchor { get; }
    }
}
