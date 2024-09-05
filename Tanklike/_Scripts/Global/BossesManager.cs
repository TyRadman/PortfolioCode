using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class BossesManager : MonoBehaviour
    {
        [Header("Boss Room")]
        [SerializeField] private Transform _bossRoomCenter;
        [SerializeField] private Vector3 _bossRoomSize;

        private List<BossComponents> _bosses = new List<BossComponents>();
        private BossesDatabase _bossesDatabase;
        private Dictionary<BossType, Pool<UnitParts>> _bossPartsPools = new Dictionary<BossType, Pool<UnitParts>>();

        public Transform BossRoomCenter => _bossRoomCenter;
        public Vector3 BossRoomSize => _bossRoomSize;

        public void SetReferences(BossesDatabase bossesDatabase)
        {
            _bossesDatabase = bossesDatabase;
        }

        public void SetUp()
        {
            InitPools();
        }

        public void SpawnBoss()
        {

        }

        public void AddBoss(BossComponents boss)
        {
            _bosses.Add(boss);
            boss.SetUp();
        }

        public void RemoveBoss(TankComponents boss)
        {
            _bosses.Remove((BossComponents)boss);
        }

        public BossComponents GetBoss(int index)
        {
            return _bosses[index];
        }

        public UnitParts GetBossPartsByType(BossType type)
        {
            UnitParts parts = _bossPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }

        public void SwitchBackBGMusic()
        {
            StartCoroutine(SwitchBackBGMusicRoutine());
        }

        public IEnumerator SwitchBackBGMusicRoutine()
        {
            GameManager.Instance.AudioManager.FadeOutBGMusic();
            yield return new WaitForSeconds(1f);
            GameManager.Instance.AudioManager.SwitchBGMusic(GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().LevelMusic);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.AudioManager.FadeInBGMusic();
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.DrawWireCube(BossRoomCenter.position, _bossRoomSize);
        //}

        #region Pools
        private void InitPools()
        {
            foreach (var boss in _bossesDatabase.GetAllBosses())
            {
                if (boss.PartsPrefab == null) continue;
                _bossPartsPools.Add(boss.BossType, CreateBossPartsPool(boss));
            }
        }

        private Pool<UnitParts> CreateBossPartsPool(BossData bossData)
        {
            var pool = new Pool<UnitParts>(
                () =>
                {
                    var obj = Instantiate(bossData.PartsPrefab);
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
