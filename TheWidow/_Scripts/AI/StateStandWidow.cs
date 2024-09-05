using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State_TheWidow_Stand", menuName = Constants.WIDOW_STATES_DIRECTORY + "Stand")]
public class StateStandWidow : MachineState
{
    [SerializeField] private float m_MinWaitingTime = 2f;
    [SerializeField] private float m_MaxWaitingTime = 5f;
    private Coroutine m_WaitingCoroutine;
    private EntityHearing m_Hearing;
    private EntitySight m_Sight;
    private EyeIndicator m_Indicator;


    public override void SetUp(IComponent entityComponents)
    {
        base.SetUp(entityComponents);
        m_Hearing = ((EntityComponents)entityComponents).Hearing;
        m_Sight = ((EntityComponents)entityComponents).Sight;
        m_Indicator = m_Components.EyeIndicator;
    }

    public override void StartState()
    {
        // stops the agent
        m_StateMachine.SetAgentSpeed(0f);
        // sets its target as itself
        m_StateMachine.SetAgentTarget(m_StateMachine.EntityTransform.position);
        // will stop the update activity after the specified time
        m_WaitingCoroutine = m_StateMachine.StartCoroutine(StandInPlaceCountDown());
        
        base.StartState();
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        CheckForSightInput();
        CheckForHearingInput();

        m_StateMachine.RotateToTarget(m_StateMachine.Target);
    }

    private void CheckForHearingInput()
    {
        // if the player is heard, then move towards them
        if (m_Hearing.State == EntityHearing.HearingState.PlayerHeard)
        {
            m_Indicator.DisplayHearingIcon(true);
            m_StateMachine.StopCoroutine(m_WaitingCoroutine);
            m_StateMachine.SetNextState(StateName.MoveToPoint);
            m_StateMachine.StopState();
        }
    }

    private void CheckForSightInput()
    {
        if (m_Sight.State == EntitySight.SightState.PlayerInSight)
        {
            m_StateMachine.StopCoroutine(m_WaitingCoroutine);
            m_StateMachine.SetNextState(StateName.Scream);
            m_StateMachine.StopState();
        }
    }

    private IEnumerator StandInPlaceCountDown()
    {
        float standingDuration = Random.Range(m_MinWaitingTime, m_MaxWaitingTime);
        yield return new WaitForSeconds(standingDuration);
        m_StateMachine.SetNextState(StateName.Patrol);
        m_StateMachine.StopState();
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }
}
