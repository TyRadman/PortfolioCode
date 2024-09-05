using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat.Abilities
{
    [CreateAssetMenu(fileName = "SA_NAME", menuName = Directories.ABILITIES_HOLDER + "Super Ability Holder")]
    public class SuperAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { set; get; }
        [field: SerializeField] public RechargingValues RechargeInfo { get; private set; }
        [field: SerializeField] public BaseIndicator IndicatorPrefab { get; private set; }
        [field: SerializeField] public AbilityConstraint OnAimConstraints { get; private set; }
        [field: SerializeField] public AbilityConstraint OnPerformConstraints { get; private set; }

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
