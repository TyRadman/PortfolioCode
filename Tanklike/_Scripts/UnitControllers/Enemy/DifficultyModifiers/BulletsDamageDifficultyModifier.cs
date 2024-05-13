using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_BulletDamage", menuName = MENU_NAME + "Bullet Damage")]
    public class BulletsDamageDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _damageRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            enemy.Shooter.SetWeaponDamage((int)_damageRange.Lerp(difficulty));
        }
    }
}
