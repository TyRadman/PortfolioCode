using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Environment;
using UnityEngine;
using TankLike.Utils;
using System.Linq;
using static TankLike.Environment.RoomSpawnPoints;
using TankLike.Sound;

namespace TankLike
{
    public class PlayerSpawner : MonoBehaviour, IManager
    {
        [SerializeField] private Audio _spawnAudio;

        private Vector3 _lastPosition;
        private WaitForSeconds _initialWait = new WaitForSeconds(SPAWN_INITIAL_DELAY);
        private PlayersDatabase _playersDatabase;

        private const float SPAWN_INITIAL_DELAY = 0.5f;

        public bool IsActive { get; private set; }

        public void SetReferences(PlayersDatabase playersDatabase)
        {
            _playersDatabase = playersDatabase;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }
        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void SpawnPlayers()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            int playersCount = GameManager.Instance.PlayersTempInfoSaver.PlayersCount;
            List<SpawnPoint> spawnPoints = GameManager.Instance.RoomsManager.CurrentRoom.Spawner.SpawnPoints.Points;
            _lastPosition = GameManager.Instance.RoomsManager.CurrentRoom.transform.position;

            for (int i = 0; i < playersCount; i++)
            {
                // get the closest not-taken spawn points to the last registered position
                SpawnPoint selectedSpawnPoint = spawnPoints.FindAll(s => !s.Taken).OrderBy(s =>
                (s.Point.position - _lastPosition).sqrMagnitude).FirstOrDefault();
                selectedSpawnPoint.Taken = true;
                SpawnPlayer(i, selectedSpawnPoint.Point.position);
            }

            if(playersCount == 2)
            {
                GameManager.Instance.PlayersManager.OnTwoPlayersMode();
            }
        }

        private void SpawnPlayer(int playerIndex, Vector3 spawnPosition)
        {
            StartCoroutine(SpawnProcess(playerIndex, spawnPosition));
        }

        private IEnumerator SpawnProcess(int playerIndex, Vector3 respawnPosition)
        {
            _lastPosition = respawnPosition;
            respawnPosition.y += 2f;
            PlayerData selectedCharacter = _playersDatabase.GetPlayerDataByType(PlayerType.PlayerOne); // TODO: Get selected character
            GameObject player = Instantiate(selectedCharacter.Prefab, respawnPosition, Quaternion.identity);

            PlayerComponents components = player.GetComponent<PlayerComponents>();
            components.PlayerIndex = playerIndex;
            components.SetUp();
            GameManager.Instance.PlayersManager.AddPlayer(components);
            player.gameObject.SetActive(false);

            yield return _initialWait;

            float effectDuration = GameManager.Instance.VisualEffectsManager.Misc.PlayPlayerSpawnVFX(respawnPosition);
            GameManager.Instance.AudioManager.Play(_spawnAudio);
            yield return new WaitForSeconds(effectDuration);

            player.gameObject.SetActive(true);
            components.Activate();

            GameManager.Instance.EffectsUIController.ShowLevelName(); // TODO: find the right place to call this
        }

        public void RevivePlayer(int playerIndex, Vector3 respawnPosition)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(RevivalProcess(playerIndex, respawnPosition));
        }

        private IEnumerator RevivalProcess(int playerIndex, Vector3 respawnPosition)
        {
            respawnPosition.y += 2f;

            //PlaySpawnEffects(out float spawnEffectDuration, respawnPosition);
            float effectDuration = GameManager.Instance.VisualEffectsManager.Misc.PlayPlayerSpawnVFX(respawnPosition);
            GameManager.Instance.AudioManager.Play(_spawnAudio);
            yield return new WaitForSeconds(effectDuration);

            PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(playerIndex);

            player.gameObject.SetActive(true);
            player.OnRevived();
            player.transform.position = respawnPosition;

            GameManager.Instance.PlayersManager.AddPlayerTransform(player);

            // make the camera follow the newly added player too
            GameManager.Instance.CameraManager.PlayerCameraFollow.AddCameraFollower(playerIndex);
            GameManager.Instance.OffScreenIndicator.EnableOffScreenIndicatorForPlayer(playerIndex, true);
        }

        private void PlaySpawnEffects(out float effectDuration, Vector3 position)
        {
            var vfx = GameManager.Instance.VisualEffectsManager.Misc.PlayerSpawning;
            vfx.transform.SetPositionAndRotation(position, Quaternion.identity);
            vfx.gameObject.SetActive(true);
            vfx.Play();

            // play audio
            GameManager.Instance.AudioManager.Play(_spawnAudio);

            effectDuration = vfx.Particles.main.startLifetime.constant / 2;
        }
    }
}
