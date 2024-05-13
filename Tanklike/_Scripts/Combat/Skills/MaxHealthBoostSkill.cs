using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "Max Health Boost Skill", menuName = "Skills/Stats/Max Health Boost")]
    public class MaxHealthBoostSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private int _healthPoints = 100;

        // what the skill does when it is added to the tank 
        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);
            tankTransform.GetComponent<PlayerHealth>().AddToMaxHealth(_healthPoints);
        }

    }
}
