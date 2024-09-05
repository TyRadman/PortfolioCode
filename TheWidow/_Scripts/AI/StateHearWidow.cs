using System.Collections;
using UnityEngine;

public class StateHearWidow : MachineState
{
    private float m_PatrolSpeed = 3f;

    public override void StartState()
    {
        // set the speed of the entity
        m_PatrolSpeed = GameManager.Instance.Settings.CurrentDifficulty.ChasingSpeed;
        m_StateMachine.SetAgentSpeed(m_PatrolSpeed);
        m_StateMachine.SetAgentTarget(m_StateMachine.PlayerTransform.position);

        base.StartState();
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        if ((m_StateMachine.EntityTransform.position - m_StateMachine.Target).sqrMagnitude > 3) // if the enemy is not close enough to the way point
        {
            m_StateMachine.SetAgentTarget(m_StateMachine.Target);
        }
        else
        {
            m_StateMachine.SetNextState(StateName.Stand);
        }
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }
}