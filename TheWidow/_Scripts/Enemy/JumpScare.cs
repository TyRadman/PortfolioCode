using UnityEngine;

// this class is more of an event. A jumpscare event that has different parameters and types
public class JumpScare : MonoBehaviour
{
    [SerializeField] private JumpScareEnemyAI.JumpScareType m_JumpScareType;
    [SerializeField] private Transform m_StartingPosition;
    [SerializeField] private Transform m_EndingPosition;

    // the trigger which starts the jumpscare
    public void TriggerToStart()
    {
        // the trigger is turned off
        transform.GetChild(1).gameObject.SetActive(false);
    }

    // the trigger that ends the jumpscare if the player got too close to the enemy
    public void TriggerToEnd()
    {
        // the ending trigger is turned off
        transform.GetChild(2).gameObject.SetActive(false);
        // the whole jumpscare expires
        gameObject.SetActive(false);
    }

    public Vector3 GetStartingPosition()
    {
        return m_StartingPosition.position;
    }

    public Vector3 GetEndingPosition()
    {
        return m_EndingPosition.position;
    }

    public JumpScareEnemyAI.JumpScareType GetJumpScareType()
    {
        return m_JumpScareType;
    }
}