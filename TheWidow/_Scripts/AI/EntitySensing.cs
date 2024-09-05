using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySensing : MonoBehaviour, IController
{
    public static float StartDelay = 0.1f;
    private StatesEntity m_Entity;
    [Tooltip("How many times should the senses be updated a second")]
    [SerializeField] private float m_RefreshRate = 10;
    public bool CanSee = true;
    private WaitForSeconds m_BetweenStatesWait;
    [SerializeField] private float m_SightDistance;
    [SerializeField] private LayerMask m_SightMask;
    [Header("References")]
    [SerializeField] private Transform m_Caster;
    public EntityHearing Hearing { get; private set; }
    public EntitySight Sight { get; private set; }
    private EntityComponents m_Compnents;

    public void SetUp(IComponent component)
    {
        m_Compnents = (EntityComponents)component;
        m_Entity = m_Compnents.Entity;

        m_RefreshRate = 1 / m_RefreshRate;
        m_BetweenStatesWait = new WaitForSeconds(m_RefreshRate);

        Hearing.SetUp(component);

    }

    #region Initializer
    


    public void Activate()
    {
    }

    public void Dispose()
    {
    }
    #endregion
}
