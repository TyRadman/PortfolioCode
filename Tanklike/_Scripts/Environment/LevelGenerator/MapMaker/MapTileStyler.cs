using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using TankLike.Combat.Destructible;

namespace TankLike.Environment.MapMaker
{
    [CreateAssetMenu(fileName = "Map Styler", menuName = "Level/ Map Styler")]
    public class MapTileStyler : ScriptableObject
    {
        [System.Serializable]
        public class TilesGroup
        {
            public TileTag Tag;
            public List<GameObject> Tiles;
        }

        [System.Serializable]
        public class OverLays
        {
            public DestructableTag Tag;
            public GameObject OverlayObject;
        }

        public List<TilesGroup> Tiles;
        public List<OverLays> OverlayReferences; 

        public GameObject GetRandomTileByTag(TileTag tag)
        {
            if (Tiles.Find(t => t.Tag == tag) == null || Tiles.Find(t => t.Tag == tag).Tiles.Count == 0)
            {
                Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return null;
            }

            return Tiles.Find(t => t.Tag == tag).Tiles.RandomItem();
        }

        public GameObject GetTile(TileTag tag)
        {
            if (Tiles.Find(t => t.Tag == tag) == null || Tiles.Find(t => t.Tag == tag).Tiles.Count == 0)
            {
                Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return null;
            }

            return Tiles.Find(t => t.Tag == tag).Tiles[0];
        }

        public GameObject GetOverlay(DestructableTag tag)
        {
            if(!OverlayReferences.Exists(o => o.Tag == tag))
            {
                return null;
            }

            return OverlayReferences.Find(o => o.Tag == tag).OverlayObject;
        }
    }
}
