using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.UI.Notifications;
using TankLike.Utils;

namespace TankLike
{
    using UI.HUD;

    /// <summary>
    /// This script holds all the information about the players, like their references for the enemies to access, their selected characters, upgrades and anything that other classes might need from the players.
    /// </summary>
    public class PlayersManager : MonoBehaviour, IManager
    {
        // TODO: should be taken out as a class of its own and cleaned
        public class PlayerTransforms
        {
            public Transform PlayerTransform;
            public Transform ImageTransform;
            public int PlayerIndex;
            public PlayerPredictedPosition PredictedPosition;

            public PlayerTransforms(PlayerComponents components)
            {
                PlayerTransform = components.transform;
                PredictedPosition = components.PredictedPosition;
                ImageTransform = components.PredictedPosition.GetImage();
                PlayerIndex = components.PlayerIndex;
            }

            public Vector3 GetImageAtDistance(float distance)
            {
                return PredictedPosition.GetPositionAtDistance(distance);
            }
        }

        [field: SerializeField] public PlayerSpawner PlayerSpawner { get; private set; }
        [field: SerializeField] public PlayerConstraintsManager Constraints { get; private set; }
        [field: SerializeField] public PlayerCoinsManager Coins { get; private set; }
        public System.Action<PlayerComponents> OnPlayerSpawned { get; set; }
        public static int PlayersCount { get; private set; }
        public static int ActivePlayersCount { get; private set; }

        public bool IsActive { get; private set; }

        //[field: SerializeField] public int CoinsAmount { get; private set; }
        public static int PlayerLayer = 11;
        public LayerMask PlayerLayerMask;

        [SerializeField] private List<GameplaySettings> _playersGameplaySettings;
        [SerializeField] private OffScreenIndicatorProfile[] _offScreenIndicatorProfiles;
        
        private Dictionary<PlayerType, Pool<UnitParts>> _playerPartsPools = new Dictionary<PlayerType, Pool<UnitParts>>();
        private List<PlayerComponents> _players = new List<PlayerComponents>();
        private List<PlayerTransforms> _playersTransforms = new List<PlayerTransforms>();
        private GameplaySettings _defaultSettings;
        private PlayersDatabase _playersDatabase;

        public void SetReferences(PlayersDatabase playersDatabase)
        {
            _playersDatabase = playersDatabase;

            PlayerSpawner.SetReferences(playersDatabase);
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            InitPools();

            PlayerSpawner.SetUp();
            Constraints.SetUp();
            Coins.SetUp();
        }

        public void Dispose()
        {
            IsActive = false;

            DisposePools();

            PlayerSpawner.Dispose();
            Constraints.Dispose();
            Coins.Dispose();
        }
        #endregion

        public void AddPlayer(PlayerComponents player)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }
                
