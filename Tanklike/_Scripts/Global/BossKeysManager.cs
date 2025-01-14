using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Environment;
    using Utils;
    using UI.Notifications;
    using System.Linq;

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

            GameManager.Instance.HUDController.UpdateKeysCount(_collectedKeys, _requiredKeysCount);
        }
        #endregion

        public void DistributeKeysAcrossRemainingRooms(List<Room> rooms)
        {
            if (_keysSpawned)
            {
                return;
            }

            _keysSpawned = true;

            List<Room> roomsList = rooms.FindAll(r => r is NormalRoom && r.RoomType is RoomType.Normal or RoomType.BossGate).ToList();

            for (int i = 0; i < _requiredKeysCount; i++)
            {
                Room room = roomsList.RandomItem();

                if (room is not NormalRoom normalRoom)
                {
                    Debug.LogError($"Room {room.name} is not a NormalRoom");
                    return;
                }

                roomsList.Remove(room);
                normalRoom.FlagAsKeyHolder();
            }

            //List<WaveData> wavesDataCopy = new List<WaveData>();

            //wavesDataCopy = waveDatas.FindAll(w => w.CanHaveKey()).ToList();

            //if (wavesDataCopy.Count < _requiredKeysCount)
            //{
            //    Debug.LogError($"Not enough waves that can hold keys. Only have {wavesDataCopy.Count} waves that can hold keys.");
            //    return;
            //}

            //for (int i = 0; i < _requiredKeysCount; i++)
            //{
            //    WaveData wave = wavesDataCopy.RandomItem();
            //    wavesDataCopy.Remove(wave);
            //    wave.HasKey = true;
            //}
        }

        public void OnKeyCollected()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            SetBossKeysCount(++_collectedKeys);
        }

        private void SetBossKeysCount(int count)
        {
            _collectedKeys = count;

            GameManager.Instance.HUDController.UpdateKeysCount(_collectedKeys, _requiredKeysCount);

            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, 1, 0);
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
