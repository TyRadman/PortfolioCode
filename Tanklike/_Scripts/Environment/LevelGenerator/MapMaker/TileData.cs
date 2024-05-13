using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    [System.Serializable]
    public class TileData : TileParent
    {
        public enum GatePartType
        {
            None = 0, PartOfGate = 1, DoublePartOfGate = 4
        }

        [field: SerializeField] public GatePartType GatePart { set; get; } = GatePartType.None;
        [SerializeField] protected BoxCollider _collider;
        [HideInInspector] public TilesArranger Arranger;
        [HideInInspector] public TileData Parent;
        [HideInInspector] public Vector2Int Dimension;
        [field: SerializeField] public GameObject TileObject { get; private set; }

        public void SetTileObject(GameObject tile, TileTag tileTag)
        {
            TileObject = tile;
            _collider = tile.GetComponent<BoxCollider>();
            Tag = tileTag;
        }

        public void EnableBoxCollider(bool enable)
        {
            _collider.enabled = enable;
        }

        public void SetGatePart(GatePartType gatePart)
        {
            GatePart = gatePart;
        }

        public void SetName(int x, int y)
        {
            TileObject.name = $"{x},{y}";
            Dimension = new Vector2Int(x, y);
        }

        public void SetArranger(TilesArranger arranger)
        {
            Arranger = arranger;
        }
    }
}
