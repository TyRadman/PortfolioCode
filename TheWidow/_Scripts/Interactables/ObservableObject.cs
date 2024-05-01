using UnityEngine;

public class ObservableObject : InteractionClass
{
    [SerializeField]
    [TextArea(2, 10)]
    private string[] m_ObservationMessages;
    [SerializeField]
    [TextArea(2, 10)]
    private string[] m_RandomObservationMessages;
    private bool m_MainMessagesTyped = false;

    protected override void Interact()
    {
        if (!m_MainMessagesTyped)
        {
            m_MainMessagesTyped = true;
            StartCoroutine(StoryTelling.DisplayMessages(m_ObservationMessages));
        }
        else
        {
            string[] randomMessage = { m_RandomObservationMessages[Random.Range(0, m_RandomObservationMessages.Length)] };
            StartCoroutine(StoryTelling.DisplayMessages(randomMessage));
        }
    }
}