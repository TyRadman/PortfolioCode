using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHearing : MonoBehaviour, IController
{
    public enum HearingState
    {
        None = 0, PlayerHeard = 1, PlayerNotHeard = 2
    }

    /// <summary>
    /// Defines the current state of the entity's hearing
    /// </summary>
    [field: SerializeField] public HearingState State { get; private set; } = HearingState.None;
    [SerializeField] private float m_RefreshRate = 10;
    private WaitForSeconds m_BetweenStatesWait;
    private StatesEntity m_Entity;
    public Vector3 HearingTarget { get; private set; }
    public bool CanHear { get; private set; } = true;
    public HearingDistances HearingDistances { get; private set; }
    public System.Action OnHeard;
    private Vector3 m_LastNoisePosition;

    public void SetUp(IComponent component)
    {
        m_Entity = ((EntityComponents)component).Entity;
        HearingDistances = GameManager.Instance.Settings.CurrentDifficulty.TheHearingDistances;
    }

    public void StartHearing()
    {
        StartCoroutine(RunHearingProcess());
    }

    private IEnumerator RunHearingProcess()
    {
        while (m_Entity.IsActive)
        {
            m_Entity.Indicator.SetEyeIndicatorSize(m_Entity.GetDistanceToTarget(m_Entity.PlayerTransform.position));
            PerformHearing(GetHearingDistance(), m_Entity.PlayerTransform.position);
            yield return m_BetweenStatesWait;
        }
    }

    public void PerformHearing(float _hearingDistance, Vector3 _target)
    {
        if (PlayerStats.Instance.IsDead || !CanHear || PlayerStats.Instance.IsHidden)
        {
            State = HearingState.PlayerNotHeard;
            return;
        }

        if (m_Entity.GetDistanceToTarget(_target) <= _hearingDistance)
        {
            State = HearingState.PlayerHeard;
            HearingTarget = _target;
            m_LastNoisePosition = _target;
        }
        else if (State != HearingState.PlayerNotHeard)
        {
            State = HearingState.PlayerNotHeard;
        }
    }

    public bool HasHeardNoise()
    {
        return State == HearingState.PlayerHeard;
    }

    public Vector3 GetLastNoisePosition()
    {
        return m_LastNoisePosition;
    }

    private float GetHearingDistance()
    {
        float hearingDistance = 0f;
        hearingDistance = HearingDistances.Distances.Find(d => 
        d.State == PlayerMovement.Instance.CurrentState).Distance;
        return hearingDistance;
    }

    public void EnableHearing(bool enable)
    {
        CanHear = enable;
    }


    public void Activate()
    {
        StartHearing();
    }

    public void Dispose()
    {
        StopAllCoroutines();
    }
}
