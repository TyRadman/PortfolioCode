using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT.Nodes
{
    public abstract class DecoratorNode : BaseNode
    {
        [HideInInspector] public BaseNode Child;

        public override void OnAwake()
        {
            Child.OnAwake();
        }

        public override void AddChild(BaseNode child)
        {
            Undo.RecordObject(this, "Behavior Tree (Add Child)");
            Child = child;
            base.AddChild(child);
        }

        public override void RemoveChild(BaseNode child)
        {
            Undo.RecordObject(this, "Behavior Tree (remove Child)");
            Child = null;
            base.RemoveChild(child);
        }

        public override List<BaseNode> GetChildren()
        {
            base.GetChildren();
            return new List<BaseNode>() { Child };
        }

        public override bool HasChildren()
        {
            return Child != null;
        }

        public override BaseNode Clone()
        {
            // we need to clone the node and its child and then assign the child to its new parent
            DecoratorNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        public override void OnForceStopNode()
        {
            base.OnForceStopNode();

            State = NodeState.Failure;
            Child.OnForceStopNode();
        }

        public override void ClearChildren()
        {
            Child = null;
        }
    }
}
