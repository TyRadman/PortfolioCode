using UnityEngine;

namespace BT.Nodes
{
	public class SetUpTheWidowActionNode : ActionNode
	{
		public BlackboardKey IsChasingKey;
		public BlackboardKey PlayerSeenKey;

		protected override void OnStart()
		{

		}

		protected override NodeState OnUpdate()
		{
			Blackboard.SetValue(IsChasingKey.Value, false);
			Blackboard.SetValue(PlayerSeenKey.Value, false);
			return NodeState.Success;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}