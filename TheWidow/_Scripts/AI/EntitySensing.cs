using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySensing : MonoBehaviour
{
    public static float StartDelay = 0.1f;
    private StatesEntity m_Entity;
    public DifficultyModifier.EnemyTag m_EnemyTag;
    [Tooltip("How many times should the senses be updated a second")]
    [SerializeField] private float m_RefreshRate = 10;
    public bool CanSee = true;
    public bool CanHear = true;
    private WaitForSeconds m_BetweenStatesWait;
    // values to fetch from the game manager
    public HearingDistances HearingDistances;
    [SerializeField] private float m_SightDistance;
    [SerializeField] private LayerMask m_SightMask;
    [Header("References")]
    [SerializeField] private Transform m_Caster;

    private void Awake()
    {
        m_Entity = GetComponent<StatesEntity>();
        m_RefreshRate = 1 / m_RefreshRate;
        m_BetweenStatesWait = new WaitForSeconds(m_RefreshRate);
    }

    private void Start()
    {
        initializeValuesFromManager();
        StartCoroutine(updateSenses());
    }

    IEnumerator updateSenses()
    {
        yield return new WaitForSeconds(StartDelay);

        while (m_Entity.IsActive)
        {
            if (m_Entity.AffectsIndicator)
            {
                m_Entity.Indicator.SetEyeIndicatorSize(m_Entity.DistanceToTarget(m_Entity.PlayerTransform.position));
            }
         
            sightProcess();
            HearingProcess(getHearingDistance());

            yield return m_BetweenStatesWait;
        }
    }

    // the process goes like this: check if anything prevents sight => cast a ray onto the player => check if player is seen => release all objects => trigger the sight event at the states entity
    private void sightProcess()
    {
        // the sight doesn't work if the player is dead, or if the enemy can't see, or if the enemy is killing the player
        if (PlayerStats.Instance.IsDead || !CanSee)
        {
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
                m_Entity.PlayerSeen(true);
                // if the player was holding an object, the onject is released
                PlayerObjectsInteraction.Instance.ReleaseObject();
                // notify the entity that the player is seen so that the right state is switched to
                m_Entity.TriggerSight();
                return;
            }

            m_Entity.PlayerSeen(false);
        }
    }

    #region Hearing Functionality
    public void HearingProcess(float _hearingDistance)
    {
        if (PlayerStats.Instance.IsDead || !CanHear || PlayerStats.Instance.IsHidden)
        {
            return;
        }

        if (m_Entity.DistanceToTarget(m_Entity.PlayerTransform.position) <= _hearingDistance)
        {
            m_Entity.TriggerHearing();
        }

        // feeds this bool to the hearing icon so that it triggers it if the value is true
        // m_Eye.HearingIconAlert(heard);
    }

    public void HearingProcess(float _hearingDistance, Vector3 _target)
    {
        if (PlayerStats.Instance.IsDead || !CanHear)
        {
            return;
        }

        if (m_Entity.DistanceToTarget(_target) <= _hearingDistance)
        {
            m_Entity.TriggerHearing();
        }

        // feeds this bool to the hearing icon so that it triggers it if the value is true
        // m_Eye.HearingIconAlert(heard);
    }
    #endregion

    private float getHearingDistance()
    {
        float hearingDistance = 0f;

        switch (PlayerMovement.Instance.CurrentState)
        {
            case PlayerMovement.States.hiding:
                {
                    hearingDistance = 0f;
                    break;
                }
            case PlayerMovement.States.crouching:
                {
                    hearingDistance = HearingDistances.CrouchingDistance;
                    break;
                }
            case PlayerMovement.States.running:
                {
                    hearingDistance = HearingDistances.RunningDistance;
                    break;
                }
            case PlayerMovement.States.standing:
                {
                    hearingDistance = 0f;
                    break;
                }
            case PlayerMovement.States.walking:
                {
                    hearingDistance = HearingDistances.WalkingDistance;
                    break;
                }
        }

        return hearingDistance;
    }

    #region Initializer
    private void initializeValuesFromManager()
    {
        // fetch a difficulty with the same tag as the entity's tag
        DifficultyModifier dif = GameManager.Instance.CurrentDifficulty;
        // set up values
        HearingDistances = dif.TheHearingDistances;
        m_SightDistance = dif.SightDistance;
    }
    #endregion
}
