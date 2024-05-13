using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat.Destructible;

namespace TankLike.Environment.LevelGeneration
{
    [CreateAssetMenu(fileName = "Level Destructible Data", menuName = "Level/Level Generation/ Destructible Data")]
    public class LevelDestructibleData_SO : ScriptableObject
    {
        public List<DestructibleDrop> DropsData = new List<DestructibleDrop>
        {
            new DestructibleDrop() { Tag = DestructableTag.Crate, Name = DestructableTag.Crate.ToString()},
            new DestructibleDrop() { Tag = DestructableTag.Rock, Name = DestructableTag.Rock.ToString() }
        };
    }
}
