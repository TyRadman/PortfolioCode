using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "HA_NAME", menuName = Directories.ABILITIES + "Hold Ability Holder")]
    public class HoldAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { get; private set; }
        [field: SerializeField] public float HoldDownDuration { set; get; } = 1f;
        public override Sprite GetIcon()
        {
            return Ability.GetIcon();
        }
    }
}
