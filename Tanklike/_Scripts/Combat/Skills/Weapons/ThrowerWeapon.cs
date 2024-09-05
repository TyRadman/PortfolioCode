using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_Thrower_Default", menuName = DIRECTORY + "Thrower")]
    public class ThrowerWeapon : Weapon
    {
        [field: SerializeField] public float BulletSpeed { get; private set; }
        [field: SerializeField] public float MaxDistance = 100f;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            // create the pools for the bullet

        }

        public override void SetSpeed(float speed)
        {
            BulletSpeed = speed;
        }
    }
}
