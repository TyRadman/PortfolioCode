using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "State_TheWidow_Scream", menuName = Constants.WIDOW_STATES_DIRECTORY + "Scream")]
public class StateScreamWidow : MachineState
{
    [SerializeField] private float m_ScreamingDuration = 2f;
    [Range(0f, 0.7f)]
    [SerializeField] private float m_LookingSpeed = 0.2f;
    private EyeIndicator m_Indicator;

    public override void SetUp(IComponent entityComponents)
    {
        base.SetUp(entityComponents);
        m_Components = (TheWidowComponents)entityComponents;
        m_Indicator = m_Components.EyeIndicator;
    }

    public override void StartState()
    {
        base.StartState();

        m_Indicator.UpdateIdicator(EyeState.IsPlayerInSight);
        m_Indicator.UpdateIdicator(EyeState.OnPlayerSpotted);
        m_StateMachine.SetAgentSpeed(0f);
        m_StateMachine.SetAgentTarget(m_StateMachine.PlayerTransform.position);
        // stop the state after the screaming duration is over
        m_StateMachine.StartCoroutine(WaitForScreaming());
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        m_StateMachine.RotateToTarget(m_StateMachine.PlayerTransform.position, m_LookingSpeed);
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }

    private IEnumerator WaitForScreaming()
    {
        yield return new WaitForSeconds(m_ScreamingDuration);
        m_StateMachine.SetNextState(StateName.Chase);
        m_StateMachine.StopState();
    }
}
