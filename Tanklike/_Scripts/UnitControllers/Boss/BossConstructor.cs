using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class BossConstructor : TankConstructor
    {
        [SerializeField] private BossComponents _components;

        public override void BuildTank()
        {
            base.BuildTank();

            Debug.Log("BUILD");
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
