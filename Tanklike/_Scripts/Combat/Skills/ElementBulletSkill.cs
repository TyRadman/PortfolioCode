using TankLike.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "Bullet Element Skill", menuName = "Skills/Stats/Bullet Element")]
    public class ElementBulletSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private ElementEffect _element;

        // what the skill does when it is added to the tank 
        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            tankTransform.GetComponent<PlayerShooter>().SetElement(_element);
        }

    }
}
