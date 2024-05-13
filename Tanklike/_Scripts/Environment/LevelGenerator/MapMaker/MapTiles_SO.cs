using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.Environment.LevelGeneration;
using TankLike.LevelGeneration;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    /// <summary>
    /// Holds a set of tiles (level theme)
    /// </summary>
    [CreateAssetMenu(fileName = "Map Tiles", menuName = "Level/ Map Tiles")]
    public class MapTiles_SO : ScriptableObject
    {
        public string Name;
        public List<Tile> Tiles;
        public int GateCount;
        public Vector2Int Size;
        public List<GateData> Surroundings = new List<GateData>(4)
        {
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.East},
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.North},
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.West},
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.South}
        };

        [System.Serializable]
        public class GateData
        {
            public Surrounding Surrounding;
            public GateDirection Direction;
        }

        public void SetTiles(List<Tile> tiles)
        {
            Tiles = new List<Tile>();
            tiles.ForEach(t => Tiles.Add(t));
            GateCount = Tiles.FindAll(t => t.Tag == TileTag.Gate).Count;

            // cache the gates
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile currentTile = Tiles[i];
                int maxXIndex = Tiles.OrderByDescending(t => t.Dimension.x).First().Dimension.x;
                int maxYIndex = Tiles.OrderByDescending(t => t.Dimension.y).First().Dimension.y;

                if (currentTile.Tag == TileTag.Gate)
                {
                    GateDirection direction = GameplayRoomGenerator.GetGateDirection(maxXIndex, maxYIndex, currentTile.Dimension);
                    int index = (int)direction / 90;
                    Surroundings[index].Surrounding = Surrounding.Gate;
                }
            }
        }
    }
}
