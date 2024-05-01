using UnityEngine;

public class SecondFloorBLocker : MonoBehaviour
{
    [SerializeField] private string m_Message;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.TypeMessage(m_Message);
        }
    }
}