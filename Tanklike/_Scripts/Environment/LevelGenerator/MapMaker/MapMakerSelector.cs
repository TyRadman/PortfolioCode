using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.Destructible;
using TankLike.Utils;
using UnityEditor;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerSelector : MonoBehaviour
    {
        public const int TILE_SIZE = 2;
        public Vector2Int LevelDimensions;
        [Header("References")]
        [SerializeField] private Room _roomPrefab;
        public List<TileData> DisplayTiles;
        [SerializeField] private Transform _pointer;
        [SerializeField] private LayerMask _layerMaskForTiles;
        [Tooltip("The tags of the tiles that will be instantiated at start to be displayed whenever this type of tile is chosen to build.")]
        [SerializeField] private List<TileTag> _tagsToDisplay;
        private Camera _cam;
        private const float RAY_DISTANCE = 40f;
        private GameObject _tileToBuild;
        private TileType _currentType;
        private Vector2Int _currentTileIndex;
        [SerializeField] private MapMakerManager _manager;
        public bool IsActive = true;

        private void Awake()
        {
            _cam = Camera.main;
            SetUpGround();
            InstantiatePrefab();
        }

        private void Update()
        {
            if (!IsActive) return;

            CheckForPointerPosition();
            CheckForInput();
        }

        private void CheckForInput()
        {
            if (Input.GetMouseButton(1))
            {
                if (_tileToBuild == null) return;

                // if the tile we're place is the same type as the tile already placed AND there are no overlays, then stop
                if(_manager.Overlays.IsEmptyOverlay(_currentTileIndex.x, _currentTileIndex.y) &&
                    _currentType == MapMakerManager.GetGeneralTypeFromSpecificType(_manager.AllTiles[_currentTileIndex.x, _currentTileIndex.y].Tag)) return;

                OnMouseClick();
            }
        }

        private void OnMouseClick()
        {
            if (_tileToBuild == null) return;

            if (_currentType == TileType.Gate)
            {
                _manager.GateArranger.PlaceTile(ref _manager.AllTiles, _currentTileIndex.x, _currentTileIndex.y);
            }
            else if (_currentType == TileType.Wall)
            {
                _manager.WallArranger.PlaceTile(ref _manager.AllTiles, _currentTileIndex.x, _currentTileIndex.y);
            }
            else if (_currentType == TileType.Ground)
            {
                _manager.GroundArranger.PlaceTile(ref _manager.AllTiles, _currentTileIndex.x, _currentTileIndex.y);
            }
            else if (_currentType == TileType.Overlay)
            {
                _manager.Overlays.PlaceTile(ref _manager.AllTiles, _currentTileIndex.x, _currentTileIndex.y, _manager.Overlays.CurrentOverlayType);
            }
        }

        public void CheckForPointerPosition()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, RAY_DISTANCE, _layerMaskForTiles))
            {
                GameObject tile = hit.collider.gameObject;

                if (tile == null) return;

                _pointer.position = new Vector3(tile.transform.position.x, 0f, tile.transform.position.z);
                TileData tileData = FindTileDataByObject(_manager.AllTiles, tile);
                _currentTileIndex = IndexOfTile(tileData, _manager.AllTiles);
            }
        }

        // lay down the ground tiles based on the dimensions and center them in the middle of the world
        public void SetUpGround()
        {
            // create a parent for the room
            if (_manager.Room != null) Destroy(_manager.Room.gameObject);

            _manager.Room = Instantiate(_roomPrefab, Vector3.zero, Quaternion.identity);

            // set dimensions for the array
            _manager.AllTiles = new TileData[LevelDimensions.x, LevelDimensions.y];
            // set the starting position at the lower left corner to make building easier
            Vector3 startingPosition = GetStartingPositionForTiles();
            GameObject groundTile = _manager.Styler.GetTile(TileTag.Ground);

            // build the tiles
            for (int i = 0; i < LevelDimensions.y; i++)
            {
                for (int j = 0; j < LevelDimensions.x; j++)
                {
                    Vector3 position = startingPosition + new Vector3(j, 0f, i) * TILE_SIZE;
                    GameObject tileObject = Instantiate(groundTile, _manager.Room.transform);
                    TileData tile = new TileData();
                    tile.SetTileObject(tileObject, TileTag.Ground);

                    tile.SetArranger(_manager.GroundArranger);
                    tile.TileObject.transform.position = position;
                    tile.SetName(j, i);
                    _manager.AllTiles[j, i] = tile;
                }
            }
        }

        public Vector3 GetStartingPositionForTiles()
        {
            return new Vector3(-((float)(LevelDimensions.x / 2f)) * TILE_SIZE + TILE_SIZE / 2, 0f, -((float)(LevelDimensions.y / 2f)) * TILE_SIZE + TILE_SIZE / 2);
        }

        private void InstantiatePrefab()
        {
            DisplayTiles = new List<TileData>();

            for (int i = 0; i < _tagsToDisplay.Count; i++)
            {
                GameObject tileObject = Instantiate(_manager.Styler.Tiles.Find(t => t.Tag == _tagsToDisplay[i]).Tiles[0]);
                TileData tile = new TileData();
                tile.SetTileObject(tileObject, _tagsToDisplay[i]);

                DisplayTiles.Add(tile);
                tile.TileObject.SetActive(false);
                tile.EnableBoxCollider(false);
            }
        }

        public void SetTileToBuild(TileType type)
        {
            SetCurrentSelection(type, DisplayTiles.Find(t => MapMakerManager.TagEquals(t.Tag, type)).TileObject);
        }

        public void SetOverLayToBuild(DestructableTag type)
        {
            SetCurrentSelection(TileType.Overlay, _manager.Overlays.GetDisplayTile(type));
            _manager.Overlays.CurrentOverlayType = type;
        }

        private void SetCurrentSelection(TileType type, GameObject tileObject)
        {
            if (_tileToBuild != null)
            {
                _tileToBuild.gameObject.SetActive(false);
            }

            _tileToBuild = tileObject;
            _tileToBuild.transform.parent = _pointer.transform;
            _tileToBuild.transform.localPosition = Vector3.zero;
            _tileToBuild.SetActive(true);
            _currentType = type;
        }

        public static Vector2Int IndexOfTile(TileData tile, TileData[,] tiles)
        {
            Vector2Int index = Vector2Int.zero;

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tile == tiles[i, j])
                    {
                        index = new Vector2Int(i, j);
                        return index;
                    }
                }
            }

            return index;
        }

        public static TileData FindTileDataByObject(TileData[,] tiles, GameObject tileObject)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i, j].TileObject == tileObject)
                    {
                        return tiles[i, j];
                    }
                }
            }

            Debug.LogError($"Tile object {tileObject.name} doesn't exist in the mapmaker manager list");
            return null;
        }
    }
}