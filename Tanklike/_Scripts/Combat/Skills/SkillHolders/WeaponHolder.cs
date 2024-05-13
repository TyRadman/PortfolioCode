using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike
{
    public class WeaponHolder : SkillHolder
    {
        [field: SerializeField] public Weapon Weapon { get; private set; }
        public override Sprite GetIcon()
        {
            return Weapon.GetIcon();
        }
    }
}
