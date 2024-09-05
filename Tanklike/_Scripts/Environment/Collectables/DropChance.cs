using System.Collections;
using System.Collections.Generic;
using TankLike.ItemsSystem;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    [System.Serializable]
    public class DropChance
    {
        public CollectableType DropType;
        /*[HideInInspector]*/
        public float Chance = 0.5f;
        [Range(0f, 1f)] public float OriginalChance = 1f;
        public Vector2Int CountRange;
        [Header("Chances Affectors")]
        public ChanceAffector Affector;
        public Vector2 EffectRange;
    }
}
