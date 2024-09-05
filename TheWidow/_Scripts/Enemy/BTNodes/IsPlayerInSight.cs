using System.Collections;
using UnityEngine;

namespace BT.Nodes
{
    public class IsPlayerInSight : ConditionalCheckNode
    {
        protected override bool IsTrue()
        {
            return Agent.Sight.State == EntitySight.SightState.PlayerInSight;
        }
    }
}