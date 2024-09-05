using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "PD_NAME", menuName = Directories.PLAYERS + "Player Data")]
    public class PlayerData : UnitData
    {
        [field: SerializeField] public PlayerType PlayerType { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public UnitParts PartsPrefab { get; private set; }
        [field: SerializeField] public PlayerSkin[] Skins { get; private set; }
    }
}
