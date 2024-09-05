using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour, IController
{
    #region Constants
    private const string PATROL_SOUND = "WidowPatrolVoice";
    private const string SCREAM_SOUND = "EnemyScreamVoice";
    private const string CHASE_SOUND = "EnemyChaseVoice";
    private const string KILL_SOUND = "EnemyKillVoice";
    #endregion

    private Coroutine m_PatrolVoicesCoroutine;
    [SerializeField] private float m_MinTimeBetweenAudio = 3f;
    [SerializeField] private float m_MaxTimeBetweenAudio = 15f;
    private bool m_IsHostile = false;

    #region Enemy voice
    public void PlayAudio(MachineState.StateName _state)
    {
        if (_state == MachineState.StateName.Patrol || _state == MachineState.StateName.Stand)
        {
            m_IsHostile = false;

            if (m_PatrolVoicesCoroutine == null)
            {
                m_PatrolVoicesCoroutine = StartCoroutine(PlayEnemyVoiceProcess());
            }

            return;
        }
        else
        {
            if (m_PatrolVoicesCoroutine != null)
            {
                StopCoroutine(m_PatrolVoicesCoroutine);
            }

            m_IsHostile = true;
        }

        if (_state == MachineState.StateName.Scream)
        {
            AudioManager.Instance.PlayEnemyAudio(SCREAM_SOUND, true);
        }
        else if (_state == MachineState.StateName.Chase)
        {
            AudioManager.Instance.PlayEnemyAudio(CHASE_SOUND, true);
        }
        else if (_state == MachineState.StateName.Kill)
        {
            AudioManager.Instance.PlayEnemyAudio(KILL_SOUND, true);
        }
    }

    private IEnumerator PlayEnemyVoiceProcess()
    {
        while (!m_IsHostile)
        {
            float waitTime = Random.Range(m_MinTimeBetweenAudio, m_MaxTimeBetweenAudio);
            AudioManager.Instance.PlayEnemyAudio(PATROL_SOUND, false);
            yield return new WaitForSeconds(waitTime);
        }
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

    public void SetUp(IComponent component)
    {
    }

    public void Activate()
    {
    }

    public void Dispose()
    {
    }
    #endregion
}