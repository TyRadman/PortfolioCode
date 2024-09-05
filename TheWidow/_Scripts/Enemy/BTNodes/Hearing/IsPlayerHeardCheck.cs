using UnityEngine;

namespace BT.Nodes
{
	public class IsPlayerHeardCheck : ConditionalCheckNode
	{
		[SerializeField] private EntityHearing.HearingState m_SuccessHearingState;
		private EntityHearing m_Hearing;

        public override void OnAwake()
        {
            base.OnAwake();

			m_Hearing = Agent.Hearing;
		}

        protected override bool IsTrue()
		{
			return m_Hearing.State == m_SuccessHearingState;
		}
	}
}