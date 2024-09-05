using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "WH_NAME", menuName = Directories.ABILITIES_HOLDER + "Weapon Holder")]
    public class WeaponHolder : SkillHolder
    {
        [field: SerializeField] public Weapon Weapon { get; private set; }
    }
}
