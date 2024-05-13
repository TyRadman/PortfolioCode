using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.Combat.Destructible;

namespace TankLike.Environment.MapMaker
{
#if UNITY_EDITOR
    public class MapSaver : MonoBehaviour
    {
        [HideInInspector] public List<Tile> Tiles;
        private const string DIRECTORY = "Assets/Environment/MapMaker/Rooms";
        private MapMakerManager _manager;
        public static Color GROUND_COLOR = Color.white;
        public static Color WALL_COLOR = new Color(1, 0.2358491f, 0.2358491f);
        //[SerializeField] private Color Color;

        private void Awake()
        {
            _manager = GetComponent<MapMakerManager>();
        }

        public void SaveMap()
        {
            string name = _manager.UI.GetMapName();
            string path = $"{DIRECTORY}/{name}.asset";

            if (System.IO.File.Exists(path))
            {
                _manager.UI.SetSaveMenuWarning($"Map with name Map already exists in {DIRECTORY}");
                return;
            }

            TileData[,] tiles = _manager.AllTiles;
            Tiles = new List<Tile>();
            MapTiles_SO map = ScriptableObject.CreateInstance<MapTiles_SO>();
            map.Name = name;

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tiles.Add(GetTileFromTileData(tiles[x, y]));
                }
            }

            map.Size = new Vector2Int(_manager.Selector.LevelDimensions.x, _manager.Selector.LevelDimensions.y);

            map.SetTiles(Tiles);
            AssetDatabase.CreateAsset(map, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CancelButton();
        }

        public Tile GetTileFromTileData(TileData tileData)
        {
            Tile tile = new Tile();
            tile.Dimension = tileData.Dimension;
            tile.Rotation = tileData.TileObject.transform.eulerAngles.y;
            tile.Tag = tileData.Tag;
            tile.Overlays = _manager.Overlays.GetOverlayTypeAtIndex(tile.Dimension.x, tile.Dimension.y);
            return tile;
        }

        #region Buttons
        public void SaveButton()
        {
            _manager.UI.ShowSaveMenu(true);
            _manager.Selector.IsActive = false;
            _manager.Controller.IsActive = false;
        }

        public void CancelButton()
        {
            _manager.UI.ShowSaveMenu(false);
            _manager.Selector.IsActive = true;
            _manager.Controller.IsActive = true;
        }
        #endregion
    }
#endif

    [System.Serializable]
    public class Tile
    {
        public TileTag Tag;
        public Vector2Int Dimension;
        public float Rotation;
        public List<DestructableTag> Overlays;
        public bool IsSpawnPoint = false;
    }
}
