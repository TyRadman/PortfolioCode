using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerConstructor : TankConstructor
    {
        [SerializeField] private PlayerComponents _components;

        public override void BuildTank()
        {
            base.BuildTank();

            _components.  SetUp();

            _currentBody.GetComponent<TankBody>().Setup(gameObject.tag);

            // copy the collider to the main tank gameObject so that all the health calculation is done there
            // we want the main game object of the tank to recieve damage, not the body. So we copy the collider of the body which will probably vary in dimension depending on the tank's size to the main gameobject
            //BoxCollider bodyCol = _currentBody.GetComponent<BoxCollider>();
            //BoxCollider col = gameObject.AddComponent<BoxCollider>();
            //col.size = bodyCol.size;
            //bodyCol.enabled = false;

            // add it to the players' manager
            //GameManager.Instance.PlayersManager.AddPlayer(_components);
        }
    }
}
