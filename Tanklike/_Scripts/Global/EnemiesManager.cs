using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Utils;
using TankLike.Environment;
using System.Linq;

namespace TankLike
{
    public class EnemiesManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2Int _wavesRange;
        [SerializeField] private bool _spawnEnemies = true;
        private Dictionary<EnemyType, Pool<EnemyAIController>> _enemiesPools = new Dictionary<EnemyType, Pool<EnemyAIController>>();
        [SerializeField] private List<EnemyAIController> _spawnedEnemies = new List<EnemyAIController>();
        [SerializeField] private WaveData[] _tutorialWaves;
        [SerializeField] private List<WaveData> _wavesToSpawn = new List<WaveData>();
        [SerializeField] private Vector2Int _roomsWithEnemiesNumberRange;
        private Vector2Int _wavesCapacityRange;
        [SerializeField] private AnimationCurve _progressionCurve;
        [SerializeField] private int _currentMinProgression = 0;
        [SerializeField] private int _currentWaveIndex = 0;
        [field: SerializeField, Range(0f, 1f)] public float Difficulty { get; private set; }

        private EnemiesDatabase _enemiesDatabase;
        [SerializeField] private List<WaveData> _waves;

        private Dictionary<EnemyType, Pool<EnemyParts>> _enemyPartsPools = new Dictionary<EnemyType, Pool<EnemyParts>>();

        public void SetUp(EnemiesDatabase enemiesDatabase)
        {
            _enemiesDatabase = enemiesDatabase;

            if (!_spawnEnemies)
            {
                return;
            }

            _waves = GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().Waves;
            _wavesCapacityRange = new Vector2Int(_waves.OrderBy(w => w.Capacity).First().Capacity,
                _waves.OrderByDescending(w => w.Capacity).First().Capacity);
            InitPools();
            CreateWaves();
        }

        private void CreateWaves()
        {
            int numberOfWaves = GameManager.Instance.RoomsManager.Rooms.Count(r => r.RoomType == RoomType.Normal);

            if(numberOfWaves < 3)
            {
                Debug.LogError("Number of rooms that can hold keys is less than the number of keys");
                return;
            }

            for (int i = 0; i < _tutorialWaves.Length; i++)
            {
                WaveData wave = Instantiate(_tutorialWaves[i]);
                _wavesToSpawn.Add(wave);
            }

            for (int i = _tutorialWaves.Length; i < numberOfWaves + 1; i++)
            {
                _currentMinProgression = (int)Mathf.Lerp(_wavesCapacityRange.x, _wavesCapacityRange.y, _progressionCurve.Evaluate((float)i / (float)numberOfWaves));
                int capacityCount = _waves.FindAll(w => w.Capacity >= _currentMinProgression).OrderByDescending(w => w.Capacity).Last().Capacity;
                WaveData waveToCreate = _waves.FindAll(w => w.Capacity == capacityCount).RandomItem();

                if (waveToCreate == null)
                {
                    waveToCreate = _waves.RandomItem();
                }

                WaveData wave = Instantiate(waveToCreate);
                _wavesToSpawn.Add(wave);
            }
            
            // ones the waves (as data, not as spawned enemies) is created, add the key holders to it
            GameManager.Instance.BossKeysManager.DistributeKeysAcrossRemainingRooms(_wavesToSpawn);
        }

        /// <summary>
        /// Called everytime the played enters a room that has enemies to spawn. Because we decide what enemies to spawn after the player enters a room to control progression.
        /// </summary>
        /// <param name="room"></param>
        public void GetEnemies(NormalRoom room)
        {
            if (!_spawnEnemies)
            {
                return;
            }

            WaveData waveToCreate = _wavesToSpawn[_currentWaveIndex++];
            List<EnemyType> waveEnemies = waveToCreate.Enemies;

            // create an empty wave and an empty list for enemies
            EnemyWave wave = new EnemyWave();

            for (int j = 0; j < waveEnemies.Count; j++)
            {
                EnemyData selectedEnemyData = null;
                selectedEnemyData = _enemiesDatabase.GetAllEnemies().Find(e => e.EnemyType == waveEnemies[j]);

                if (selectedEnemyData == null)
                {
                    Debug.LogError($"No enemy data of type {waveEnemies[j]}");
                    break;
                }

                EnemySpawnProfile enemyProfile = new EnemySpawnProfile(selectedEnemyData.EnemyType);
                wave.Enemies.Add(enemyProfile);
            }

            wave.HasKey = waveToCreate.HasKey;
            room.Spawner.SetRoomEnemyWaves(new List<EnemyWave>() { wave});
            room.SpawnedEnemies = true;
        }

        #region Pools
        private void InitPools()
        {
            foreach (var enemy in _enemiesDatabase.GetAllEnemies())
            {
                _enemiesPools.Add(enemy.EnemyType, CreateEnemyPool(enemy));
            }

            foreach (var enemy in _enemiesDatabase.GetAllEnemies())
            {
                if (enemy.PartsPrefab == null) continue;
                _enemyPartsPools.Add(enemy.EnemyType, CreateEnemyPartsPool(enemy));
            }
        }

        private Pool<EnemyAIController> CreateEnemyPool(EnemyData enemyData)
        {
            var pool = new Pool<EnemyAIController>(
                () =>
                {
                    var obj = Instantiate(enemyData.Prefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj.GetComponent<EnemyAIController>();
                },
                (EnemyAIController obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (EnemyAIController obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (EnemyAIController obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }

        private Pool<EnemyParts> CreateEnemyPartsPool(EnemyData enemyData)
        {
            var pool = new Pool<EnemyParts>(
                () =>
                {
                    var obj = Instantiate(enemyData.PartsPrefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj.GetComponent<EnemyParts>();
                },
                (EnemyParts obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (EnemyParts obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (EnemyParts obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }
        #endregion

        public EnemyParts GetEnemyPartsByType(EnemyType type)
        {
            EnemyParts parts = _enemyPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }

        public EnemyAIController SpawnEnemyAtPoint(EnemyType type, Vector3 point)
        {
            EnemyData data = null;

            foreach (var stat in _enemiesDatabase.GetAllEnemies())
            {
                if(stat.EnemyType == type)
                {
                    data = stat;
                    break;
                }
            }

            Vector3 pos = point;
            pos.y = data.SpawnYOffset;
            EnemyAIController enemy = _enemiesPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            enemy.transform.position = pos;
            enemy.gameObject.SetActive(true);

            _spawnedEnemies.Add(enemy);

            return enemy;
        }

        public void RemoveEnemy(EnemyAIController enemy)
        {
            _spawnedEnemies.Remove(enemy);
        }

        public List<Transform> GetSpawnedEnemies()
        {
            List<Transform> enemies = new List<Transform>();
            _spawnedEnemies.ForEach(e => enemies.Add(e.transform));
            return enemies;
        }

        public void DestroyAllEnemies()
        {
            for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
            {
                ((EnemyHealth)_spawnedEnemies[i].Components.Health).Die();
            }
        }

        public List<WaveData> GetWavesToSpawn()
        {
            return _wavesToSpawn;
        }

        public void SetDifficulty(float difficulty)
        {
            Difficulty = difficulty;
        }

        public void EnableSpawnEnemies(bool enable)
        {
            _spawnEnemies = enable;
        }

        public bool SpawnEnemies()
        {
            return _spawnEnemies;
        }
    }
}
