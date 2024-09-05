using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State_TheWidow_MoveToPoint", menuName = Constants.WIDOW_STATES_DIRECTORY + "Move to point")]
public class StateMoveToHearingTargetWidow : MachineState
{
    private Vector3 m_Target;
    private const float STOPPING_DISTNACE = 3f;
    private EntityHearing m_Hearing;
    private EntitySight m_Sight;
    private EyeIndicator m_Indicator;
    private float m_PatrolSpeed;

    public override void SetUp(IComponent entityComponents)
    {
        base.SetUp(entityComponents);
        m_Hearing = ((EntityComponents)entityComponents).Hearing;
        m_Sight = ((EntityComponents)entityComponents).Sight;
        m_Indicator = m_Components.Entity.Indicator;
        m_PatrolSpeed = GameManager.Instance.Settings.CurrentDifficulty.WalkingSpeed;
    }

    public override void StartState()
    {
        m_StateMachine.SetAgentSpeed(m_PatrolSpeed);
        SetTarget();
        base.StartState();
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        CheckForSightInput();
        CheckForHearingInput();

        if ((m_StateMachine.EntityTransform.position - m_Target).sqrMagnitude > STOPPING_DISTNACE) // if the enemy is not close enough to the way point
        {
            m_StateMachine.SetAgentTarget(m_Target);
        }
        else
        {
            m_StateMachine.SetNextState(StateName.Stand);
            m_StateMachine.StopState();
        }
    }

    private void CheckForSightInput()
    {
        if (m_Sight.State == EntitySight.SightState.PlayerInSight)
        {
            m_StateMachine.SetNextState(StateName.Scream);
            m_StateMachine.StopState();
        }
    }

    private void CheckForHearingInput()
    {
        if (m_Hearing.State == EntityHearing.HearingState.PlayerHeard)
        {
            m_Indicator.DisplayHearingIcon(true);
            SetTarget();
            //m_StateMachine.SetNextState(StateName.MoveToPoint);
            //m_StateMachine.StopState();
        }
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }

    private void SetTarget()
    {
        m_Target = m_Hearing.HearingTarget;
    }
}
