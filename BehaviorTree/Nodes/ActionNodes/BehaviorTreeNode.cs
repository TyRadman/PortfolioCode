using UnityEngine;

namespace BT.Nodes
{
	public class BehaviorTreeNode : ActionNode
	{
		[SerializeField] BehaviorTree _behaviorTree;
		[HideInInspector]
		public RootNode RootNode;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override BaseNode Clone()
		{
			base.Clone();

			if (_behaviorTree == null)
			{
				Debug.LogError($"No behavior tree referenced at the behavior tree node {name}");
				return null;
			}

			RootNode = _behaviorTree.RootNode as RootNode;

			if(RootNode == null)
            {

            }

			BehaviorTreeNode btNodeClone = Instantiate(this);
			RootNode rootNodeClone = Instantiate(RootNode);
			btNodeClone.RootNode = rootNodeClone;
			rootNodeClone.Clone();
			return btNodeClone;
		}

        protected override void OnStart()
		{

		}

		protected override NodeState OnUpdate()
		{
			NodeState state = RootNode.Update();
			return state;
		}

		protected override void OnStop()
		{

		}

		public BehaviorTree GetBehaviorTree()
        {
			return _behaviorTree;
        }
	}
}