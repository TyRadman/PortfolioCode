using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.ItemsSystem;
using TankLike.Combat.Destructible;
using TankLike.Environment.LevelGeneration;

namespace TankLike
{
    public class CollectableManager : MonoBehaviour
    {
        [SerializeField] private List<Collectable> _collectables;
        [SerializeField] private Vector2 _spreadRange;

        private Dictionary<CollectableTag, Pool<Collectable>> _collectablesPools = new Dictionary<CollectableTag, Pool<Collectable>>();

        public void SetUp()
        {
            InitPools();
        }

        private void InitPools()
        {
            foreach (var collectable in _collectables)
            {
                _collectablesPools.Add(collectable.Tag, CreatePool(collectable));
            }
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

        public void SpawnCollectablesOfType(List<DropChance> tags, Vector3 position, DestructableTag dropper)
        {
            LevelDestructibleData_SO data = GameManager.Instance.DestructiblesManager.GetDestructibleDropData();
            DestructibleDrop drops = data.DropsData.Find(d => d.Tag == dropper);

            List<DropChance> selectedDrops = new List<DropChance>();

            // apply chances changes depending on the affectors
            ApplyChancesEffects(drops.Drops);
            drops.UpdateHighestChance();
            float chance = Random.value * drops.HighestChance;

            drops.Drops.FindAll(d => d.Chance > chance).ForEach(d => selectedDrops.Add(d));
            selectedDrops.ForEach(t => SpawnCollectable(position, t.DropTag, t.CountRange.RandomValue()));

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

        public void SpawnRandomCollectables(float chance, Vector3 position, CollectableTag tag)
        {
            var rand = Random.Range(0f, 1f);

            if (rand <= chance)
            {
                SpawnCollectable(position, tag, 1);
            }      
        }

        private void SpawnCollectable(Vector3 position, CollectableTag drop, int count)
        {
            if (drop == CollectableTag.Empty)
            {
                return;
            }

            Collectable collectable = _collectables.Find(c => c.Tag == drop);

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = position + new Vector3(_spreadRange.RandomValue(), 0f, _spreadRange.RandomValue());
                var collect = _collectablesPools[drop].RequestObject(pos, Quaternion.identity);
                GameManager.Instance.SetParentToRoomSpawnables(collect.gameObject);
                collect.gameObject.SetActive(true);
            }
        }
    }

    public enum CollectableTag
    {
        Health = 0, Coins = 1, Gem = 2, Ammo = 3, BossKey = 4, Empty = 5
    }

    public enum ChanceAffector
    {
        None = 0, PlayerAmmo = 1, PlayerHealth = 2
    }
}
