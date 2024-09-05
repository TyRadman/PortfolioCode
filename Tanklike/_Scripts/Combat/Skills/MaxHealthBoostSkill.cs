using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "MaxHealthBoost", menuName = MENU_ROOT + "Max Health Boost")]
    public class MaxHealthBoostSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private int _healthPoints = 100;

        // what the skill does when it is added to the tank 
        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            components.Health.AddToMaxHealth(_healthPoints);
        }

    }
}
