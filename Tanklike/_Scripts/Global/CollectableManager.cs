using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using ItemsSystem;
    using Combat.Destructible;
    using Environment.LevelGeneration;

    public class CollectableManager : MonoBehaviour, IManager
    {
        [SerializeField] private List<Collectable> _collectables;
        [SerializeField] private Vector2 _spreadRange;
        [SerializeField] private CollectablesDropSettings _defaultCollectableSettings;

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

        public void SpawnCollectablesOfType(CollectablesDropRequest requestData)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            DestructibleDrop dropsToUse;

            if (requestData.DropPassedTags && requestData.Drops != null)
            {
                dropsToUse = requestData.Drops;
            }
            else
            {
                LevelDestructibleData_SO data = GameManager.Instance.DestructiblesManager.GetDestructibleDropData();
                dropsToUse = data.DropsData.Find(d => d.Tag == requestData.DropperTag);
            }

            List<DropChance> selectedDrops;
            selectedDrops = new List<DropChance>();

            // apply chances changes depending on the affectors
            ApplyChancesEffects(dropsToUse.Drops);
            dropsToUse.UpdateHighestChance();
            float rand = Random.value * dropsToUse.HighestChance;

            dropsToUse.Drops.FindAll(d => d.CurrentChance > rand).ForEach(d => selectedDrops.Add(d));

            for (int i = 0; i < selectedDrops.Count; i++)
            {
                DropChance drop = selectedDrops[i];

                SpawnCollectable(requestData.Position, drop.DropType, drop.CountRange.RandomValue(), requestData.Settings);
            }
        }

        /// <summary>
        /// Applies affectors to the chances, which influences the weights of the drops depending on the player's stats.
        /// </summary>
        /// <param name="drops"></param>
        private void ApplyChancesEffects(List<DropChance> drops)
        {
            for (int i = 0; i < drops.Count; i++)
            {
                DropChance drop = drops[i];

                if (drop.Affector.AffectorType == ChanceAffector.None)
                {
                    continue;
                }

                float effect = 1f;

                switch (drop.Affector.AffectorType)
                {
                    case ChanceAffector.PlayerHealth:
                        {
                            effect = drop.Affector.EffectRange.Lerp(GameManager.Instance.PlayersManager.GetPlayersTotalHealth01());
                            break;
                        }
                }

                drops.FindAll(d => d != drop).ForEach(d => d.CurrentChance *= effect);
            }
        }

        private void SpawnCollectable(Vector3 position, CollectableType drop, int count, CollectablesDropSettings settings = null)
        {
            if (drop == CollectableType.Empty)
            {
                return;
            }

            if (settings == null)
            {
                settings = _defaultCollectableSettings;
            }

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = position + new Vector3(_spreadRange.RandomValue(), 0f, _spreadRange.RandomValue());
                var collectable = _collectablesPools[drop].RequestObject(pos, Quaternion.identity);
                GameManager.Instance.SetParentToRoomSpawnables(collectable.gameObject);
                collectable.gameObject.SetActive(true);

                if (!settings.HasDeathCountDown)
                {
                    collectable.ActivateSelfDestructTimer(false);
                }

                collectable.StartCollectable();
            }
        }

        /// <summary>
        /// Spawns a collectable with a chance.
        /// </summary>
        /// <param name="chance"></param>
        /// <param name="position"></param>
        /// <param name="tag"></param>
        public void SpawnRandomCollectables(float chance, Vector3 position, CollectableType tag)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            float rand = Random.Range(0f, 1f);

            if (rand <= chance)
            {
                SpawnCollectable(position, tag, 1);
            }
        }
    }

    public enum ChanceAffector
    {
        None = 0, PlayerAmmo = 1, PlayerHealth = 2
    }

    public class CollectablesDropRequest
    {
        public DestructibleDrop Drops;
        public Vector3 Position;
        public DropperTag DropperTag;
        public CollectablesDropSettings Settings;
        public bool DropPassedTags = false;
    }
}
