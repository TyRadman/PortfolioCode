using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.Destructible;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class OverlaysArranger : MonoBehaviour
    {
        [System.Serializable]
        public class OverlayInfo
        {
            public DestructableTag Tag;
            public GameObject Model;
            public Vector2Int PositionIndex;
        }

        [System.Serializable]
        public class Overlay
        {
            public List<DestructableTag> CurrentOverlayTypes = new List<DestructableTag>();
            public Color OverlayColor;
            public Vector2Int Dimension;
            public List<OverlayInfo> Info = new List<OverlayInfo>();

            public void SetModel(List<OverlayInfo> info, Transform parent, Vector3 position)
            {
                for (int i = 0; i < info.Count; i++)
                {
                    float offset = MapMakerSelector.TILE_SIZE / 2f;
                    Vector3 modelPosition = position - new Vector3(offset, 0f, offset);
                    modelPosition += new Vector3(TILE_SIZE + info[i].PositionIndex.x * TILE_SIZE, 0f, 
                        TILE_SIZE + info[i].PositionIndex.y * TILE_SIZE);

                    OverlayInfo overlayInfo = new OverlayInfo();
                    overlayInfo.Model = Instantiate(info[i].Model, modelPosition, Quaternion.identity, parent);
                    overlayInfo.Model.SetActive(false);
                    overlayInfo.Tag = info[i].Tag;
                    Info.Add(overlayInfo);
                }
            }

            public void ActivateVisual(DestructableTag tag)
            {
                Info.Find(t => t.Tag == tag).Model.SetActive(true);

                if (!CurrentOverlayTypes.Exists(t => t == tag)) CurrentOverlayTypes.Add(tag);
            }

            public GameObject GetDisplayTile(DestructableTag tag)
            {
                return Info.Find(t => t.Tag == tag).Model;
            }

            public void DisableVisuals()
            {
                CurrentOverlayTypes = new List<DestructableTag>();
                Info.ForEach(t => t.Model.SetActive(false));
            }
        }

        public const float TILE_SIZE = 0.5f;
        [SerializeField] private List<OverlayInfo> OverlayInfos;
        private List<Overlay> CurrentOverlays = new List<Overlay>();
        private Overlay DisplayOverlay = new Overlay();
        [SerializeField] private MapMakerManager _manager;
        [SerializeField] private GameObject _overlayBoxPrefab;
        public DestructableTag CurrentOverlayType;
        private Transform _parent;

        private void Start()
        {
            SetUpOverlayBoxes();
            CreateDisplayTiles();
        }

        /// <summary>
        /// Should be called once only. When the scene starts.
        /// </summary>
        private void SetUpOverlayBoxes()
        {
            int xNum = _manager.Selector.LevelDimensions.x;
            int yNum = _manager.Selector.LevelDimensions.y;
            Vector3 startingPosition = _manager.Selector.GetStartingPositionForTiles();
            _parent = new GameObject("Overlays").transform;

            for (int i = 0; i < xNum; i++)
            {
                for (int j = 0; j < yNum; j++)
                {
                    Vector3 position = startingPosition + new Vector3(j, 0f, i) * MapMakerSelector.TILE_SIZE;
                    Overlay overlay = new Overlay
                    {
                        Dimension = new Vector2Int(j, i),
                    };

                    overlay.SetModel(OverlayInfos, _parent, position);
                    CurrentOverlays.Add(overlay);
                }
            }
        }

        private void CreateDisplayTiles()
        {
            Overlay overlay = new Overlay();
            overlay.SetModel(OverlayInfos, _parent, Vector3.zero);
            DisplayOverlay = overlay;
        }

        public void PlaceTile(ref TileData[,] tiles, int x, int y, DestructableTag tag)
        {
            // if the tile we're setting as the one having a destructible is not a ground tile, then stop
            if(!MapMakerManager.TagEquals(tiles[x, y].Tag, TileType.Ground) || tiles[x, y].GatePart != TileData.GatePartType.None)
            {
                _manager.UI.DisplayMessage($"Can only place {tag}s on ground tiles");
                return;
            }

            Overlay overlayToShow = CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y);

            if(overlayToShow.CurrentOverlayTypes.Exists(o => o == DestructableTag.SpawnPoint)
                && tag != DestructableTag.SpawnPoint)
            {
                _manager.UI.DisplayMessage($"Can't place {tag} on a spawn point");
                return;
            }

            // set the tile as one having a destructible
            overlayToShow.ActivateVisual(tag);
        }

        public void RemoveTile(int x, int y)
        {
            Overlay overlay = CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y);
            overlay.CurrentOverlayTypes = new List<DestructableTag>();
            overlay.DisableVisuals();
        }

        public bool IsEmptyOverlay(int x, int y)
        {
            return CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y).CurrentOverlayTypes.Count == 0;
        }

        public GameObject GetDisplayTile(DestructableTag tag)
        {
            DisplayOverlay.DisableVisuals();
            DisplayOverlay.ActivateVisual(tag);
            return DisplayOverlay.GetDisplayTile(tag);
        }

        public List<DestructableTag> GetOverlayTypeAtIndex(int x, int y)
        {
            List<DestructableTag> destructibleTags = new List<DestructableTag>();
            CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y).CurrentOverlayTypes.ForEach(o => destructibleTags.Add(o));
            return destructibleTags;
        }

        public void ClearOverlays()
        {
            for (int i = 0; i < CurrentOverlays.Count; i++)
            {
                CurrentOverlays[i].DisableVisuals();
            }
        }
    }
}
