using UnityEngine;
using System.Collections;


namespace BT.Nodes
{
	public class StandAction : ActionNode
	{
		[SerializeField] private float _standDuration = 3f;
		private TheWidowComponents _theWidow;
		private Coroutine _standingCoroutine;
		private StatesEntity m_Entity;

		protected override void OnStart()
		{
			m_Entity = this.GetEntity();

			if (_theWidow == null)
			{
				_theWidow = Agent.GetComponent<TheWidowComponents>();
			}

			_standingCoroutine = _theWidow.StartCoroutine(StandingProcess());

			m_Entity.Animation.Animate(MachineState.StateName.Stand);
		}

		private IEnumerator StandingProcess()
        {
			yield return new WaitForSeconds(_standDuration);
			_standingCoroutine = null;
			State = NodeState.Success;
        }

		protected override NodeState OnUpdate()
		{
			return _standingCoroutine == null ? NodeState.Success : NodeState.Running;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}