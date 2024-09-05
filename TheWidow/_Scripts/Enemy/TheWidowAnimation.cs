using UnityEngine;
using static MachineState;

public class TheWidowAnimation : MonoBehaviour
{
    public Animator Anim;
    //private EnemyAIDecisions m_Enemy;
    public static TheWidowAnimation Instance;
    private EntityComponents m_Components;

    #region Constants
    private const string WALKING_SPEED = "walkingSpeed";
    private const string PATROL_NUMBER = "patrolNumber";
    private const string STAND = "stand";
    private const string CHASE_NUMBER = "chaseNumber";
    private const string IS_CHASING = "isChasing";
    private const string RUNNING_SPEED = "runningSpeed";
    private const string IS_SCREAMING = "isScreaming";
    private const string IS_KILLING = "isKilling";
    #endregion

    public void SetUp(IController components)
    {
        m_Components = (EntityComponents)components;
    }

    // animates the enemy according to its state
    public void Animate(StateName state)
    {
        // to make the walking realistic, we adjust the animation speed in accordance to the enemy's speed
        Anim.SetFloat(WALKING_SPEED, m_Components.Entity.Agent.speed);

        switch (state)
        {
            case StateName.Patrol:
                {
                    Anim.SetInteger(PATROL_NUMBER, Random.Range(0, 2));// the walking trigger is activated
                    Anim.SetBool(STAND, false);
                    Anim.SetFloat(WALKING_SPEED, m_Components.Difficulty.WalkingSpeed);
                    break;
                }
            case StateName.Stand:
                {
                    Anim.SetBool(STAND, true);
                    break;
                }
            case StateName.Chase:
                {
                    Anim.SetInteger(CHASE_NUMBER, Random.Range(0, 2));
                    Anim.SetBool(STAND, false);
                    Anim.SetTrigger(IS_CHASING);
                    Anim.SetFloat(RUNNING_SPEED, m_Components.Difficulty.ChasingSpeed);
                    break;
                }
            case StateName.Scream:
                {
                    Anim.SetTrigger(IS_SCREAMING);
                    break;
                }
            case StateName.Kill:
                {
                    Anim.SetTrigger(IS_KILLING);
                    break;
                }
        }
    }

    public void DisableCulling()
    {
        Anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }
}