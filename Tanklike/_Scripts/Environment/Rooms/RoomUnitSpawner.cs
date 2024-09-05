using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Environment
{
    public class RoomUnitSpawner : MonoBehaviour
    {
        public List<EnemyAIController> CurrentlySpawnedEnemies { get; private set; }
        [SerializeField] private Vector2 _spawningDelaysRange;
        private int _currentWaveIndex = 0;
        [field: SerializeField] public RoomSpawnPoints SpawnPoints { get; private set; }
        [SerializeField] private List<EnemyWave> _enemyWaves = new List<EnemyWave>();
        [SerializeField] private Room _room;
        [SerializeField] private Audio _spawnAudio;
        private WaitForSeconds _particleWait;

        private void Start()
        {
            if(GameManager.Instance == null)
            {
                return;
            }

            Invoke(nameof(SetParticleWait), 0.5f);
        }

        // TODO: Roomts shouldn't set themselves up on start, otherwise, things will break when we have a room already spawned
        private void SetParticleWait()
        {
            _particleWait = new WaitForSeconds(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant / 2);

        }

        public void ActivateSpawnedEnemies()
        {
            if (_enemyWaves.Count <= 0)
            {
                return;
            }

            SpawnWave(_enemyWaves[_currentWaveIndex]);
        }

        private void SpawnWave(EnemyWave wave)
        {
            // reset the list to start counting down the enemies again to know when to trigger the next wave or stop the waves if this is the last wave
            CurrentlySpawnedEnemies = new List<EnemyAIController>();

            foreach (var enemy in wave.Enemies)
            {
                StartCoroutine(SpawnEnemy(enemy));
            }
        }

        public void OnEnemyDeathHandler(EnemyAIController enemy)
        {
            // remove the enemy if it's a part of the currently spawned enemies
            if (CurrentlySpawnedEnemies.Contains(enemy))
            {
                CurrentlySpawnedEnemies.Remove(enemy);
                enemy.OnEnemyDeath -= OnEnemyDeathHandler;
            }

            // if there are no enemies left, check for the next wave or open the room
            if (CurrentlySpawnedEnemies.Count <= 0)
            {
                CheckForNewWave();
            }
        }

        private IEnumerator SpawnEnemy(EnemySpawnProfile enemy)
        {
            // perform slight delay before the enemy spawns (for variety)
            yield return new WaitForSeconds(_spawningDelaysRange.RandomValue());
            Vector3 pos = SpawnPoints.GetRandomSpawnPoint().position;

            // play the effect (Should be called from the pooling system later)
            GameManager.Instance.VisualEffectsManager.Misc.PlayEnemySpawnVFX(pos);
            GameManager.Instance.AudioManager.Play(_spawnAudio);
            yield return _particleWait;

            EnemyAIController spawnedEnemy = GameManager.Instance.EnemiesManager.SpawnEnemyAtPoint(enemy.Enemy, pos);

            spawnedEnemy.OnEnemyDeath += OnEnemyDeathHandler;

            // add the enemy to the currently spawned enemies list
            CurrentlySpawnedEnemies.Add(spawnedEnemy);
            spawnedEnemy.Activate();

            // assign this enemy as a key holder if it was chosen to be one
            if (enemy.HasKey)
            {
                spawnedEnemy.Components.ItemDrop.SetAsKeyHolder();
            }
        }

        public void CheckForNewWave()
        {
            // check if there are any waves left
            if (_currentWaveIndex < _enemyWaves.Count - 1)
            {
                _currentWaveIndex++;
                SpawnWave(_enemyWaves[_currentWaveIndex]);
                // set all the points to not taken
                SpawnPoints.SetAllPointsAsNotTaken();
            }
            else
            {
                _room.GatesInfo.Gates.ForEach(g => g.Gate.OpenGate());
                _room.PlayOpenGateAudio();
                GameManager.Instance.CameraManager.Zoom.SetToNormalZoom();
                GameManager.Instance.EnemiesManager.SetFightActivated(false);
            }
        }

        public void SetRoomEnemyWaves(List<EnemyWave> waves)
        {
            _enemyWaves = waves;
        }

        public void AddKeyHolder()
        {
            // cache all the waves that don't have an enemy holding a key and have at least one enemy that can hold keys
            List<EnemyWave> noKeyWaves = _enemyWaves.FindAll(w => /*w.Enemies.TrueForAll(e => !e.HasKey) &&*/ w.Enemies.Exists(e => e.CanHaveKey));

            if (noKeyWaves.Count > 0)
            {
                // select a random enemy in a random available wave to hold the key
                noKeyWaves.RandomItem().Enemies.FindAll(e => e.CanHaveKey).RandomItem().HasKey = true;
            }
            else
            {
                // if all the waves are have an enemy holding a key in them, then selected an enemy that doesn't hold a key at least
                _enemyWaves.RandomItem().Enemies.Find(e => !e.HasKey).HasKey = true;
            }
        }

        public bool CanHaveKeys()
        {
            return _enemyWaves.Exists(w => w.HasKey);
        }

        public bool HasEnemies()
        {
            return _enemyWaves.Count > 0;
        }
    }
}
