using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.ItemsSystem;
using TankLike.Combat.Destructible;
using TankLike.Environment.LevelGeneration;

namespace TankLike
{
    public class CollectableManager : MonoBehaviour, IManager
    {
        [SerializeField] private List<Collectable> _collectables;
        [SerializeField] private Vector2 _spreadRange;

        public bool IsActive { get; private set; }

        private Dictionary<CollectableType, Pool<Collectable>> _collectablesPools = new Dictionary<CollectableType, Pool<Collectable>>();

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            InitPools();
        }

        public void Dispose()
        {
            IsActive = false;

            DisposePools();
        }
        #endregion

        #region Pools
        private void InitPools()
        {
            foreach (var collectable in _collectables)
            {
                _collectablesPools.Add(collectable.Type, CreatePool(collectable));
            }
        }

        private void DisposePools()
        {
            foreach (KeyValuePair<CollectableType, Pool<Collectable>> collectable in _collectablesPools)
            {
                collectable.Value.Clear();
            }

            _collectablesPools.Clear();
        }

        private Pool<Collectable> CreatePool(Collectable collectable)
        {
            var pool = new Pool<Collectable>(
                () =>
                {
                    var obj = Instantiate(collectable);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj.GetComponent<Collectable>();
                },
                (Collectable obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (Collectable obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (Collectable obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }
        #endregion

        public void SpawnCollectablesOfType(List<DropChance> tags, Vector3 position, DestructableTag dropper)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            LevelDestructibleData_SO data = GameManager.Instance.DestructiblesManager.GetDestructibleDropData();
            DestructibleDrop drops = data.DropsData.Find(d => d.Tag == dropper);

            List<DropChance> selectedDrops = new List<DropChance>();

            // apply chances changes depending on the affectors
            ApplyChancesEffects(drops.Drops);
            drops.UpdateHighestChance();
            float chance = Random.value * drops.HighestChance;

            drops.Drops.FindAll(d => d.Chance > chance).ForEach(d => selectedDrops.Add(d));
            selectedDrops.ForEach(t => SpawnCollectable(position, t.DropType, t.CountRange.RandomValue()));

            // the count has to be controlled in the future
            //tags.ForEach(t => SpawnCollectable(position, t.DropTag, t.CountRange.RandomValue()));
        }

        private void ApplyChancesEffects(List<DropChance> drops)
        {
            for (int i = 0; i < drops.Count; i++)
            {
                DropChance drop = drops[i];

                if(drop.Affector == ChanceAffector.None)
                {
                    continue;
                }

                float effect = 1f;

                switch (drop.Affector)
                {
                    case ChanceAffector.PlayerHealth:
                        {
                            effect = drop.EffectRange.Lerp(GameManager.Instance.PlayersManager.GetHealth());
                            break;
                        }
                }

                //print($"value: {effect}. Health: {GameManager.Instance.PlayersManager.GetHealth()}. Effect range: {drop.EffectRange}");
                drops.FindAll(d => d != drop).ForEach(d => d.Chance *= effect);
            }
        }

        public void SpawnRandomCollectables(float chance, Vector3 position, CollectableType tag)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            var rand = Random.Range(0f, 1f);

            if (rand <= chance)
            {
                SpawnCollectable(position, tag, 1);
            }      
        }

        private void SpawnCollectable(Vector3 position, CollectableType drop, int count)
        {
            if (drop == CollectableType.Empty)
            {
                return;
            }

            Collectable collectable = _collectables.Find(c => c.Type == drop);

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = position + new Vector3(_spreadRange.RandomValue(), 0f, _spreadRange.RandomValue());
                var collect = _collectablesPools[drop].RequestObject(pos, Quaternion.identity);
                GameManager.Instance.SetParentToRoomSpawnables(collect.gameObject);
                collect.gameObject.SetActive(true);
            }
        }
    }

    public enum ChanceAffector
    {
        None = 0, PlayerAmmo = 1, PlayerHealth = 2
    }
}
