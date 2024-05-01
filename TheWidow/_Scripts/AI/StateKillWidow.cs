using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateKillWidow : MachineState
{
    public override void StartActivity()
    {
        base.StartActivity();

        // rotate the enemy towards the player so that the animation plays well
        Entity.RotateToTarget(Entity.PlayerTransform.position, 1f);
        // create a fading screen
        FadingScreen.Instance.Fade(0.3f, 0.02f, 0.1f, Color.black);
        // call the game over function
        GameManager.Instance.GameOver();
        PlayerMovement.Instance.AllowMovement(false);
        PlayerStats.Instance.IsDead = true;
        // stopping the enemy
        Entity.Agent.enabled = false;
    }

    public override void UpdateActivity()
    {
        base.UpdateActivity();

        // rotate the enemy towards the player so that the animation plays well
        Entity.RotateToTarget(Entity.PlayerTransform.position, 1f);
    }

    public override void EndActivity()
    {
        base.EndActivity();


    }
}
