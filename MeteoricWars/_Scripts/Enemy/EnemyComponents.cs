using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyComponents : ShipComponenets
    {
        public ShipRank Rank;
        public bool HasAbility = false;
        public EnemyShipMovement Movement;
        public EnemyShooting ShootingMethod;
        public EnemyShipAbilities Abilities;
        public EnemyStats EnemyStats;
        public float OffenceValue;
        public float DefenceValue;
        public float MobilityValue;
        public new EnemyHealth Health;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}
