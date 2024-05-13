using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyConstructor : TankConstructor
    {
        [SerializeField] private EnemyComponents _components;

        //private void Awake()
        //{
        //    BuildTank();
        //}

        public override void BuildTank()
        {
            base.BuildTank();

            _components.SetUp();

            _currentBody.GetComponent<TankBody>().Setup(gameObject.tag);

            // copy the collider to the main tank gameObject so that all the health calculation is done there
            //BoxCollider bodyCol = _currentBody.GetComponent<BoxCollider>();
            //BoxCollider col = gameObject.AddComponent<BoxCollider>();
            //col.size = bodyCol.size;
            //bodyCol.enabled = false;
        }
    }
}
