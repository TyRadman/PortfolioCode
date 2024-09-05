using System.Collections;
using UnityEngine;

namespace BT.Nodes
{
	public class IncrementPlayerLosingTimerActionNode : ActionNode
	{
		public BlackboardKey PlayerLosingTimerKey;
		private float m_LosingPlayerTime;
		private bool m_CountDownFinished = false;
		private EyeIndicator m_Indicator;

        public override void OnAwake()
        {
            base.OnAwake();

			m_Indicator = Agent.EyeIndicator;
			m_LosingPlayerTime = Agent.Difficulty.LosingPlayerTime;
		}

        protected override void OnStart()
		{
			m_CountDownFinished = false;
			m_Indicator.UpdateIdicator(EyeState.IsPlayerOutOfSight);
			Agent.StartCoroutine(CountDownProcess());
		}

		protected override NodeState OnUpdate()
		{
			if(m_CountDownFinished)
            {
				return NodeState.Success;
            }

			return NodeState.Running;
		}

		private IEnumerator CountDownProcess()
        {
			float timerValue = 0f;

			while (timerValue < m_LosingPlayerTime)
            {
				timerValue += + Time.deltaTime;
				GameManager.Instance.Debugger.UpdateBarImageAmount(timerValue / m_LosingPlayerTime);
				//Debug.Log(timerValue);
				yield return null;
			}

			GameManager.Instance.Debugger.UpdateBarImageAmount(0);
			m_CountDownFinished = true;
		}

		protected override void OnStop()
		{
			// stop logic
		}

        public override void OnForceStopNode()
        {
            base.OnForceStopNode();
			Agent.StopAllCoroutines();
			GameManager.Instance.Debugger.UpdateBarImageAmount(0);
		}
    }
}