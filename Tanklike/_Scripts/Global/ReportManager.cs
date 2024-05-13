using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using TankLike.ItemsSystem;
using TankLike.Environment;
using TankLike.Combat.Destructible;

namespace TankLike
{
    public class ReportManager : MonoBehaviour
    {
        public System.Action<EnemyData, int> OnEnemyKill { get; set; }
        public System.Action<Collectable, int> OnCollectableCollected;
        public System.Action<DestructableTag, int> OnObjectDestroyed;
        public System.Action<BossData, int> OnBossKill;

        public void ReportEnemyKill(EnemyData data, int playerIndex)
        {
            OnEnemyKill?.Invoke(data, playerIndex);
        }

        public void ReportCollection(Collectable collectable, int playerIndex)
        {
            OnCollectableCollected?.Invoke(collectable, playerIndex);
        }

        public void ReportDestroyingObject(DestructableTag tag, int playerIndex)
        {
            OnObjectDestroyed?.Invoke(tag, playerIndex);
        }

        public void ReportBossKill(BossData data, int playerIndex)
        {
            OnBossKill?.Invoke(data, playerIndex);
        }
    }
}
