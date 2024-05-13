using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = PREFIX + "CustomShot", menuName = Directories.ABILITIES + "Custom shot")]
    public class CustomShot_OnHoldSkill : Ability
    {
        [Header("Special Values")]
        [SerializeField] private Weapon _weapon;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            _components = tankTransform.GetComponent<PlayerComponents>();
        }

        public override void OnActivateAbility()
        {
            base.OnActivateAbility();

            _weapon.OnShot(_components);
        }
    }
}
