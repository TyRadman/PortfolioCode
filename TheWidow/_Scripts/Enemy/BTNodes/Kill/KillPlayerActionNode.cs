using UnityEngine;

namespace BT.Nodes
{
	public class KillPlayerActionNode : ActionNode
	{
		private StatesEntity m_Entity;

		protected override void OnStart()
		{
			m_Entity = Agent.Entity;

			// rotate the enemy towards the player so that the animation plays well
			m_Entity.RotateToTarget(m_Entity.PlayerTransform.position, 1f);
			// create a fading screen
			FadingScreen.Instance.Fade(0.3f, 0.02f, 0.1f, Color.black);
			// call the game over function
			GameManager.Instance.GameOver();
			PlayerMovement.Instance.AllowMovement(false);
			PlayerStats.Instance.IsDead = true;
			// stopping the enemy
			m_Entity.Agent.enabled = false;
		}

		protected override NodeState OnUpdate()
		{
			m_Entity.RotateToTarget(m_Entity.PlayerTransform.position, 1f);
			return NodeState.Running;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
}