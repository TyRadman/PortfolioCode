using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.MapMaker;
using TankLike.Sound;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    /// <summary>
    /// Holds the values that distinct a level from another.
    /// </summary>
    [CreateAssetMenu(fileName = "Level Data", menuName = "Level/Level Data")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public MapTileStyler Styler { get; private set; }
        [field: SerializeField] public MapTiles_SO BossRoom { get; private set; }
        [field: SerializeField] public BossRoomGate BossGate { get; private set; }
        [field: SerializeField] public BossData BossData { get; private set; }
        [field: SerializeField] public Audio LevelMusic { get; private set; }
        [field: SerializeField] public List<MapTiles_SO> MapPools { get; private set; }
        [field: SerializeField] public Vector2Int DroppersRange { get; private set; }
        [field: SerializeField] public List<WaveData> Waves { get; private set; }

    }
}
