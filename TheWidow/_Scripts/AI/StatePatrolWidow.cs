using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "State_TheWidow_Patrol", menuName = Constants.WIDOW_STATES_DIRECTORY + "Patrol")]
public class StatePatrolWidow : MachineState
{
    private Vector3 m_Target;
    private EntityHearing m_Hearing;
    private const float STOPPING_DISTNACE = 3f;
    private EntitySight m_Sight;
    private float m_PatrolSpeed;
    private EyeIndicator m_Indicator;

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
        // set the speed of the entity
        m_StateMachine.SetAgentSpeed(m_PatrolSpeed);
        // select a random waypoint and set it as a target
        ChooseNewWayPoint();
        // play the patroling animation
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
            m_StateMachine.SetNextState(StateName.MoveToPoint);
            m_StateMachine.StopState();
        }
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }

    // chooses a way point different from the one chosen previously and makes sure its reachable by the enemy
    private void ChooseNewWayPoint()
    {
        // caches all waypoints except for the previously chosen point and the unreachable points
        List<Vector3> availableWayPoints = m_StateMachine.WayPoints.FindAll(wp => wp != m_Target && m_StateMachine.DestinationIsReachable(wp));
        m_Target = availableWayPoints[Random.Range(0, availableWayPoints.Count)];
        m_StateMachine.SetAgentTarget(m_Target);
    }

    // chooses a waypoint close to the player
    private void chooseCloseWayPoint()
    {
        // caches all waypoints except for the previously chosen point and the unreachable points
        List<Vector3> newWayPoints = m_StateMachine.WayPoints.FindAll(wp => wp != m_Target && m_StateMachine.DestinationIsReachable(wp));
        // selects the closest waypoint by ordering the waypoints according to the distance between them and the player and choosing the first one
        m_Target = newWayPoints.OrderBy(w => Vector3.Distance(w, m_StateMachine.PlayerTransform.position)).First();
        m_StateMachine.SetAgentTarget(m_Target);
    }
}
