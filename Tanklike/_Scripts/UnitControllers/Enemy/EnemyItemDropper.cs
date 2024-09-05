using System.Collections;
using System.Collections.Generic;
using TankLike.ItemsSystem;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyItemDropper : MonoBehaviour
    {
        // to be set from the enemy data
        private float _dropChance = 0.5f;
        [SerializeField] private List<CollectableType> _droppedCollectables;
        private List<CollectableType> _collectablesToDrop;

        private void OnEnable()
        {
            _collectablesToDrop = _droppedCollectables;
        }

        public void DropItem()
        {
            GameManager.Instance.CollectableManager.SpawnRandomCollectables(_dropChance, transform.position, _collectablesToDrop.RandomItem()); 
        }

        public void SetAsKeyHolder()
        {
            _collectablesToDrop = new List<CollectableType>();
            _collectablesToDrop.Add(CollectableType.BossKey);
            _dropChance = 1f;
        }

        public void Dispose()
        {
            _collectablesToDrop.Clear();
        }
    }
}
