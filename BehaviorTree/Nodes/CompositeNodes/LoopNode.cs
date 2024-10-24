using UnityEngine;

namespace BT.Nodes
{
	public class LoopNode : DecoratorNode
	{
		private int _loopsCompleted;
		[SerializeField] private int _loopsToComplete = 3;

		protected override void OnStart()
		{
			_loopsToComplete = Mathf.Max(1, _loopsToComplete);

			_loopsCompleted = 0;
		}

		protected override NodeState OnUpdate()
		{
			NodeState childState = Child.Update();

			if(childState == NodeState.Failure || childState == NodeState.Success)
            {
				_loopsCompleted++;
            }

			if (_loopsCompleted >= _loopsToComplete)
			{
				return NodeState.Success;
            }

			return NodeState.Running;
		}

		protected override void OnStop()
		{

		}
	}
}