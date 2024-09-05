using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "BA_NAME", menuName = Directories.ABILITIES_HOLDER + "Boost Ability Holder")]
    public class BoostAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { get; set; }
        [Tooltip("How far should the tank travel before the ability is activated again (only if it's on update.)")]
        [field: SerializeField] public float DistancePerActivation { get; private set; } = 1f;
        [Tooltip("How much more fuel will be consumed on start when boosting with this ability equipped.")]
        [field: SerializeField, Header("Modifiers")] public float FuelStartConsumptionRateMultiplier { get; private set; } = 1.2f;
        [field: SerializeField] public float FuelConsumptionRateMultiplier { get; private set; } = 1.2f;
        [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.5f;
    }
}
