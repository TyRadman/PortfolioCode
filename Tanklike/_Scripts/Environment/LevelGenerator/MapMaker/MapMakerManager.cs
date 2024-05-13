using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.Combat.Destructible;
using TankLike.Environment.LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerManager : MonoBehaviour
    {
        public MapMakerWallArranger WallArranger;
        public MapMakerGateArranger GateArranger;
        public MapMakerCameraControl Controller;
        public GressArranger GroundArranger;
        public MapMakerSelector Selector;
        public OverlaysArranger Overlays;
        public MapMakerUI UI;
        public TileData[,] AllTiles;
        [HideInInspector] public Room Room;
        public MapTileStyler Styler;
        public MapMakerRoomGenerator Generator;
        [Header("Map Loading")]
        [SerializeField] private MapTiles_SO _mapToBuild;
        [SerializeField] private LevelData _styler;
        [SerializeField] private int _tilesCount = 0;

        private void Start()
        {
            CorrectWalls();
        }

        private void Update()
        {
            _tilesCount = AllTiles.GetLength(0) * AllTiles.GetLength(1);
        }

        public void CorrectWalls()
        {
            WallArranger.CheckTiles(ref AllTiles, 0, 0, false);
        }

        /// <summary>
        /// Use this method to compare whether a ground tile is ground, or if a specific wall is a general wall
        /// </summary>
        /// <param name="tileToCompare"></param>
        /// <param name="tileToCompareTo"></param>
        /// <returns></returns>
        public static bool TagEquals(TileTag tileToCompare, TileType tileToCompareTo)
        {
            int tile1 = (int)tileToCompare;

            if (tileToCompareTo == TileType.Ground)
            {
                if (tile1 >= 0 && tile1 <= 3) return true;
            }
            else if(tileToCompareTo == TileType.Wall)
            {
                if (tile1 >= 4 && tile1 <= 9) return true;
            }
            else if(tileToCompareTo == TileType.Gate)
            {
                if (tile1 == 10) return true;
            }

            return false;
        }

        public static TileType GetGeneralTypeFromSpecificType(TileTag type)
        {
            int tile1 = (int)type;
            if (tile1 >= 0 && tile1 <= 3) return TileType.Ground;
            if (tile1 >= 4 && tile1 <= 9) return TileType.Wall;
            else return TileType.Gate;
        }

        public void ClearLevel()
        {
            Selector.SetUpGround();
            Overlays.ClearOverlays();
        }

        [ContextMenu("Load Map")]
        public void LoadMap()
        {
            Generator.BuildRoom(_mapToBuild, _styler, Room);
        }
    }

    public enum TileType
    {
        Ground = 0, Wall = 1, Gate = 2, Overlay = 3, SpawnPoint = 4
    }

    public enum TileTag
    {
        Ground = 0, Ground_OneSide = 1, Ground_OutCorner = 2, Ground_InCorner = 3, 
        Wall_NoSides = 4, Wall_OneSide = 5, Wall_TwoSides = 6, Wall_Corner = 7, Wall_ThreeSides = 8, Wall_FourSides = 9,
        Gate = 10
    }
}
