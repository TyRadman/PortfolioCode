using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    #region Constants
    private const string PATROL_SOUND = "WidowPatrolVoice";
    private const string SCREAM_SOUND = "EnemyScreamVoice";
    private const string CHASE_SOUND = "EnemyChaseVoice";
    private const string KILL_SOUND = "EnemyKillVoice";
    #endregion

    private EnemyAIDecisions m_TheWidow;
    private Coroutine m_EnemyVoice;
    [SerializeField] private float m_MinTimeBetweenAudio = 3f;
    [SerializeField] private float m_MaxTimeBetweenAudio = 15f;

    public void Initialize()
    {
        m_TheWidow = GetComponent<EnemyAIDecisions>();
        // assigns listeners to the enemy as it changes its state
        m_TheWidow.StateChanged += PlayEnemyAudio;
        m_TheWidow.StateChanged += enemyPatrolVoice;
    }

    #region Enemy voice
    void enemyPatrolVoice(EnemyAIDecisions.EnemyActions _action)
    {
        if (_action == EnemyAIDecisions.EnemyActions.patrol || _action == EnemyAIDecisions.EnemyActions.stand)
        {
            AudioManager.Instance.PlayEnemyAudio(PATROL_SOUND, false);
            // another way to create a loop
            m_EnemyVoice = StartCoroutine(enemyVoice(_action, Random.Range(m_MinTimeBetweenAudio, m_MaxTimeBetweenAudio)));
        }
        else if (_action == EnemyAIDecisions.EnemyActions.scream)
        {
            if (m_EnemyVoice != null)
            {
                StopCoroutine(m_EnemyVoice);
            }

            AudioManager.Instance.PlayEnemyAudio(SCREAM_SOUND, true);
        }
        else if (_action == EnemyAIDecisions.EnemyActions.chase)
        {
            AudioManager.Instance.PlayEnemyAudio(CHASE_SOUND, true);
        }
        else if (_action == EnemyAIDecisions.EnemyActions.kill)
        {
            AudioManager.Instance.PlayEnemyAudio(KILL_SOUND, true);
        }
    }

    IEnumerator enemyVoice(EnemyAIDecisions.EnemyActions _action, float _time)
    {
        yield return new WaitForSeconds(_time);
        enemyPatrolVoice(_action);
    }
    #endregion

    #region Chasing music
    void PlayEnemyAudio(EnemyAIDecisions.EnemyActions _action)
    {
        if (_action == EnemyAIDecisions.EnemyActions.scream)
        {
            AudioManager.Instance.PlayChasingMusic();
        }
        else if (_action == EnemyAIDecisions.EnemyActions.stand)
        {
            AudioManager.Instance.PlayBGMusicAfterVollion();
        }
    }
    #endregion
}