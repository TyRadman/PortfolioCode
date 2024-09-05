using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public static class Constants
    {
        public static int PlayerLayer { get; private set; } = 11;
        public static int EnemyLayer { get; private set; } = 10;
        public static int PlayerDamagableLayer { get; private set; } = 23;
        public static int EnemyDamagableLayer { get; private set; } = 24;
        public static float GroundHeight { get; private set; } = 0.1f;
        [Tooltip("The ratio of items' prices when they're sold vs bought.")]
        public const float SHOP_SELL_ITEMS_PRICE_MULTIPLIER = 0.25f;
        public const int REVIVAL_COST = 300;
        public static LayerMask MutualHittableLayer { get; private set; } = 1 << 12;
        public static LayerMask WallLayer { get; private set; } = 1 << 8;
        public static LayerMask GroundLayer { get; private set; } = 1 << 14;
        // tools
        public static int MaxToolsUsageCount { get; private set; } = 7;
        // UI
        public static float ButtonRepeatFrequency { get; private set; } = 0.1f;
        public static float PreButtonRepeatWaitTime { get; private set; } = 0.5f;
    }

    public static class Directories
    {
        public const string MAIN = "TankLike/";
        public const string COMBAT = MAIN + "Combat/";
        public const string EDITOR = MAIN + "Editor/";
        public const string CAMERA = MAIN + "Camera/";
        public const string AMMUNITION = MAIN + "Ammunition/";
        public const string SHOT_CONFIGURATIONS = AMMUNITION + "Shot Configurations/";
        public const string ABILITIES = MAIN + "Abilities/";
        public const string ABILITIES_HOLDER = MAIN + "Abilities/Holders/";
        public const string SKILL_TREE = MAIN + "Skills/Skill Tree/";
        public const string PLAYERS = MAIN + "Players/";
        public const string ENEMIES = MAIN + "Enemies/";
        public const string BOSSES = MAIN + "Bosses/";
        public const string SKILLS = MAIN + "Skills/";
        public const string TOOLS = MAIN + "Tools/";
        public const string TOOLS_SETTINGS = TOOLS + "Settings/";
        public const string UI = MAIN + "UI/";

        public const string DATA_COLLECTION = MAIN + "Data Collection/";
    }
}
