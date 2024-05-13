using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public static class Constants
    {
        public static int PlayerLayer = 11;
        public static int EnemyLayer = 10;
        public static int PlayerDamagableLayer = 23;
        public static int EnemyDamagableLayer = 24;
        [Tooltip("The ratio of items' prices when they're sold vs bought.")]
        public const float SHOP_SELL_ITEMS_PRICE_MULTIPLIER = 0.25f;
        public const int REVIVAL_COST = 300;
        public static LayerMask MutualHittableLayer = 1 << LayerMask.NameToLayer("MutualHittable");
        public static LayerMask WallLayer = 1 << LayerMask.NameToLayer("Wall");
        public static LayerMask GroundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    public static class Directories
    {
        public const string MAIN = "TankLike/";
        public const string COMBAT = MAIN + "Combat/";
        public const string EDITOR = MAIN + "Editor/";
        public const string CAMERA = MAIN + "Camera/";
        public const string AMMUNITION = MAIN + "Ammunition/";
        public const string ABILITIES = MAIN + "Abilities/";
        public const string SKILL_TREE = MAIN + "Skills/Skill Tree/";
        public const string ENEMIES = MAIN + "Enemies/";
        public const string BOSSES = MAIN + "Bosses/";
        public const string SKILLS = MAIN + "Skills/";
    }
}
