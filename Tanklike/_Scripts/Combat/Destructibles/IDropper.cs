using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.LevelGeneration;
using UnityEngine;

namespace TankLike.Combat.Destructible
{
    public interface IDropper
    {
        List<DropChance> CollecatblesToSpawn { set; get; }
        DestructableTag Tag { get; }
        void SetCollectablesToSpawn(DestructibleDrop collectables);
    }
}
