using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.Cam;
using TankLike.Combat.Destructible;
using TankLike.Environment;
using TankLike.Environment.LevelGeneration;
using TankLike.Environment.MapMaker;
using TankLike.Utils;
using UnityEngine;
using static TankLike.Environment.MapMaker.MapTileStyler;

namespace TankLike.LevelGeneration
{
    /// <summary>
    /// Builds an individual room
    /// </summary>
    public class GameplayRoomGenerator : MonoBehaviour, IRoomGenerator
    {
        [SerializeField] private RoomGate _gateToBuild;
        private Room _currentRoom;

        public void BuildRoom(MapTiles_SO map, LevelData level, Room room)
        {
            int xDimension = map.Tiles.Max(t => t.Dimension.x) + 1;
            int yDimension = map.Tiles.Max(t => t.Dimension.y) + 1;
            room.RoomDimensions = new Vector2Int(xDimension, yDimension);
            _currentRoom = room;
            Vector3 startingPosition = new Vector3(-MapMakerSelector.TILE_SIZE * xDimension / 2f + MapMakerSelector.TILE_SIZE / 2,
                0f, -MapMakerSelector.TILE_SIZE * yDimension / 2f + MapMakerSelector.TILE_SIZE / 2);
            List<Vector2Int> overlayIndices = new List<Vector2Int>();
            // rename the room to the map's name
            room.gameObject.name = map.name;
            Transform tilesParent = new GameObject("Tiles").transform;
            tilesParent.parent = room.transform;
            Transform gatesParent = new GameObject("Gates").transform;
            gatesParent.parent = room.transform; 
            Transform spawnPointsParent = new GameObject("SpawnPoints").transform;
            spawnPointsParent.parent = room.transform;

            // variables for adding the gates
            int maxXIndex = map.Tiles.OrderByDescending(t => t.Dimension.x).First().Dimension.x;
            int maxYIndex = map.Tiles.OrderByDescending(t => t.Dimension.y).First().Dimension.y;

            for (int i = 0; i < map.Tiles.Count; i++)
            {
                Tile currentTile = map.Tiles[i];
                // get the corresponding tile from the styler
                GameObject tileToBuildPrefab;

                if (currentTile.Tag != TileTag.Gate)
                {
                    tileToBuildPrefab = level.Styler.GetRandomTileByTag(currentTile.Tag);
                }
                else
                {
                    // build a ground for the gap
                    tileToBuildPrefab = level.Styler.GetRandomTileByTag(TileTag.Ground);

                    GameObject gate = Instantiate(_gateToBuild.gameObject, gatesParent);

                    GateInfo info = new GateInfo()
                    {
                        Gate = gate.GetComponent<RoomGate>(),
                    };

                    GateDirection direction = GetGateDirection(maxXIndex, maxYIndex, currentTile.Dimension);
                    info.SetDirection(direction);
                    // we set the local direction because as the room rotates during the level generation, the main direction is change, and this results in a loss of the gates' original index in relation to the room's initial direction
                    info.SetLocalDirection(direction);
                    info.Gate.Direction = info.Direction;
                    room.GatesInfo.Gates.Add(info);

                    gate.name = $"{currentTile.Dimension.x},{currentTile.Dimension.y}";
                    // set the position of the tile
                    Vector3 gatePosition = startingPosition + new Vector3(currentTile.Dimension.x, 0f, currentTile.Dimension.y) * MapMakerSelector.TILE_SIZE;
                    gate.transform.position = gatePosition;
                    // set the tile rotation
                    gate.transform.eulerAngles += Vector3.up * currentTile.Rotation;
                }

                // create the tile as a child to the room
                if (tileToBuildPrefab == null)
                {
                    Debug.LogError($"Tile of type {currentTile.Tag} doesn't exist in {level.Styler.name} styler");
                }

                // instantiate the tile
                GameObject tileToBuild = Instantiate(tileToBuildPrefab, tilesParent);
                // rename it
                tileToBuild.name = $"{currentTile.Dimension.x},{currentTile.Dimension.y}";
                // set the position of the tile
                Vector3 position = startingPosition + new Vector3(currentTile.Dimension.x, 0f, currentTile.Dimension.y) * MapMakerSelector.TILE_SIZE;
                tileToBuild.transform.position = position;
                // set the tile rotation
                tileToBuild.transform.eulerAngles += Vector3.up * currentTile.Rotation;

                if (currentTile.Overlays.Exists(o => o == DestructableTag.SpawnPoint))
                {
                    room.Spawner.SpawnPoints.AddSpawnPoint(position, spawnPointsParent);
                    continue;
                }

                if(currentTile.Overlays.Exists(o => o == DestructableTag.BossSpawnPoint))
                {
                    GameObject spawnPoint = new GameObject("Boss Spawn Point");
                    ((BossRoom)room).SetBossSpawnPoint(spawnPoint.transform);
                    continue;
                }
            }

            // create the droppers
            CreateDroppers(map.Tiles, level, startingPosition, room);
            // set camera limits
            SetLevelLimits(map, room);

            // sort the gates' list
            room.GatesInfo.SortGates();
        }

