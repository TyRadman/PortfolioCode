using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatePatrolWidow : MachineState
{
    private Vector3 m_Target;

    public override void StartActivity()
    {
        // set the speed of the entity
        Entity.SetAgentSpeed(Entity.Speeds.PatrolSpeed);
        // select a random waypoint and set it as a target
        chooseDifferentWayPoint();
        // play the patroling animation
        base.StartActivity();
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        if ((Entity.EntityTransform.position - m_Target).sqrMagnitude > 3) // if the enemy is not close enough to the way point
        {
            Entity.SetAgentTarget(m_Target);
        }
        else
        {
            Entity.PerfromNextState(StateName.Stand);
        }
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }

    // chooses a way point different from the one chosen previously and makes sure its reachable by the enemy
    private void chooseDifferentWayPoint()
    {
        // caches all waypoints except for the previously chosen point and the unreachable points
        List<Vector3> availableWayPoints = Entity.WayPoints.FindAll(wp => wp != m_Target && Entity.DestinationIsReachable(wp));
        m_Target = availableWayPoints[Random.Range(0, availableWayPoints.Count)];
        Entity.SetAgentTarget(m_Target);
    }

    // chooses a waypoint close to the player
    private void chooseCloseWayPoint()
    {
        // caches all waypoints except for the previously chosen point and the unreachable points
        List<Vector3> newWayPoints = Entity.WayPoints.FindAll(wp => wp != m_Target && Entity.DestinationIsReachable(wp));
        // selects the closest waypoint by ordering the waypoints according to the distance between them and the player and choosing the first one
        m_Target = newWayPoints.OrderBy(w => Vector3.Distance(w, Entity.PlayerTransform.position)).First();
        Entity.SetAgentTarget(m_Target);
    }
}
