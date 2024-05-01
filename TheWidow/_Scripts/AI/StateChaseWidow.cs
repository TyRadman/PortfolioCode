using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChaseWidow : MachineState
{
    [SerializeField] private float m_CatchingDistance;
    [SerializeField] private Coroutine m_TimerToLosePlayer;
    
    public override void StartActivity()
    {
        base.StartActivity();

        // set the chasing speed
        Entity.SetAgentSpeed(Entity.Speeds.ChasingSpeed);
        // turn on vision again 
        Entity.Senses.CanSee = true;
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        #region If Player Is Seen
        if (!PlayerStats.Instance.IsHidden)
        {
            if (Vector3.Distance(Entity.EntityTransform.position, Entity.PlayerTransform.position) > m_CatchingDistance)
            {
                Entity.SetAgentTarget(Entity.PlayerTransform.position);
            }
            else
            {
                Entity.PerfromNextState(StateName.Kill);
                return;
            }
        }
        #endregion

        #region If Player Went Hidden
        // if the player is hidden then the enemy moves to the hiding spot
        else if (PlayerStats.Instance.IsHidden)
        {
            if (Vector3.Distance(Entity.EntityTransform.position, Entity.Target) > Entity.Agent.stoppingDistance)
            {
                Entity.SetAgentTarget(PlayerStats.Instance.HidingSpot.EnemyStandingPlace);
            }
            else
            {
                // once it reaches the hiding spot it increaases the number of time the player escaped using that spot
                // increases the number of times and once the limit is exceeded the hiding spot will no longer hide the player
                PlayerStats.Instance.HidingSpot.SpottedHiding();

                // the enemy will stand only if the player is hidden
                if (PlayerStats.Instance.IsHidden)
                {
                    Entity.PerfromNextState(StateName.Stand);
                    return;
                }
            }
        }
        #endregion

        #region If Player Is Out Of Sight
        // if the pllayer vanished from the enemy's sight
        if (!Entity.PlayerIsSeen)
        {
            // makes sure this is the first time the player is seen since last time he wasn't seen
            if (m_TimerToLosePlayer == null)
            {
                // changes the eye indicator mode to chasing
                // m_Eye.PlayerInSight(true);
                // starts a coroutine that starts a count down when the player is out of sight after which the enemy goes back to patroling
                m_TimerToLosePlayer = StartCoroutine(losingPlayer());
            }
        }
        // if he was spotted again while being chased
        else
        {
            if (m_TimerToLosePlayer != null)
            {
                StopCoroutine(m_TimerToLosePlayer);
                m_TimerToLosePlayer = null;
            }
        }
        #endregion
    }

    private IEnumerator losingPlayer()
    {
        yield return new WaitForSeconds(Entity.PlayerLosingTime);
        Entity.Indicator.UpdateIdicator(EyeState.Lost);
        Entity.Senses.CanHear = true;
        Entity.PerfromNextState(StateName.Stand);
    }
}
