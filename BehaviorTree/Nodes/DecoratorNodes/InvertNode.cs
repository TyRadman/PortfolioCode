using UnityEngine;

namespace BT.Nodes
{
	public class InvertNode : DecoratorNode
	{
		protected override void OnStart()
		{

		}

		protected override NodeState OnUpdate()
		{
			NodeState state = Child.Update();

			switch (state)
			{
				case NodeState.Running:
					{
						return NodeState.Running;
					}
				case NodeState.Success:
					{
						return NodeState.Failure;
					}
				case NodeState.Failure:
					{
						return NodeState.Success;
					}
				default:
					{
						return state;
					}
			}
		}

		protected override void OnStop()
		{
			// end logic
		}
	}
}