using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using Utils;

    public class GrassPainter : RoomPainter
    {
        [System.Serializable]
        public struct GrassColors
        {
            public Color FrontTopColor;
            public Color FrontBottomColor;
            public Color BackTopColor;
            public Color BackBottomColor;
        }

        [System.Serializable]
        public struct GrassTexture
        {
            public Texture2D Texture;
            public float Probability;
        }

        [SerializeField] private GrassProp _grassPrefab;
        [SerializeField] private Texture2D _noiseTexture;
        [SerializeField] private List<Texture2D> _grassTextures = new List<Texture2D>();
        [SerializeField] private List<GrassTexture> _grassTexturesChances = new List<GrassTexture>();
        [SerializeField] private List<GrassColors> _grassColors = new List<GrassColors>();

        public override void PaintRoom(MapTiles_SO map, Room room)
        {
            List<Tile> tiles = map.Tiles;
            List<Tile> groundTiles = GetTilesByRules(tiles, PaintingRules);
            groundTiles.RemoveAll(t => t.BuiltTile == null || t == null);

            if (groundTiles.Count == 0)
            {
                Debug.Log("No suitable tiles found for grass");
                return;
            }

            int grassCount = _levelData.GrassRangePerRoom.RandomValue();

            for (int i = 0; i < grassCount; i++)
            {
                Tile selectedTile = groundTiles.RandomItem();

                Vector3 offset;

                // ensure the grass spawns between tiles
                if (Random.value >= 0.5f)
                {
                    offset = new Vector3(Constants.TILE_SIZE / 2f * (Random.value >= 0.5f? 1f : -1f), 0f, Constants.TILE_SIZE.GetRandomValue() * 0.5f);
                }
                else
                {
                    offset = new Vector3(Constants.TILE_SIZE.GetRandomValue() * 0.5f, 0f, Constants.TILE_SIZE / 2f * (Random.value >= 0.5f ? 1f : -1f));
                }

                Vector3 position = selectedTile.BuiltTile.transform.position + offset;

                GrassProp grass = Instantiate(_grassPrefab, position, Quaternion.identity, room.Spawnables.SpawnablesParent);
                grass.transform.eulerAngles = new Vector3().Add(y: Random.Range(0, 360));


                Dictionary<System.Func<Texture2D>, float> textureChances = new Dictionary<System.Func<Texture2D>, float>();

                foreach (GrassTexture grassTexture in _grassTexturesChances)
                {
                    textureChances.Add(() => grassTexture.Texture, grassTexture.Probability);
                }

                Texture2D backwardSelectedTexture = ChanceSelector.SelectByChance(textureChances);
                Texture2D forwardSelectedTexture = ChanceSelector.SelectByChance(textureChances);

                grass.ApplyTexture(backwardSelectedTexture, forwardSelectedTexture);
                grass.ApplyColors(_grassColors.RandomItem());
            }
        }
    }
}
