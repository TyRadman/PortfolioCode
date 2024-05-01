using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateStandWidow : MachineState
{
    [SerializeField] private float m_MinWaitingTime = 2f;
    [SerializeField] private float m_MaxWaitingTime = 5f;
    private Coroutine m_WaitingCoroutine;

    public override void StartActivity()
    {
        // stops the agent
        Entity.SetAgentSpeed(0f);
        // sets its target as itself
        Entity.SetAgentTarget(Entity.EntityTransform.position);
        // will stop the update activity after the specified time
        m_WaitingCoroutine = StartCoroutine(standInPlace());
        
        base.StartActivity();
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        Entity.RotateToTarget(Entity.Target);
    }

    public override void EndActivity()
    {
        base.EndActivity();

        // making sure that we end the invoke in case this state was left before it was over
        if(m_WaitingCoroutine != null)
        {
            StopCoroutine(m_WaitingCoroutine);
            m_WaitingCoroutine = null;
            return;
        }

        Entity.PerfromNextState(StateName.Patrol);
    }

    private IEnumerator standInPlace()
    {
        yield return new WaitForSeconds(Random.Range(m_MinWaitingTime, m_MaxWaitingTime));

        Entity.PerfromNextState(StateName.Patrol);
        m_WaitingCoroutine = null;
    }
}
