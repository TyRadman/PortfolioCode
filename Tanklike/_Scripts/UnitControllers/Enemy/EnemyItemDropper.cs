using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyItemDropper : MonoBehaviour
    {
        // to be set from the enemy data
        private float _dropChance = 0.5f;
        [SerializeField] private List<CollectableTag> _droppedCollectables;
        private List<CollectableTag> _collectablesToDrop;

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
            _collectablesToDrop = new List<CollectableTag>();
            _collectablesToDrop.Add(CollectableTag.BossKey);
            _dropChance = 1f;
        }

        public void Dispose()
        {
            _collectablesToDrop.Clear();
        }
    }
}
