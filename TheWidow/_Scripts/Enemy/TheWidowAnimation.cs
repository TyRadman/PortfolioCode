using UnityEngine;

public class TheWidowAnimation : MonoBehaviour
{
    public Animator Anim;
    private EnemyAIDecisions m_Enemy;
    public static TheWidowAnimation Instance;

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

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        m_Enemy = GetComponent<EnemyAIDecisions>();
    }

    // animates the enemy according to its state
    public void Animate(EnemyAIDecisions.EnemyActions _action)
    {
        // to make the walking realistic, we adjust the animation speed in accordance to the enemy's speed
        Anim.SetFloat(WALKING_SPEED, m_Enemy.Agent.speed);

        switch (_action)
        {
            case EnemyAIDecisions.EnemyActions.patrol:
                {
                    Anim.SetInteger(PATROL_NUMBER, Random.Range(0, 2));// the walking trigger is activated
                    Anim.SetBool(STAND, false);
                    Anim.SetFloat(WALKING_SPEED, m_Enemy.PatrolSpeed);
                    break;
                }
            case EnemyAIDecisions.EnemyActions.stand:
                {
                    Anim.SetBool(STAND, true);
                    break;
                }
            case EnemyAIDecisions.EnemyActions.chase:
                {
                    Anim.SetInteger(CHASE_NUMBER, Random.Range(0, 2));
                    Anim.SetBool(STAND, false);
                    Anim.SetTrigger(IS_CHASING);
                    Anim.SetFloat(RUNNING_SPEED, m_Enemy.ChaseSpeed);
                    break;
                }
            case EnemyAIDecisions.EnemyActions.scream:
                {
                    Anim.SetTrigger(IS_SCREAMING);
                    break;
                }
            case EnemyAIDecisions.EnemyActions.kill:
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