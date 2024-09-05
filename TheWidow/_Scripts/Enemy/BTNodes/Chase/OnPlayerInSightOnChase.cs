using UnityEngine;

namespace BT.Nodes
{
	public class OnPlayerInSightOnChase : ActionNode
	{
		private EyeIndicator m_Indicator;

        public override void OnAwake()
        {
            base.OnAwake();

			m_Indicator = Agent.EyeIndicator;
        }

        protected override void OnStart()
		{
			m_Indicator.UpdateIdicator(EyeState.IsPlayerInSight);
		}

		protected override NodeState OnUpdate()
		{
			return NodeState.Success;
		}

		protected override void OnStop()
		{
		}
	}
}