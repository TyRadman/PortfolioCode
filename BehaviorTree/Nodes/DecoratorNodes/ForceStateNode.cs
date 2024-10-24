using UnityEngine;

namespace BT.Nodes
{
	public class ForceStateNode : DecoratorNode
	{
		[SerializeField] private NodeState _returnedState;

		protected override void OnStart()
		{
		}

		protected override NodeState OnUpdate()
		{
			NodeState state = Child.Update();

			if (state == NodeState.Running)
			{
				return state;
			}

			return _returnedState;
		}

		protected override void OnStop()
		{
			// end logic
		}
	}
}