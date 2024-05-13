using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class BossKeysManager : MonoBehaviour
    {
        [SerializeField] private int _keysCount = 3;
        [SerializeField] private int _collectedKeys = 0;
        private BossRoomGate _bossRoomGate;
        private bool _keysSpawned = false;

        public void SetUp()
        {
            //DistributeKeysAcrossRooms();
        }

        public void DistributeKeysAcrossRemainingRooms(List<WaveData> waveDatas)
        {
            if (_keysSpawned) return;

            _keysSpawned = true;

            List<WaveData> wavesCopy = new List<WaveData>();
            waveDatas.FindAll(w => w.CanHaveKey()).ForEach(w => wavesCopy.Add(w));

            if (wavesCopy.Count < _keysCount)
            {
                Debug.LogError($"Not enough waves that can hold keys. Only have {wavesCopy.Count} waves that can hold keys.");
                return;
            }

            for (int i = 0; i < _keysCount; i++)
            {
                WaveData wave = wavesCopy.RandomItem();
                wavesCopy.Remove(wave);
                wave.HasKey = true;
            }
        }

        public void OnKeyCollected()
        {
            _collectedKeys++;
        }

        public bool HasEnoughKeys()
        {
            return _collectedKeys >= _keysCount;
        }

        public void SetBossRoomGate(BossRoomGate gate)
        {
            _bossRoomGate = gate;
        }

        public void PlaceKeys(int playerIndex)
        {
            _bossRoomGate.PlaceKeys(playerIndex);
        }
    }
}
