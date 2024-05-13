using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.UI.Inventory;
using TankLike.UI.Notifications;

namespace TankLike
{
    /// <summary>
    /// This script holds all the information about the players, like their references for the enemies to access, their selected characters, upgrades and anything that other classes might need from the players.
    /// </summary>
    public class PlayersManager : MonoBehaviour
    {
        public class PlayerTransforms
        {
            public Transform PlayerTransform;
            public Transform ImageTransform;
            public int PlayerIndex;
        }

        public System.Action<PlayerComponents> OnPlayerSpawned { get; set; }
        private GameplaySettings _defaultSettings;
        [SerializeField] private List<GameplaySettings> _playersGameplaySettings;
        private List<PlayerComponents> _players = new List<PlayerComponents>();
        private List<PlayerTransforms> _playersTransforms = new List<PlayerTransforms>();
        public static int PlayerLayer = 11;
        public LayerMask PlayerLayerMask;
        public static int PlayersCount { get; private set; }
        public static int ActivePlayersCount { get; private set; }
        [field: SerializeField] public int CoinsAmount { get; private set; }
        [SerializeField] private Color[] _playerColors;

        public void AddPlayer(PlayerComponents player)
        {
            player.IsAlive = true;
            _players.Add(player);
            AddPlayerTransform(player);
            ActivePlayersCount = _playersTransforms.Count;
            OnPlayerSpawned?.Invoke(player);
            player.SetUpSettings();
            PlayersCount = _players.Count;
            SetColors();
        }

        private void AddPlayerTransform(PlayerComponents player)
        {
            _playersTransforms.Add(new PlayerTransforms
            {
                PlayerTransform = player.transform,
                ImageTransform = player.PredictedPosition.GetImage(),
                PlayerIndex = player.PlayerIndex
            });
        }

        public List<PlayerTransforms> GetPlayerTransforms()
        {
            return _playersTransforms;
        }

        public Transform GetClosestPlayerTransform(Vector3 startPoint)
        {
            if(ActivePlayersCount == 0)
            {
                return _players[0].transform;
            }

            if (ActivePlayersCount == 1)
            {
                return _playersTransforms[0].PlayerTransform;
            }

            float distanceToPlayerOne = (_playersTransforms[0].PlayerTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].PlayerTransform.position - startPoint).sqrMagnitude;

            if(distanceToPlayerOne < distanceToPlayerTwo)
            {
                return _playersTransforms[0].PlayerTransform;
            }
            else
            {
                return _playersTransforms[1].PlayerTransform;
            }
        }

        public Transform GetClosestPlayerImage(Vector3 startPoint)
        {
            if (ActivePlayersCount == 1)
            {
                return _playersTransforms[0].ImageTransform;
            }

            float distanceToPlayerOne = (_playersTransforms[0].ImageTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].ImageTransform.position - startPoint).sqrMagnitude;

