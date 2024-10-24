using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT.Nodes
{
    public class RootNode : BaseNode
    {
        //[HideInInspector]
        [SerializeField]
        private BaseNode Child;

        public override void OnAwake()
        {
            Child.OnAwake();
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override NodeState OnUpdate()
        {
            return Child.Update();
        }

        public override void AddChild(BaseNode child)
        {
            Undo.RecordObject(this, "Behavior Tree (Add Child)");

            if (child == null)
            {
                Debug.Log("CHILD IS NULL");
            }

            Child = child;
            base.AddChild(child);
        }

        public override void RemoveChild(BaseNode child)
        {
            Undo.RecordObject(this, "Behavior Tree (Remove Child)");
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
            RootNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }

        public override void OnForceStopNode()
        {
            base.OnForceStopNode();

            State = NodeState.Success;
            Child.OnForceStopNode();
        }
    }
}
