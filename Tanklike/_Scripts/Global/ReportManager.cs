using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using TankLike.ItemsSystem;
using TankLike.Environment;
using TankLike.Combat.Destructible;

namespace TankLike
{
    public class ReportManager : MonoBehaviour, IManager
    {
        public System.Action<Collectable, int> OnCollectableCollected;
        public System.Action<DestructableTag, int> OnObjectDestroyed;
        public System.Action<BossData, int> OnBossKill;

        public System.Action<EnemyData, int> OnEnemyKill { get; set; }

        public bool IsActive { get; private set; }


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

        #region Reports

        public void ReportEnemyKill(EnemyData data, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnEnemyKill?.Invoke(data, playerIndex);
        }

        public void ReportCollection(Collectable collectable, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnCollectableCollected?.Invoke(collectable, playerIndex);
        }

        public void ReportDestroyingObject(DestructableTag tag, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnObjectDestroyed?.Invoke(tag, playerIndex);
        }

        public void ReportBossKill(TankComponents components)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnBossKill?.Invoke((BossData)components.Stats, ((BossHealth)components.Health).PlayerIndex);
        }
        #endregion
    }
}
