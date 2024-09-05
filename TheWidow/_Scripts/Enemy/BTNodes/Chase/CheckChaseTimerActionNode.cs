using UnityEngine;

namespace BT.Nodes
{
	public class CheckChaseTimerActionNode : ActionNode
	{
		private float m_LosingPlayerTime;
		public BlackboardKey m_ChaseTimerKey;

		protected override void OnStart()
		{
		}

		protected override NodeState OnUpdate()
		{
			Debug.Log($"{ Blackboard.GetValue<float>(m_ChaseTimerKey.Value)} vs {m_LosingPlayerTime}");
			bool match = Blackboard.GetValue<float>(m_ChaseTimerKey.Value) >= m_LosingPlayerTime;
			return match? NodeState.Success : NodeState.Failure;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}