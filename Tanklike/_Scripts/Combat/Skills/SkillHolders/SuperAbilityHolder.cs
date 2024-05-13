using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "SA_NAME", menuName = Directories.ABILITIES + "Super Ability Holder")]
    public class SuperAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { set; get; }
        [field: SerializeField] public RechargingValues RechargeInfo { get; private set; }
        
        public override Sprite GetIcon()
        {
            return Ability.GetIcon();
        }
    }

    [System.Serializable]
    public class RechargingValues
    {
        public AbilityRechargingMethod RechargingMethod;
        public float Time;
        public int NumberOfEnemyHits;
        public int NumberOfHits;
    }
}