        public static GateDirection GetGateDirection(int maxX, int maxY, Vector2Int tileDimension)
        {
            if (tileDimension.x == maxX) return GateDirection.East;
            else if (tileDimension.x == 0) return GateDirection.West;
            else if (tileDimension.y == maxY) return GateDirection.North;
            else return GateDirection.South;
        }

        public void SetLevelLimits(MapTiles_SO map, Room room)
        {
            CameraLimits limits = new CameraLimits();
            float leftLimit = (map.Size.x * MapMakerSelector.TILE_SIZE) / 2;
            float rightLimit = (map.Size.x * MapMakerSelector.TILE_SIZE) / 2;
            float downLimit = (map.Size.x * MapMakerSelector.TILE_SIZE) / 2;
            float upLimit = (map.Size.x * MapMakerSelector.TILE_SIZE) / 2;
            limits.HorizontalLimits = new Vector2(-leftLimit, rightLimit);
            limits.VerticalLimits = new Vector2(-downLimit, upLimit);
            room.SetCameraLimits(limits);
        }

        private void CreateDroppers(List<Tile> tiles, LevelData level, Vector3 startingPosition, Room room)
        {
            // create a parent for the droppers (like crates, rocks, etc)
            Transform droppersParent = new GameObject("Droppers").transform;
            // set the droppers parent as a child of the room
            droppersParent.parent = _currentRoom.transform;
            // find tiles that have droppers in them
            List<Tile> dropTiles = tiles.FindAll(t => t.Overlays.Count > 0 && !t.Overlays.Contains(DestructableTag.SpawnPoint));
            // shuffle them to add randomness
            dropTiles.Shuffle();
            int dropCount = Mathf.Clamp(Random.Range(level.DroppersRange.x, level.DroppersRange.y), 0, dropTiles.Count);

            for (int i = 0; i < dropCount; i++)
            {
                Tile currentTile = dropTiles[i];
                Vector3 position = startingPosition + new Vector3(currentTile.Dimension.x, 0f, currentTile.Dimension.y) * MapMakerSelector.TILE_SIZE;
                GameObject dropper = GetOverlay(level.Styler.OverlayReferences, currentTile.Overlays.RandomItem());

                if (dropper != null)
                {
                    Transform spawnedDropper = Instantiate(dropper, position, Quaternion.identity, droppersParent).transform;
                    GameManager.Instance.DestructiblesManager.SetDestructibleValues(spawnedDropper.GetComponent<IDropper>());
                    room.AddDropper(spawnedDropper);
                }
            }
        }

        public void ReplaceGateWithBossGate(BossRoomGate bossGate, GateInfo gateToReplace, Room room)
        {
            // create the boss gate
            RoomGate gate = gateToReplace.Gate;
            bossGate = Instantiate(bossGate, gate.transform.position, gate.transform.rotation, gate.transform.parent);
            // connect the boss gate to whatever the previous gate was connected to
            bossGate.SetConnection(gate.ConnectedRoom, gate.ConnectedGate);
            gate.ConnectedGate.SetConnection(room, bossGate);
            gateToReplace.Gate = bossGate;
            // destroy the old gate
#if UNITY_EDITOR
            DestroyImmediate(gate.gameObject);
#else
            Destroy(gate.gameObject);
#endif
        }

        public GameObject GetOverlay(List<OverLays> overlays, DestructableTag tag)
        {
            if (!overlays.Exists(o => o.Tag == tag))
            {
                return null;
            }

            return overlays.Find(o => o.Tag == tag).OverlayObject;
        }
    }
}
