using UnityEngine;

// the tigger class of jumpscares
public class Trigger : MonoBehaviour
{
    public enum triggerType_enum
    {
        startJumpScare,
        endJumpScare,
    }

    public triggerType_enum TriggerType;
    private JumpScare m_JumpScare;

    private void Awake()
    {
        m_JumpScare = transform.parent.parent.GetComponent<JumpScare>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TriggerType == triggerType_enum.startJumpScare)
            {
                m_JumpScare.TriggerToStart();
            }
            else
            {
                m_JumpScare.TriggerToEnd();
            }
        }
    }
}