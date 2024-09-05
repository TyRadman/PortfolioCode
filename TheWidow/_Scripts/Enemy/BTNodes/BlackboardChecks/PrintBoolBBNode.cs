using UnityEngine;

namespace BT.Nodes
{
	public class PrintBoolBBNode : ActionNode
	{
		public BlackboardKey BoolKey;

		protected override void OnStart()
		{
			// start logic
		}

		protected override NodeState OnUpdate()
		{
			Debug.Log($"{Blackboard.GetValue<bool>(BoolKey.Value)}");
			return NodeState.Success;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}