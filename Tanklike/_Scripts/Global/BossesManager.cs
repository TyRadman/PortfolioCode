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
        private Dictionary<BossType, Pool<EnemyParts>> _bossPartsPools = new Dictionary<BossType, Pool<EnemyParts>>();

        public Transform BossRoomCenter => _bossRoomCenter;
        public Vector3 BossRoomSize => _bossRoomSize; 

        public void SetUp(BossesDatabase bossesDatabase)
        {
            _bossesDatabase = bossesDatabase;

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

        public void RemoveBoss(BossComponents boss)
        {
            _bosses.Remove(boss);
        }

        public BossComponents GetBoss(int index)
        {
            return _bosses[index];
        }

        public EnemyParts GetBossPartsByType(BossType type)
        {
            EnemyParts parts = _bossPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
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

        private Pool<EnemyParts> CreateBossPartsPool(BossData bossData)
        {
            var pool = new Pool<EnemyParts>(
                () =>
                {
                    var obj = Instantiate(bossData.PartsPrefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj.GetComponent<EnemyParts>();
                },
                (EnemyParts obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (EnemyParts obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (EnemyParts obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }
        #endregion
    }
}
