using UnityEngine;

namespace BT.Nodes
{
	public class HearingCheckActionNode : ActionNode
	{
		private EntityHearing m_Hearing;
        private EyeIndicator m_Indicator;
		private StatesEntity m_Entity;
		public BlackboardKey NoisePositionKey;

        public override void OnAwake()
        {
            base.OnAwake();

			m_Hearing = Agent.Hearing;
			m_Indicator = Agent.EyeIndicator;
			m_Entity = Agent.Entity;
		}

        protected override void OnStart()
		{
			// start logic
		}

		protected override NodeState OnUpdate()
		{
			if (m_Hearing.HasHeardNoise())
			{
				m_Indicator.DisplayHearingIcon(true);
				Vector3 lastNoisePosition = m_Hearing.GetLastNoisePosition();
				Blackboard.SetValue(NoisePositionKey.Value, lastNoisePosition);
				return NodeState.Success;
			}

			return NodeState.Failure;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}