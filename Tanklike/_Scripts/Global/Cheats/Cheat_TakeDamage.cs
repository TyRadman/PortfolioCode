using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = NAME + "DealDamage", menuName = ROOT + "Deal Damage")]
    public class Cheat_TakeDamage : Cheat
    {
        [SerializeField] private int _damageToDeal = 10;

        public override void Initiate()
        {
            base.Initiate();
        }

        public override void PerformCheat()
        {
            GameManager.Instance.PlayersManager.GetPlayers().ForEach(p => p.Health.TakeDamage(_damageToDeal, Vector3.zero, null, p.transform.position));
        }
    }
}
