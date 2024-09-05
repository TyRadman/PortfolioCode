using UnityEngine;

namespace BT.Nodes
{
	public class IsPlayerOutSight : ConditionalCheckNode
    {
        [SerializeField] private EntitySight.SightState _successSightState;

        protected override bool IsTrue()
        {
            //Debug.Log($"{Agent.Sight.State} at node {ViewDetails.Name}");
            return Agent.Sight.State == _successSightState;
        }
    }
}