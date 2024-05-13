using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Environment
{
    [System.Serializable]
    public class EnemySpawnProfile
    {
        public EnemyType Enemy;
        public bool HasKey;
        public bool CanHaveKey = true;

        public EnemySpawnProfile(EnemyType enemy)
        {
            // check whehter this enemy is one of the enemies that can't hold keys
            CanHaveKey = !GameManager.Instance.Constants.NotKeyHolderEnemyTags.Exists(t => t == enemy);
            Enemy = enemy;
            HasKey = false;
        }
    }
}
