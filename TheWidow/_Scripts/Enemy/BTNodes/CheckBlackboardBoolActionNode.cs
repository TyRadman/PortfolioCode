using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
    public class CheckBlackboardBoolActionNode : ActionNode
    {
        public BlackboardKey BoolKey;
        [SerializeField] private bool _value;
        [SerializeField] private bool _debug = false;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override NodeState OnUpdate()
        {
            bool match = _value == Blackboard.GetValue<bool>(BoolKey.Value);
            if(_debug) Debug.Log($"Match is {match} at node {ViewDetails.Name}");
            return match ? NodeState.Success : NodeState.Failure;
        }
    }
}