            if (distanceToPlayerOne < distanceToPlayerTwo)
            {
                return _playersTransforms[0].ImageTransform;
            }
            else
            {
                return _playersTransforms[1].ImageTransform;
            }
        }

        public PlayerTransforms GetClosestPlayer(Vector3 startPoint)
        {
            if (ActivePlayersCount == 1)
            {
                return _playersTransforms[0];
            }

            float distanceToPlayerOne = (_playersTransforms[0].ImageTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].ImageTransform.position - startPoint).sqrMagnitude;

            if (distanceToPlayerOne < distanceToPlayerTwo)
            {
                return _playersTransforms[0];
            }
            else
            {
                return _playersTransforms[1];
            }
        }

        public List<PlayerComponents> GetPlayerProfiles()
        {
            return _players;
        }

        public PlayerComponents GetPlayer(int index)
        {
            return _players[index];
        }

        public int GetPlayersCount()
        {
            return _players.Count;
        }

        public void SetAimSensitivity(float amount, int playerIndex)
        {
            _playersGameplaySettings[playerIndex].AimSensitivity = amount;
            _players[playerIndex].Crosshair.SetAimSensitivity(amount);
        }

        public GameplaySettings GetGameplaySettings(int playerIndex)
        {
            return _playersGameplaySettings[playerIndex];
        }

        public void EnablePauseInputForSecondPlayer(int playerIndex, bool enable)
        {
            if (PlayersCount == 1)
            {
                return;
            }

            int secondPlayerIndex = (playerIndex + 1) % 2;

            if (enable)
            {
                _players[secondPlayerIndex].UIController.PauseMenuController.SetUpInput(secondPlayerIndex);
            }
            else
            {
                _players[secondPlayerIndex].UIController.PauseMenuController.DisposeInput(secondPlayerIndex);
            }
        }

        public void ReportPlayerDeath(int playerIndex)
        {
            _players[playerIndex].IsAlive = false;
            GameManager.Instance.CameraManager.PlayerCameraFollow.RemoveCameraFollow(playerIndex);
            _playersTransforms.Remove(_playersTransforms.Find(t => t.PlayerIndex == _players[playerIndex].PlayerIndex));
            ActivePlayersCount = _playersTransforms.Count;
            GameManager.Instance.OffScreenIndicator.EnableOffScreenIndicatorForPlayer(playerIndex, false);
            bool allPlayersDead = _players.TrueForAll(p => !p.IsAlive);

            if (allPlayersDead)
            {
                GameManager.Instance.OnGameOver();
            }
        }

        public int GetInactivePlayerIndex()
        {
            if(ActivePlayersCount == PlayersCount)
            {
                return -1;
            }

            return _players.Find(p => !p.IsAlive).PlayerIndex;
        }

        public void RevivePlayer(int playerIndex, Vector3 respawnPosition)
        {
            StartCoroutine(RevivalProcess(playerIndex, respawnPosition));
        }

        private IEnumerator RevivalProcess(int playerIndex, Vector3 respawnPosition)
        {
            var vfx = GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning;
            vfx.transform.SetPositionAndRotation(respawnPosition, Quaternion.identity);
            vfx.gameObject.SetActive(true);
            vfx.Play();

            // play audio
            //GameManager.Instance.AudioManager.Play(_spawnAudio);
            WaitForSeconds wait = new WaitForSeconds(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant / 2);
            yield return wait;

            _players[playerIndex].gameObject.SetActive(true);
            _players[playerIndex].transform.position = respawnPosition;

            AddPlayerTransform(_players[playerIndex]);
            ActivePlayersCount = _playersTransforms.Count;
            _players[playerIndex].OnRevived();

            // make the camera follow the newly added player too
            GameManager.Instance.CameraManager.PlayerCameraFollow.AddCameraFollower(playerIndex);
            GameManager.Instance.OffScreenIndicator.EnableOffScreenIndicatorForPlayer(playerIndex, true);
        }

        public void AddCoins(int amount, int playerIndex = 0)
        {
            CoinsAmount += amount;
            GameManager.Instance.NotificationsManager.PushCollectionNotification(NotificationType.Coins, amount, playerIndex);
        }

        private void SetColors()
        {
            int index = ActivePlayersCount - 1;
            Color color = _playerColors[index];
            _players[index].Crosshair.SetColor(color);
        }

        public Color GetPlayerColor(int playerIndex)
        {
            return _playerColors[playerIndex];
        }

        public float GetHealth()
        {
            float health = 0f;
            _players.ForEach(p => health += p.Health.GetHealth());
            return health;
        }

        /// <summary>
        /// Returns the the players' position. If it's a single player, then only the position of that player is returned, otherwise, the average of the position of both players is returned.
        /// </summary>
        public Vector3 GetPlayersPosition()
        {
            Vector3 position = Vector3.zero;
            _players.ForEach(p => position += p.transform.position);
            return position;
        }

        public void ApplyConstraints(bool apply, AbilityConstraint constraints)
        {
            _players.ForEach(p => p.Constraints.ApplyConstraints(apply, constraints));
        }

        public List<Transform> GetPlayers()
        {
            List<Transform> transforms = new List<Transform>();
            _players.ForEach(p => transforms.Add(p.transform));
            return transforms;
        }
    }
}
