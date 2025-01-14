using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TankLike.Environment.LevelGeneration
{
    using Combat.Destructible;

    /// <summary>
    /// Represents the data and logic for determining loot drops from destructible objects -<b>IDroppers</b>- upon their destruction.<br></br>
    /// This class defines the range and probability of items that will be dropped, as well as the highest drop chance.<br></br>
    /// It ensures that loot chances are reset to their original values when initialized.
    /// </summary>
    [System.Serializable]
    public class DestructibleDrop
    {
        [HideInInspector] public string Name;
        [ReadOnly] public DropperTag Tag;
        public List<DropChance> Drops;
        [HideInInspector] public float HighestChance = 1f;

        public void SetUp()
        {
            HighestChance = Drops.OrderByDescending(d => d.Chance).FirstOrDefault().CurrentChance;
            Drops.ForEach(d => d.CurrentChance = d.Chance);
        }

        public void UpdateHighestChance()
        {
            HighestChance = Drops.OrderByDescending(d => d.CurrentChance).FirstOrDefault().CurrentChance;
        }
    }
}