            player.IsAlive = true;
            _players.Add(player);
            AddPlayerTransform(player);
            OnPlayerSpawned?.Invoke(player);
            player.SetUpSettings();
            PlayersCount = _players.Count;
            SetColors(player);
            SetPlayerTexture(player);
        }

        public void OnTwoPlayersMode()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            AddPlayersAsOffScreenIndicatorTargets();
        }

        private void AddPlayersAsOffScreenIndicatorTargets()
        {
            for (int i = 0; i < 2; i++)
            {
                OffScreenIndicatorProfile profile = _offScreenIndicatorProfiles[i];
                profile.FollowTarget = true;
                profile.IsShown = true;
                profile.TargetTransform = _players[i].transform;
                GameManager.Instance.OffScreenIndicator.AddTarget(profile);
                profile.Icon.SetColor(GetPlayerColor(i));
            }
        }

        public void AddPlayerTransform(PlayerComponents player)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            PlayerTransforms playerTransform = new PlayerTransforms(player);
            _playersTransforms.Add(playerTransform);

            ActivePlayersCount = _playersTransforms.Count;
        }

        #region Get Methods
        public List<PlayerTransforms> GetPlayerTransforms()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _playersTransforms;
        }

        public Transform GetClosestPlayerTransform(Vector3 startPoint)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (ActivePlayersCount == 0)
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
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

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
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (_playersTransforms.Count == 0)
            {
                return null;
            }

            if (ActivePlayersCount < 2)
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

        public PlayerTransforms GetFarthestPlayer(Vector3 startPoint)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (ActivePlayersCount == 1)
            {
                return _playersTransforms[0];
            }

            float distanceToPlayerOne = (_playersTransforms[0].ImageTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].ImageTransform.position - startPoint).sqrMagnitude;

            if (distanceToPlayerOne > distanceToPlayerTwo)
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
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _players;
        }

        public PlayerComponents GetPlayer(int index)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _players[index];
        }

        public int GetPlayersCount()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            return _players.Count;
        }

        public int GetInactivePlayerIndex()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            if (ActivePlayersCount == PlayersCount)
            {
                return -1;
            }

            return _players.Find(p => !p.IsAlive).PlayerIndex;
        }

        public List<Transform> GetPlayers()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            List<Transform> transforms = new List<Transform>();
            _players.ForEach(p => transforms.Add(p.transform));
            return transforms;
        }

        public UnitParts GetPlayerPartsByType(PlayerType type)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            UnitParts parts = _playerPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }
        #endregion

        public void SetAimSensitivity(float amount, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _playersGameplaySettings[playerIndex].AimSensitivity = amount;
            _players[playerIndex].Crosshair.SetAimSensitivity(amount);
        }

        public GameplaySettings GetGameplaySettings(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _playersGameplaySettings[playerIndex];
        }

        public void EnablePauseInputForSecondPlayer(int playerIndex, bool enable)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

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

        public void ReportPlayerDeath(TankComponents components)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            PlayerComponents player = (PlayerComponents)components;
            int playerIndex = player.PlayerIndex;
            player.IsAlive = false;
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

        private void SetColors(PlayerComponents player)
        {
            int index = ActivePlayersCount - 1;
            Color color = ((PlayerData)player.Stats).Skins[index].Color;
            _players[index].Crosshair.SetColor(color);
        }

        private void SetPlayerTexture(PlayerComponents player)
        {
            int index = ActivePlayersCount - 1;
            Texture2D texture = ((PlayerData)player.Stats).Skins[index].Texture;
            _players[index].Visuals.SetTextureForMainMaterial(texture);
            _players[index].TankBodyParts.SetTextureForMainMaterial(texture);
        }

        private Color GetPlayerColor(int playerIndex)
        {
            Color color = ((PlayerData)_players[playerIndex].Stats).Skins[playerIndex].Color;
            return color;
        }

        public float GetHealth()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0f;
            }

            float health = 0f;
            _players.ForEach(p => health += p.Health.GetHealth());
            return health;
        }

        /// <summary>
        /// Returns the the players' position. If it's a single player, then only the position of that player is returned, otherwise, the average of the position of both players is returned.
        /// </summary>
        public Vector3 GetPlayersPosition()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return Vector3.zero;
            }

            Vector3 position = Vector3.zero;
            _players.ForEach(p => position += p.transform.position);
            return position;
        }

        public void ApplyConstraints(bool apply, AbilityConstraint constraints)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _players.ForEach(p => p.Constraints.ApplyConstraints(apply, constraints));
        }

        #region Pools
        private void InitPools()
        {
            foreach (var player in _playersDatabase.GetAllPlayers())
            {
                if (player.PartsPrefab == null) continue;
                _playerPartsPools.Add(player.PlayerType, CreatePlayerPartsPool(player));
            }
        }

        private void DisposePools()
        {
            foreach (KeyValuePair<PlayerType, Pool<UnitParts>> playerParts in _playerPartsPools)
            {
                playerParts.Value.Clear();
            }

            _playerPartsPools.Clear();
        }

        private Pool<UnitParts> CreatePlayerPartsPool(PlayerData playerData)
        {
            var pool = new Pool<UnitParts>(
                () =>
                {
                    var obj = Instantiate(playerData.PartsPrefab);
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
    }
}
