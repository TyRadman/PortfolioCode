using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    public class OnImpact : ScriptableObject
    {
        public const string MENU_MAIN = Directories.SHOT_CONFIGURATIONS + "On Impact/";

        public virtual void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {

        }
    }
}
