using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.Combat.Destructible;
using TankLike.Environment.LevelGeneration;
using TankLike.LevelGeneration;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerRoomGenerator : MonoBehaviour, IRoomGenerator
    {
        private MapMakerManager _manager;

        private void Awake()
        {
            _manager = GetComponent<MapMakerManager>();
        }

        [ContextMenu("Load Map")]
        public void BuildRoom(MapTiles_SO map, LevelData level, Room room, BuildConfigs configs = null)
        {
            // remove previous tile
            for (int i = 0; i < _manager.AllTiles.GetLength(0); i++)
            {
                for (int j = 0; j < _manager.AllTiles.GetLength(1); j++)
                {
                    Destroy(_manager.AllTiles[i, j].TileObject);
                }
            }

            _manager.Overlays.ClearOverlays();

            int xDimension = map.Tiles.Max(t => t.Dimension.x) + 1;
            int yDimension = map.Tiles.Max(t => t.Dimension.y) + 1;

            Vector3 startingPosition = new Vector3(-MapMakerSelector.TILE_SIZE * xDimension / 2f + MapMakerSelector.TILE_SIZE / 2,
                0f, -MapMakerSelector.TILE_SIZE * yDimension / 2f + MapMakerSelector.TILE_SIZE / 2);
            // set the 2D array dimensions
            _manager.AllTiles = new TileData[xDimension, yDimension];
            List<List<DestructableTag>> overlays = new List<List<DestructableTag>>();

            for (int i = 0; i < map.Tiles.Count; i++)
            {
                Tile tile = map.Tiles[i];
                GameObject tileToBuildPrefab = level.Styler.GetTile(tile.Tag);
                // create the tile as a child to the room

                if (tileToBuildPrefab == null)
                {
                    Debug.LogError($"Tile of type {tile.Tag} doesn't exist in {level.Styler.name} styler");
                    Debug.Break();
                }

                GameObject tileToBuild = Instantiate(tileToBuildPrefab, room.transform);
                TileData tileData = new TileData();
                tileData.SetTileObject(tileToBuild, tile.Tag);

                // rename it
                tileToBuild.name = $"{tile.Dimension.x},{tile.Dimension.y}";
                // set the position of the tile
                tileToBuild.transform.position = startingPosition + new Vector3(tile.Dimension.x, 0f, tile.Dimension.y) * MapMakerSelector.TILE_SIZE;
                tileData.Dimension = tile.Dimension;
                // set the tile rotation
                tileToBuild.transform.eulerAngles += Vector3.up * tile.Rotation;
                // assign it to the 2D array
                _manager.AllTiles[tile.Dimension.x, tile.Dimension.y] = tileData;
                SetAssigner(tileData);

                if (tile.Overlays.Count > 0)
                {
                    overlays.Add(tile.Overlays);
                    //Selector.SetOverLayToBuild(tile.Overlay);
                    tile.Overlays.ForEach(o => _manager.Overlays.PlaceTile(ref _manager.AllTiles, tile.Dimension.x, tile.Dimension.y, o));
                }
            }
        }

        public void SetAssigner(TileData tile)
        {
            int tileTag = (int)tile.Tag;

            if (tileTag >= 0 && tileTag <= 3)
            {
                tile.Arranger = _manager.GroundArranger;
            }
            else if (tileTag >= 1 && tileTag <= 6)
            {
                tile.Arranger = _manager.GroundArranger;
            }
            else if (tileTag == (int)TileTag.Gate)
            {
                tile.Arranger = _manager.GateArranger;
            }
        }
    }
}
