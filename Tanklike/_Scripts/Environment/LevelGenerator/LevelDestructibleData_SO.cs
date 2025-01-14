using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Combat.Destructible;
    using Environment.MapMaker;

    [CreateAssetMenu(fileName = "Level Destructible Data", menuName = "Level/Level Generation/ Destructible Data")]
    public class LevelDestructibleData_SO : ScriptableObject
    {
        [ReadOnly] public List<DestructibleDrop> DropsData = new List<DestructibleDrop>
        {
            new DestructibleDrop() { Tag = DropperTag.Crate, Name = DestructableTag.Crate.ToString()},
            new DestructibleDrop() { Tag = DropperTag.Stone, Name = DestructableTag.Rock.ToString() }
        };
    }
}
