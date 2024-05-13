using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "Speed Boost Skill", menuName = "Skills/Stats/Speed Boost")]
    public class SpeedBoostSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private float _speedMultiplier = 0.3f;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            // changes the tank's speed modifier
            tankTransform.GetComponent<TankMovement>().MultiplySpeed(_speedMultiplier);
        }
    }
}
