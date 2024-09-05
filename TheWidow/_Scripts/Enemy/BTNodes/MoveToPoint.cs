using UnityEngine;

namespace BT.Nodes
{
	public class MoveToPoint : ActionNode
	{
		public BlackboardKey PointToMoveToKey;
		public BlackboardKey PatrolSpeedKey;
		private Vector3 m_Point = Vector3.zero;
		private StatesEntity m_Entity;

		private const float STOPPING_DISTNACE = 3f;

        protected override void OnStart()
		{
			m_Entity = this.GetEntity();

			m_Point = Blackboard.GetValue<Vector3>(PointToMoveToKey.Value);

			m_Entity.SetAgentSpeed(Blackboard.GetValue<float>(PatrolSpeedKey.Value));

			m_Entity.SetAgentTarget(m_Point);

			m_Entity.Animation.Animate(MachineState.StateName.Patrol);
		}

		protected override NodeState OnUpdate()
		{
			if (m_Point == Vector3.zero)
			{
				return NodeState.Failure;
			}

			// if the widow is not with the point distance
			if ((m_Entity.EntityTransform.position - m_Point).sqrMagnitude > STOPPING_DISTNACE)
			{
				return NodeState.Running;
			}

			return NodeState.Success;
		}

		protected override void OnStop()
		{
			m_Entity.SetAgentSpeed(0f);
		}

        public override void OnForceStopNode()
		{
			base.OnForceStopNode();
		}
    }
}