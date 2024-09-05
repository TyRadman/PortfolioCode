using UnityEngine;

namespace BT.Nodes
{
	public class SetBBBoolActionNode : ActionNode
	{
		public BlackboardKey BoolKey;
		[SerializeField] private bool _value;

		protected override void OnStart()
		{
		}

		protected override NodeState OnUpdate()
        {
            try
            {
				Blackboard.SetValue(BoolKey.Value, _value);
				return NodeState.Success;
			}
            catch
            {
				return NodeState.Failure;
            }

		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}