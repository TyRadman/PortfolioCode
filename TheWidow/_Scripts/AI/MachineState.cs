using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineState : ScriptableObject
{
    public enum StateName
    {
        None = 0, Stand = 1, Patrol = 2, Hear = 3, Scream = 4,
        Chase = 5, Kill = 6, MoveToPoint = 7
    }

    public StateName StateTag;
    protected StatesEntity m_StateMachine;
    protected EntityComponents m_Components;
    protected EnemyAudio m_Audio;

    public virtual void SetUp(IComponent entityComponents)
    {
        m_Components = (EntityComponents)entityComponents;
        m_StateMachine = m_Components.Entity;
        m_Audio = m_Components.Audio;
    }

    #region Virtual Functions
    public virtual void StartState()
    {
        m_StateMachine.Animation.Animate(StateTag);
        m_Audio.PlayAudio(StateTag);
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
    #endregion
}
