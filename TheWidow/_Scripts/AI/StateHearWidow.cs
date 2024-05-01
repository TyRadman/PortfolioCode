using System.Collections;
using UnityEngine;

public class StateHearWidow : MachineState
{
    public override void StartActivity()
    {
        // set the speed of the entity
        Entity.SetAgentSpeed(Entity.Speeds.PatrolSpeed);
        Entity.SetAgentTarget(Entity.PlayerTransform.position);

        base.StartActivity();
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        if ((Entity.EntityTransform.position - Entity.Target).sqrMagnitude > 3) // if the enemy is not close enough to the way point
        {
            Entity.SetAgentTarget(Entity.Target);
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
}