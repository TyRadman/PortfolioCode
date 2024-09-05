using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
	public class SelectWayPoint : ActionNode
	{
		public BlackboardKey CurrentWayPointKey;
		private StatesEntity m_Entity;

		protected override void OnStart()
		{
			m_Entity = this.GetEntity();

			if(m_Entity == null)
            {
				Debug.Break();
            }

			Vector3 lastPoint = Blackboard.GetValue<Vector3>(CurrentWayPointKey.Value);

			List<Vector3> availableWayPoints = m_Entity.WayPoints.FindAll(wp => wp != lastPoint && m_Entity.DestinationIsReachable(wp));
			lastPoint = availableWayPoints[Random.Range(0, availableWayPoints.Count)];

			Blackboard.SetValue(CurrentWayPointKey.Value, lastPoint);

			m_Entity.SetAgentTarget(lastPoint);
		}

		protected override NodeState OnUpdate()
		{
			return NodeState.Success;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}