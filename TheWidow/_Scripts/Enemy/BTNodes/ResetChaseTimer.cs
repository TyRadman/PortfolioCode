using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
    public class ResetChaseTimer : ActionNode
    {
        public BlackboardKey PlayerLosingTimerKey;

        protected override void OnStart()
        {
            Blackboard.SetValue(PlayerLosingTimerKey.Value, 0f);
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeState OnUpdate()
        {
            return NodeState.Success;
        }
    }
}