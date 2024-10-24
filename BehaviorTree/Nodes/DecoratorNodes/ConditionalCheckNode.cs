using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
    public abstract class ConditionalCheckNode : DecoratorNode
    {
        protected abstract bool IsTrue();

        protected override NodeState OnUpdate()
        {
            if (!IsTrue())
            {
                return NodeState.Failure;
            }

            return Child.Update();
        }
    }
}
