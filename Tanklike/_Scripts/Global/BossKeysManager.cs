using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    using UI.Notifications;

    public class BossKeysManager : MonoBehaviour, IManager
    {
        [SerializeField] private int _requiredKeysCount = 3;
        [SerializeField] private int _collectedKeys = 0;
        private BossRoomGate _bossRoomGate;
        private bool _keysSpawned = false;
        [SerializeField] private NotificationBarSettings_SO _notificationSettings;

        public bool IsActive { get; private set; }


        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }
        public void Dispose()
        {
            IsActive = false;

            _collectedKeys = 0;
        }
        #endregion

        public void DistributeKeysAcrossRemainingRooms(List<WaveData> waveDatas)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (_keysSpawned) return;

            _keysSpawned = true;

            List<WaveData> wavesCopy = new List<WaveData>();
            waveDatas.FindAll(w => w.CanHaveKey()).ForEach(w => wavesCopy.Add(w));

            if (wavesCopy.Count < _requiredKeysCount)
            {
                Debug.LogError($"Not enough waves that can hold keys. Only have {wavesCopy.Count} waves that can hold keys.");
                return;
            }

            for (int i = 0; i < _requiredKeysCount; i++)
            {
                WaveData wave = wavesCopy.RandomItem();
                wavesCopy.Remove(wave);
                wave.HasKey = true;
            }
        }

        public void OnKeyCollected()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _collectedKeys++;

            string amountText = $" ({_collectedKeys}/{_requiredKeysCount})";
            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, 1, 0, amountText);
        }

        public bool HasEnoughKeys()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return false;
            }

            return _collectedKeys >= _requiredKeysCount;
        }

        public void SetBossRoomGate(BossRoomGate gate)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _bossRoomGate = gate;
        }

        public void PlaceKeys(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _bossRoomGate.PlaceKeys(playerIndex);
        }

        public int GetCollectedKeysCount()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            return _collectedKeys;
        }

        public int GetRequiredKeysCount()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            return _requiredKeysCount;
        }
    }
}
