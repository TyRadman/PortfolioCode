using UnityEngine;

namespace BT.Nodes
{
	public class OnPlayerEscapedActionNode : ActionNode
	{
		public BlackboardKey IsChasingKey;
		public BlackboardKey PlayerHasBeenSeenKey;
		private EyeIndicator m_Indicator;

        public override void OnAwake()
        {
            base.OnAwake();

			m_Indicator = Agent.EyeIndicator;
		}

        protected override void OnStart()
		{
			Blackboard.SetValue(IsChasingKey.Value, false);
			Blackboard.SetValue(PlayerHasBeenSeenKey.Value, false);

			m_Indicator.UpdateIdicator(EyeState.OnPlayerLost);
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