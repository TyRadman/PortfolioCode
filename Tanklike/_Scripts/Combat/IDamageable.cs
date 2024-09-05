using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    public interface IDamageable
    {
        bool IsInvincible { get; }
        Transform Transform { get; }
        void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition);
        void Die();
        DamagePopUpAnchor PopUpAnchor { get; }
    }
}
