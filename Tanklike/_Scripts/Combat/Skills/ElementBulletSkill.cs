using TankLike.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "BulletElement", menuName = MENU_ROOT + "Bullet Element")]
    public class ElementBulletSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private ElementEffect _element;

        // what the skill does when it is added to the tank 
        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            components.Shooter.SetElement(_element);
        }

    }
}
