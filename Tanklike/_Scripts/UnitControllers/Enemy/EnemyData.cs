using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// A unique
    /// </summary>
    [CreateAssetMenu(fileName = "ED_NAME", menuName = Directories.ENEMIES + "Enemy data")]
    public class EnemyData : TankData
    {
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float SpawnYOffset { get; private set; }
        [field: SerializeField] public int ExperiencePerKill { get; private set; }
        [field: SerializeField] public int Rank { get; private set; }
        [field: SerializeField] public EnemyParts PartsPrefab { get; private set; }

    }
}