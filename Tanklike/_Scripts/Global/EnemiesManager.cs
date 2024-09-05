using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Utils;
using TankLike.Environment;
using System.Linq;

namespace TankLike
{
    public class EnemiesManager : MonoBehaviour, IManager
    {
        [Header("Settings")]
        [SerializeField] private Vector2Int _wavesRange;
        [SerializeField] private bool _spawnEnemies = true;
        private Dictionary<EnemyType, Pool<EnemyAIController>> _enemiesPools = new Dictionary<EnemyType, Pool<EnemyAIController>>();
        [SerializeField] private List<Transform> _spawnedEnemies = new List<Transform>();
        [SerializeField] private WaveData[] _tutorialWaves;
        [SerializeField] private List<WaveData> _wavesToSpawn = new List<WaveData>();
        [SerializeField] private Vector2Int _roomsWithEnemiesNumberRange;
        private Vector2Int _wavesCapacityRange;
        [SerializeField] private AnimationCurve _progressionCurve;
        [SerializeField] private int _currentMinProgression = 0;
        [SerializeField] private int _currentWaveIndex = 0;

        [field: SerializeField, Range(0f, 1f)] public float Difficulty { get; private set; }
        public bool IsActive { get; private set; }

        private EnemiesDatabase _enemiesDatabase;
        [SerializeField] private List<WaveData> _waves;

        private Dictionary<EnemyType, Pool<UnitParts>> _enemyPartsPools = new Dictionary<EnemyType, Pool<UnitParts>>();

        private bool _fightActivated;

        public void SetReferences(EnemiesDatabase enemiesDatabase)
        {
            _enemiesDatabase = enemiesDatabase;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _waves = GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().Waves;
            _wavesCapacityRange = new Vector2Int(_waves.OrderBy(w => w.Capacity).First().Capacity,
                _waves.OrderByDescending(w => w.Capacity).First().Capacity);

            InitPools();

            if (!_spawnEnemies)
            {
                return;
            }

            CreateWaves();
        }

        public void Dispose()
        {
            IsActive = false;

            DisposePools();
        }
        #endregion

        public void CreateWaves()
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
            // set the game difficulty
            SetDifficulty(GameManager.Instance.PlayersTempInfoSaver.GameDifficulty);
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

        private void DisposePools()
        {
            foreach (KeyValuePair<EnemyType, Pool<EnemyAIController>> enemy in _enemiesPools)
            {
                enemy.Value.Clear();
            }

            foreach (KeyValuePair<EnemyType, Pool<UnitParts>> enemyParts in _enemyPartsPools)
            {     
                 enemyParts.Value.Clear();
            }

            _enemiesPools.Clear();
            _enemyPartsPools.Clear();
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

        private Pool<UnitParts> CreateEnemyPartsPool(EnemyData enemyData)
        {
            var pool = new Pool<UnitParts>(
                () =>
                {
                    var obj = Instantiate(enemyData.PartsPrefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj.GetComponent<UnitParts>();
                },
                (UnitParts obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (UnitParts obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (UnitParts obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }
        #endregion

        public UnitParts GetEnemyPartsByType(EnemyType type)
        {
            UnitParts parts = _enemyPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }

        public EnemyAIController SpawnEnemyAtPoint(EnemyType type, Vector3 point)
        {
            EnemyAIController enemy = _enemiesPools[type].RequestObject(Vector3.zero, Quaternion.identity);

            enemy.transform.position = point;

            enemy.gameObject.SetActive(true);

            _spawnedEnemies.Add(enemy.transform);

            return enemy;
        }

        public void AddEnemy(TankComponents enemy)
        {
            _spawnedEnemies.Add(enemy.transform);
        }

        public void RemoveEnemy(TankComponents enemy)
        {
            _spawnedEnemies.Remove(enemy.transform);
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
                _spawnedEnemies[i].GetComponent<TankComponents>().Health.Die();
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

        public void SetFightActivated(bool value)
        {
            _fightActivated = value;
        }

        public bool IsFightActivated()
        {
            return _fightActivated;
        }
    }
}
