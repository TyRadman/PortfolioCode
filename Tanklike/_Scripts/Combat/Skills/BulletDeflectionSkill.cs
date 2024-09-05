using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "BulletDeflection", menuName = MENU_ROOT + "Bullet Deflection")]
    public class BulletDeflectionSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private Deflection _deflectionInfo;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            TankShooter shooter = components.Shooter;
            shooter.SetDeflection(_deflectionInfo);
        }
    }
}
