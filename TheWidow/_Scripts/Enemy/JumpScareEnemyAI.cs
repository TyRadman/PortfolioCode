using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class JumpScareEnemyAI : MonoBehaviour
{
    // so far there are two types of jumpscares
    public enum JumpScareType
    {
        Standing, Crawling
    }

    [SerializeField] private Animator m_Anim;
    private NavMeshAgent m_Agent;
    [SerializeField] private float m_CrawlingSpeed = 12f;
    [SerializeField] private float m_StandingDuration = 6f;

    #region Constants
    private const string JUMP_SCARE_BOOL = "jumpScare";
    private const string JUMP_SCARE_STANDING_BOOL = "jumpScareStanding";
    private const string CRAWLING_SPEED = "crawlingJumpScareSpeed";
    private const string JUMPSCARE_AUDIO = "JumpScareEntry";
    private const string RUNNING_JUMPSCARE_AUDIO = "RunningJumpScare";
    #endregion

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        // m_Anim.SetBool(JUMP_SCARE_BOOL, true);
    }

    public void Activate(JumpScare _jumpscareInfo)
    {
        PlayerMovement.Instance.AllowedToRun = false;
        var type = _jumpscareInfo.GetJumpScareType();

        switch (type)
        {
            case JumpScareType.Standing:
                {
                    standing();
                    break;
                }
            case JumpScareType.Crawling:
                {
                    crawling(_jumpscareInfo.GetEndingPosition());
                    break;
                }
        }
    }

    // the functionality of the standing jumpscare where the enemy just stands with some scary music staring at the player before tilting its heading and disappearing
    private void standing()
    {
        // makes the screen flicker
        FadingScreen.Instance.FlickeryScreen(Color.black, m_StandingDuration, 0.7f);
        // zeros the speed of the widow so that she stands in place
        m_Agent.speed = 0f;
        // so that she performs the standing animation
        m_Anim.SetBool(JUMP_SCARE_STANDING_BOOL, true);
        // so that she performs jumpscare animations rather than normal animations
        m_Anim.SetBool(JUMP_SCARE_BOOL, true);
        // plays the jumpscare audio
        AudioManager.Instance.PlayAudio(JUMPSCARE_AUDIO, null, true);
        // set the rotation of the enemy to face the player
        StartCoroutine(lookAtPlayer());
        // stops the rotation update when the jumpscare is over
        Invoke(nameof(turnOffJumpScare), m_StandingDuration);
    }

    // in case the player got too close
    public void StopJumpScares()
    {
        FadingScreen.Instance.StopFlickeryScreen();
        CancelInvoke();
        StopAllCoroutines();
        turnOffJumpScare();
    }

    IEnumerator lookAtPlayer()
    {
        var time = 0f;

        while (time < m_StandingDuration)
        {
            time += Time.deltaTime;
            // the enemy will always stare at the enemy as long as it's standing
            transform.LookAt(PlayerMovement.Instance.transform);        

            yield return null;
        }
    }

    // when the jump scare is over
    private void turnOffJumpScare()
    {
        // the player is not allowed to run when the jumpscare is played. After it end, the player is allowed to run once again
        PlayerMovement.Instance.AllowedToRun = true;
    }

    private void crawling(Vector3 _target)
    {
        AudioManager.Instance.PlayAudioWithDelay(RUNNING_JUMPSCARE_AUDIO, null, true, 0.5f);
        m_Agent.speed = m_CrawlingSpeed;
        m_Anim.SetBool(JUMP_SCARE_BOOL, true);
        m_Anim.SetBool(JUMP_SCARE_STANDING_BOOL, false);
        m_Anim.SetFloat(CRAWLING_SPEED, m_CrawlingSpeed);
        m_Agent.SetDestination(_target);
        StartCoroutine(checkForTarget());
    }

    // interrupts the crawling jumpscare if the player gets close
    IEnumerator checkForTarget()
    {
        while (Vector3.Distance(transform.position, m_Agent.destination) > m_Agent.stoppingDistance)
        {
            yield return null;
        }

        turnOffJumpScare();
    }
}
