using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "Bullet Deflection Skill", menuName = "Skills/Stats/Bullet Deflection")]
    public class BulletDeflectionSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private Deflection _deflectionInfo;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            TankShooter shooter = tankTransform.GetComponent<TankShooter>();
            shooter.SetDeflection(_deflectionInfo);
        }
    }
}
