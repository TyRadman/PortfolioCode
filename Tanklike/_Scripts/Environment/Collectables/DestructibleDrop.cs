using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.Destructible;
using UnityEngine;
using System.Linq;

namespace TankLike.Environment.LevelGeneration
{
    [System.Serializable]
    public class DestructibleDrop
    {
        [HideInInspector] public string Name;
#if UNITY_EDITOR
        [ReadOnly()]
#endif
        [HideInInspector] public DestructableTag Tag;
        public Vector2Int DropsCountRange;
        public List<DropChance> Drops;
        [HideInInspector] public float HighestChance = 1f;

        public void SetUp()
        {
            HighestChance = Drops.OrderByDescending(d => d.OriginalChance).FirstOrDefault().Chance;
            Drops.ForEach(d => d.Chance = d.OriginalChance);
        }

        public void UpdateHighestChance()
        {
            HighestChance = Drops.OrderByDescending(d => d.Chance).FirstOrDefault().Chance;
        }
    }
}
