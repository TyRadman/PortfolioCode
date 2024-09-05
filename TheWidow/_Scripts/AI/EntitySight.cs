using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySight : MonoBehaviour, IController
{
    public enum SightState
    {
        None = 0, PlayerInSight = 1, PlayerNotInSight = 2
    }

    /// <summary>
    /// Defines the current state of the entity's sight
    /// </summary>
    [field: SerializeField] public SightState State { get; private set; } = SightState.None;

    public static float StartDelay = 0.1f;
    private StatesEntity m_Entity;
    [Tooltip("How many times should the senses be updated a second")]
    [SerializeField] private float m_RefreshRate = 10;
    public bool CanSee = true;
    private WaitForSeconds m_BetweenStatesWait;
    private float m_SightDistance;
    [SerializeField] private LayerMask m_SightMask;
    [SerializeField] private Transform m_Caster;
    [SerializeField] private EyeIndicator m_Indicator;

    public void SetUp(IComponent component)
    {
        m_Entity = ((EntityComponents)component).Entity;
        m_SightDistance = GameManager.Instance.Settings.CurrentDifficulty.SightDistance;
    }

    public void Activate()
    {
        m_Indicator.UpdateIdicator(EyeState.OnEyeEnabled);
        StartCoroutine(RunSightProcess());
    }

    private IEnumerator RunSightProcess()
    {
        yield return new WaitForSeconds(StartDelay);

        while (m_Entity.IsActive)
        {
                m_Entity.Indicator.SetEyeIndicatorSize(m_Entity.GetDistanceToTarget(m_Entity.PlayerTransform.position));

            PerformSightProcess();
            yield return m_BetweenStatesWait;
        }
    }

    // the process goes like this: check if anything prevents sight => cast a ray onto the player => check if player is seen => release all objects => trigger the sight event at the states entity
    private void PerformSightProcess()
    {
        // the sight doesn't work if the player is dead, or if the enemy can't see, or if the enemy is killing the player
        if (PlayerStats.Instance.IsDead || !CanSee)
        {
            State = SightState.PlayerNotInSight;
            return;
        }

        m_Caster.transform.LookAt(m_Entity.PlayerTransform);

        // shoots a ray from the caster to the player
        Ray sightRay = new Ray(m_Caster.position, m_Caster.forward);

        if (Physics.Raycast(sightRay, out RaycastHit hit, m_SightDistance, m_SightMask))
        {
            // if the player is hit with the ray
            if (hit.collider.CompareTag("Player"))
            {
                State = SightState.PlayerInSight;

                PlayerStats.Instance.IsSeen = true;
                PlayerObjectsInteraction.Instance.ReleaseObject();
                return;
            }

            State = SightState.PlayerNotInSight;
            PlayerStats.Instance.IsSeen = false;
        }
    }

    public void Dispose()
    {
        StopAllCoroutines();
    }
}
