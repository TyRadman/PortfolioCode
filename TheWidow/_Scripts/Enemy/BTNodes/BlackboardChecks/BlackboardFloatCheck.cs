using System;
using UnityEngine;

namespace BT.Nodes
{
	public class BlackboardFloatCheck : ActionNode
	{
		public enum Comparisons
        {
			Equals = 0, NotEquals = 1, Greater = 2, GreaterOrEquals = 3, Smaller = 4, SmallerOrEquals = 5
        }

		public BlackboardKey FloatKey;
		[SerializeField] private float _valueToCompareTo = 0f;
		[SerializeField] private Comparisons _compaison = Comparisons.GreaterOrEquals;

		protected override void OnStart()
		{

		}

		protected override NodeState OnUpdate()
		{
			return GetResult();
		}

        private NodeState GetResult()
        {
			float keyValue = Blackboard.GetValue<float>(FloatKey.Value);

			switch(_compaison)
            {
				case Comparisons.Equals:
					return _valueToCompareTo == keyValue ? NodeState.Success : NodeState.Failure;
				case Comparisons.NotEquals:
					return _valueToCompareTo != keyValue ? NodeState.Success : NodeState.Failure;
				case Comparisons.Greater:
					return _valueToCompareTo > keyValue ? NodeState.Success : NodeState.Failure;
				case Comparisons.GreaterOrEquals:
					return _valueToCompareTo >= keyValue ? NodeState.Success : NodeState.Failure;
				case Comparisons.Smaller:
					return _valueToCompareTo < keyValue ? NodeState.Success : NodeState.Failure;
				case Comparisons.SmallerOrEquals:
					return _valueToCompareTo <= keyValue ? NodeState.Success : NodeState.Failure;
				default:
					return NodeState.Failure;
			}
        }

        protected override void OnStop()
		{

		}
	}
}