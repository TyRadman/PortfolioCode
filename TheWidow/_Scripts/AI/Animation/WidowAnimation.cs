using System.Collections;
using UnityEngine;

public class WidowAnimation : EntityAnimation
{
    private const string WALKING_SPEED = "walkingSpeed";
    private const string PATROL_NUMBER = "patrolNumber";
    private const string STAND = "stand";
    private const string CHASE_NUMBER = "chaseNumber";
    private const string IS_CHASING = "isChasing";
    private const string RUNNING_SPEED = "runningSpeed";
    private const string IS_SCREAMING = "isScreaming";
    private const string IS_KILLING = "isKilling";

    /// <summary>
    /// Call this method after setting up the agent's speed
    /// </summary>
    /// <param name="_state">The current state of the entity.</param>
    public override void Animate(MachineState.StateName _state)
    {
        base.Animate(_state);
        // to make the walking realistic, we adjust the animation speed in accordance to the enemy's speed
        Anim.SetFloat(WALKING_SPEED, Entity.Agent.speed);

        switch (_state)
        {
            case MachineState.StateName.Patrol:
                {
                    Anim.SetInteger(PATROL_NUMBER, Random.Range(0, 2));// the walking trigger is activated
                    Anim.SetBool(STAND, false);
                    Anim.SetFloat(WALKING_SPEED, Entity.Agent.speed);
                    break;
                }
            case MachineState.StateName.MoveToPoint:
                {
                    Anim.SetInteger(PATROL_NUMBER, Random.Range(0, 2));// the walking trigger is activated
                    Anim.SetBool(STAND, false);
                    Anim.SetFloat(WALKING_SPEED, Entity.Agent.speed);
                    break;
                }
            case MachineState.StateName.Stand:
                {
                    Anim.SetBool(STAND, true);
                    break;
                }
            case MachineState.StateName.Chase:
                {
                    // choosing what chasing animation to play randomly
                    Anim.SetInteger(CHASE_NUMBER, Random.Range(0, 2));
                    // disabling standing so that we don't move from chasing to standing
                    Anim.SetBool(STAND, false);
                    // we trigger chasing
                    Anim.SetTrigger(IS_CHASING);
                    // we set the speed of the animation to match the speed of the enemy chasing speed
                    float runningSpeed = GameManager.Instance.Settings.CurrentDifficulty.ChasingSpeed;
                    Anim.SetFloat(RUNNING_SPEED, runningSpeed);
                    break;
                }
            case MachineState.StateName.Scream:
                {
                    Anim.SetTrigger(IS_SCREAMING);
                    break;
                }
            case MachineState.StateName.Kill:
                {
                    Anim.SetTrigger(IS_KILLING);
                    break;
                }
            case MachineState.StateName.Hear:
                {
                    Anim.SetInteger(PATROL_NUMBER, Random.Range(0, 2));// the walking trigger is activated
                    Anim.SetBool(STAND, false);
                    Anim.SetFloat(WALKING_SPEED, Entity.Agent.speed);
                    break;
                }
        }
    }
}