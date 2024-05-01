using UnityEngine;
using System.Collections;

public class StateScreamWidow : MachineState
{
    [SerializeField] private float m_ScreamingDuration = 2f;
    [Range(0f, 0.7f)]
    [SerializeField] private float m_LookingSpeed = 0.2f;
    private Coroutine m_ScreamingWaitingCoroutine;

    public override void StartActivity()
    {
        base.StartActivity();

        Entity.Senses.CanHear = false;
        Entity.Senses.CanSee = false;
        Entity.SetAgentSpeed(0f);
        Entity.SetAgentTarget(Entity.PlayerTransform.position);
        // stop the state after the screaming duration is over
        m_ScreamingWaitingCoroutine = StartCoroutine(waitForScreaming());
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        Entity.RotateToTarget(Entity.PlayerTransform.position, m_LookingSpeed);
    }

    public override void EndActivity()
    {
        base.EndActivity();

        // if the state was ended by something related to senses, then we stop the coroutine that will move us to the next state
        if(m_ScreamingWaitingCoroutine != null)
        {
            StopCoroutine(m_ScreamingWaitingCoroutine);
            m_ScreamingWaitingCoroutine = null;
            return;
        }

        // start chasing after the screaming state is over
        Entity.PerfromNextState(StateName.Chase);
    }

    public override bool SensesDealBreaker()
    {
        return Entity.CheckCurrentState(StateName.Chase) || Entity.CheckCurrentState(StateName.Scream);
    }

    private IEnumerator waitForScreaming()
    {
        yield return new WaitForSeconds(m_ScreamingDuration);

        m_ScreamingWaitingCoroutine = null;
        // start chasing after the screaming state is over
        Entity.PerfromNextState(StateName.Chase);
    }
}
