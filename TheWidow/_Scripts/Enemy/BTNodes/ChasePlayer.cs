using UnityEngine;

namespace BT.Nodes
{
	public class ChasePlayer : ActionNode
	{
		private StatesEntity m_Entity;
		private EnemyAudio m_Audio;
		private EyeIndicator m_Indicator;
		[SerializeField] private MachineState.StateName m_StateTag;

        public override void OnAwake()
        {
            base.OnAwake();

			m_Entity = Agent.Entity;
			m_Audio = Agent.Audio;
			m_Indicator = Agent.EyeIndicator;
		}

        protected override void OnStart()
		{
			m_Indicator.UpdateIdicator(EyeState.IsPlayerOutOfSight);
			m_Entity.Indicator.EnableIsChasing(true);
			// set the chasing speed
			m_Entity.SetAgentSpeed(Agent.Difficulty.ChasingSpeed);
			// turn on vision again 
			Agent.Sight.CanSee = true;

            m_Entity.SetAgentTarget(m_Entity.PlayerTransform.position);

			m_Entity.Animation.Animate(m_StateTag);
			m_Audio.PlayAudio(m_StateTag);
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