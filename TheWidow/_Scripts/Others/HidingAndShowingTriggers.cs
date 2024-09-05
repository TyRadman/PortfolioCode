using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class HidingAndShowingTriggers : MonoBehaviour
{
    [SerializeField] private UnityEvent m_OnTriggerEvent;
    [SerializeField] private BoxCollider m_TriggerCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_OnTriggerEvent?.Invoke();
        }
    }
}
