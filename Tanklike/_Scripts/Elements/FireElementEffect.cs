using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Fire Effect", menuName = "Elements/ Fire or Poison")]
    public class FireElementEffect : ElementEffect
    {
        [Header("Special Values")]
        [SerializeField] private int _damagePerSecond = 5;

        public override void TakeEffect(TankComponents tank)
        {
            tank.Health.TakeDamage((int)_damagePerSecond, Vector3.zero, tank, Vector3.zero);
            // play any effects

        }
    }
}
