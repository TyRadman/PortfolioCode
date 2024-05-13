using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Destructible
{
    public interface IDestructable 
    {
        DestructableTag Tag { get; }
    }

    public enum DestructableTag
    {
        None = 1000, Crate = 0, Rock = 1, Wall = 2, SpawnPoint = 3, BossSpawnPoint = 4
    }
}
