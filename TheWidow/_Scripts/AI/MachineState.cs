using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineState : MonoBehaviour
{
    public enum StateName
    {
        None, Stand, Patrol, Hear, Scream, Chase, Kill
    }
    public enum ExternalTriggers
    {
        None, Sight, Hearing
    }

    public StateName StateTag;
    [SerializeField] private ExternalTriggers m_TriggeredBy;
    protected StatesEntity Entity;

    #region Virtual Functions
    public virtual void StartActivity()
    {
        Entity.Animation.Animate(StateTag);
        // print(SimpleFunctions.ColorText($"The start of the {StateTag} state", nameof(Color.green)));
    }

    public virtual void UpdateActivity()
    {
        // print(SimpleFunctions.ColorText($"The update of the {StateTag} state", nameof(Color.yellow)));
    }

    public virtual void EndActivity()
    {
        // print(SimpleFunctions.ColorText($"The end of the {StateTag} state", nameof(Color.red)));
    }

    /// <summary>
    /// Conditions that disables senses for this state
    /// </summary>
    /// <returns></returns>
    public virtual bool SensesDealBreaker()
    {
        return false;
    }
    #endregion

    #region Initializers
    public void AssignListeners()
    {
        // we assign listeners to the main events
        if(m_TriggeredBy == ExternalTriggers.Sight)
        {
            Entity.m_SightEvent = this;
        }
        else if(m_TriggeredBy == ExternalTriggers.Hearing)
        {
            Entity.m_HearingEvent = this;
        }
    }

    public void SetMutualReferences(StatesEntity _statesEntity)
    {
        Entity = _statesEntity;
    }
    #endregion
}
