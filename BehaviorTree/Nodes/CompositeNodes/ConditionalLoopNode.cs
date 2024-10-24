using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
    public abstract class ConditionalLoopNode : DecoratorNode
    {
		private int _loopsCompleted;
		[SerializeField] private int _loopsToComplete = 3;

		/// <summary>
		/// When false, the loop breaks.
		/// </summary>
		/// <returns></returns>
		protected abstract bool IsTrue();

		protected override void OnStart()
		{
			_loopsToComplete = Mathf.Max(1, _loopsToComplete);

			_loopsCompleted = 0;
		}

		protected override NodeState OnUpdate()
		{
			if(!IsTrue())
            {
				return NodeState.Failure;
            }

			NodeState childState = Child.Update();

			if (childState == NodeState.Failure || childState == NodeState.Success)
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
