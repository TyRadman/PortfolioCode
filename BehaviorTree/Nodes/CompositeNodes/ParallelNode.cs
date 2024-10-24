using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
	public class ParallelNode : CompositeNode
	{
		public enum ParallelType
		{
			OneChildFailIsFail = 0, AllChildrenFailIsFail = 1, NumberOfChildrenFailIsFail = 2
		}

		[SerializeField] private ParallelType _failureCondition;
		[HideInInspector] public int ConditionalFailedChildrenCount;

		protected override void OnStart()
		{

		}

		protected override NodeState OnUpdate()
		{
			switch (_failureCondition)
			{
				case ParallelType.OneChildFailIsFail:
					return PerformOneChildFailLogic();
				case ParallelType.AllChildrenFailIsFail:
					return PerformAllChildrenFailLogic();
				case ParallelType.NumberOfChildrenFailIsFail:
					return PerformNumberOfChildrenFailLogic();
				default:
					return NodeState.Running;
			}
		}

		private NodeState PerformNumberOfChildrenFailLogic()
		{
			int failedChildrenCount = 0;

			for (int i = 0; i < Children.Count; i++)
			{
				NodeState state = Children[i].Update();

				if (state == NodeState.Failure)
				{
					failedChildrenCount++;
				}

				if (failedChildrenCount >= ConditionalFailedChildrenCount)
				{
					return NodeState.Failure;
				}
			}

			return NodeState.Running;
		}

		private NodeState PerformAllChildrenFailLogic()
		{
			bool allFailed = true;

			for (int i = 0; i < Children.Count; i++)
			{
				NodeState state = Children[i].Update();

				if (state != NodeState.Failure)
				{
					allFailed = false;
				}
			}

			return allFailed ? NodeState.Failure : NodeState.Running;
		}

		private NodeState PerformOneChildFailLogic()
		{
			for (int i = 0; i < Children.Count; i++)
			{
				NodeState state = Children[i].Update();

				if (state == NodeState.Failure)
				{
					return NodeState.Failure;
				}
			}

			return NodeState.Running;
		}

		protected override void OnStop()
		{

		}
	}
}