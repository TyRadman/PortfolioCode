using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyShooting : MonoBehaviour
    {
        [HideInInspector] public ShipComponenets Components;

        public virtual void SetUp()
        {

        }

        public virtual void PerformShooting()
        {

        }

        public void ShootingProcess()
        {
            Components.ShipShooter.Shoot();
        }

        public virtual void StopShooting()
        {

        }
    }
}