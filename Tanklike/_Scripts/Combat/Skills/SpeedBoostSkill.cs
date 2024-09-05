using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "SpeedBoost", menuName = MENU_ROOT + "Speed Boost")]
    public class SpeedBoostSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private float _speedMultiplier = 0.3f;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            // changes the tank's speed modifier
            components.Movement.MultiplySpeed(_speedMultiplier);
        }
    }
}
