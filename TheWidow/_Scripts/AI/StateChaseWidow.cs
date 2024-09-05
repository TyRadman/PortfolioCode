using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State_TheWidow_Chase", menuName = Constants.WIDOW_STATES_DIRECTORY + "Chase")]
public class StateChaseWidow : MachineState
{
    [SerializeField] private float m_CatchingDistance = 2f;
    [SerializeField] private Coroutine m_TimerToLosePlayer;
    private float m_ChasingSpeed = 3f;
    private EntitySight m_Sight;
    private float m_Timer = 0f;
    private float m_PlayerLosingTime = 5f;
    private EyeIndicator m_Indicator;

    public override void SetUp(IComponent entityComponents)
    {
        base.SetUp(entityComponents);
        m_Components = (EntityComponents)entityComponents;
        m_Indicator = m_Components.Entity.Indicator;
        m_Sight = m_Components.Sight;
        m_ChasingSpeed = m_Components.Difficulty.ChasingSpeed;
        m_PlayerLosingTime = m_Components.Difficulty.LosingPlayerTime;
        m_CatchingDistance = 3f;
    }

    public override void StartState()
    {
        base.StartState();

        m_Indicator.EnableIsChasing(true);
        // set the chasing speed
        m_StateMachine.SetAgentSpeed(m_ChasingSpeed);
        // turn on vision again 
        m_Components.Sight.CanSee = true;
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();
        m_Timer += Time.deltaTime;

        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (m_Timer < EntityComponents.REFRESH_RATE)
        {
            return;
        }

        m_Timer = 0f;

        if (!PlayerStats.Instance.IsHidden)
        {
            if (Vector3.Distance(m_StateMachine.EntityTransform.position, m_StateMachine.PlayerTransform.position) > m_CatchingDistance)
            {
                m_StateMachine.SetAgentTarget(m_StateMachine.PlayerTransform.position);
            }
            else
            {
                m_StateMachine.SetNextState(StateName.Kill);
                m_StateMachine.StopState();
                return;
            }

            if (m_Sight.State == EntitySight.SightState.PlayerNotInSight)
            {
                m_Indicator.UpdateIdicator(EyeState.IsPlayerOutOfSight);

                // makes sure this is the first time the player is seen since last time he wasn't seen
                if (m_TimerToLosePlayer == null)
                {
                    m_TimerToLosePlayer = m_StateMachine.StartCoroutine(LosingPlayerProcess());
                }
            }
            // if he was spotted again while being chased
            else
            {
                m_Indicator.UpdateIdicator(EyeState.IsPlayerInSight);

                if (m_TimerToLosePlayer != null)
                {
                    m_StateMachine.StopCoroutine(m_TimerToLosePlayer);
                    m_TimerToLosePlayer = null;
                }
            }
        }
        else if (PlayerStats.Instance.IsHidden)
        {
            if (Vector3.Distance(m_StateMachine.EntityTransform.position, m_StateMachine.Target) > m_CatchingDistance)
            {
                m_StateMachine.SetAgentTarget(PlayerStats.Instance.HidingSpot.EnemyStandingPlace);
            }
            else
            {
                PlayerStats.Instance.HidingSpot.IncreaseSpottingTimes();

                // the enemy will stand only if the player is hidden
                if (PlayerStats.Instance.IsHidden)
                {
                    m_StateMachine.SetNextState(StateName.Stand);
                    m_StateMachine.StopState();
                    return;
                }
            }
        }
    }

    private IEnumerator LosingPlayerProcess()
    {
        yield return new WaitForSeconds(m_PlayerLosingTime);
        m_Indicator.UpdateIdicator(EyeState.OnPlayerLost);
        m_Indicator.EnableIsChasing(false);
        m_Components.Hearing.EnableHearing(true);
        m_StateMachine.SetNextState(StateName.Stand);
        m_StateMachine.StopState();
    }
}
