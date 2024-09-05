using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        private WaitForSeconds _particleWait;
        public System.Action OnEnemyDeathEvent { get; set; }
        private Vector3 _respawnPoint;
        private WaitForSeconds _respawnDelay = new WaitForSeconds(1f);
        private List<EnemyType> _enemiesToSpawn = new List<EnemyType>();

        public void SetUp()
        {
            _particleWait = new WaitForSeconds(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant / 2);
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();
            SetUpPlayerSpawn();
        }

        #region Enemies
        public void SpawnEnemies(WaypointMarker marker)
        {
            // spawn enemies
            for (int i = 0; i < marker.EnemiesToSpawn.Count; i++)
            {
                SpawnEnemy(marker.EnemiesToSpawn[i]);
            }
        }

        public void SpawnEnemy(EnemySpawn spawn)
        {
            if(_enemiesToSpawn == null)
            {
                return;
            }

            StartCoroutine(SummoningProcess(spawn));
        }

        private IEnumerator SummoningProcess(EnemySpawn spawn)
        {
            GameManager.Instance.VisualEffectsManager.Misc.PlayEnemySpawnVFX(spawn.SpawnPoint.position);
            //GameManager.Instance.AudioManager.Play(_spawnAudio);

            yield return _particleWait;

            EnemyAIController spawnedEnemy = GameManager.Instance.EnemiesManager.SpawnEnemyAtPoint(spawn.Enemy, spawn.SpawnPoint.position);

            spawnedEnemy.Components.Health.OnDeath += OnEnemyDeath;
            spawnedEnemy.Activate();
            spawn.Modifiers.ForEach(m => spawnedEnemy.Components.DifficultyModifier.ApplyModifier(m));
        }

        private void OnEnemyDeath(TankComponents controller)
        {
            Debug.Log("Died");
            OnEnemyDeathEvent?.Invoke();
            controller.Health.OnDeath -= OnEnemyDeath;
        }

        public void DestroyEnemies()
        {
            GameManager.Instance.EnemiesManager.DestroyAllEnemies();
        }
        #endregion

        public void EnableInfiniteSuperAbility(bool enable)
        {
            if (enable)
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperRecharger.FullyChargeSuperAbility());
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperAbilities.EnableChargeConsumption(false));
            }
            else
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperAbilities.EnableChargeConsumption(true));
            }
        }

        #region Respawn Player
        private void SetUpPlayerSpawn()
        {
            GameManager.Instance.PlayersManager.GetPlayer(0).Health.OnDeath += RespawnPlayer;
            // remove displaying the gameover screen from the OnDeath subscribers
            GameManager.Instance.PlayersManager.GetPlayer(0).Health.OnDeath -= GameManager.Instance.PlayersManager.ReportPlayerDeath;
        }

        private void RespawnPlayer(TankComponents tank)
        {
            StartCoroutine(RespawnPlayerProcess());
        }

        private IEnumerator RespawnPlayerProcess()
        {
            yield return _respawnDelay;
            GameManager.Instance.PlayersManager.PlayerSpawner.RevivePlayer(0, _respawnPoint);
        }

        public void SetRespawnPoint(Transform respawnPoint)
        {
            _respawnPoint = respawnPoint.position;
        }
        #endregion
    }
}
