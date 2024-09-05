using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State_TheWidow_Kill", menuName = Constants.WIDOW_STATES_DIRECTORY + "Kill")]
public class StateKillWidow : MachineState
{
    public override void StartState()
    {
        base.StartState();

        // rotate the enemy towards the player so that the animation plays well
        m_StateMachine.RotateToTarget(m_StateMachine.PlayerTransform.position, 1f);
        // create a fading screen
        FadingScreen.Instance.Fade(0.3f, 0.02f, 0.1f, Color.black);
        // call the game over function
        GameManager.Instance.GameOver();
        PlayerMovement.Instance.AllowMovement(false);
        PlayerStats.Instance.IsDead = true;
        // stopping the enemy
        m_StateMachine.Agent.enabled = false;
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        // rotate the enemy towards the player so that the animation plays well
        m_StateMachine.RotateToTarget(m_StateMachine.PlayerTransform.position, 1f);
    }

    public override void EndActivity()
    {
        base.EndActivity();


    }
}
