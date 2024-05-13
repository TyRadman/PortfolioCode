using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    [System.Serializable]
    public class EnemyWave
    {
        public List<EnemySpawnProfile> Enemies;
        public bool HasKey = false;

        public EnemyWave()
        {
            Enemies = new List<EnemySpawnProfile>();
        }
    }
}
