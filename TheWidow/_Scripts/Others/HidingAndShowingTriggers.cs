using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HidingAndShowingTriggers : MonoBehaviour
{
    [SerializeField] private bool m_OnTriggerActivation = true;
    [SerializeField] private string m_EventName;
    [SerializeField] private GameObject m_ObjectToHide;
    [SerializeField] private GameObject m_ObjectToShow;
    [Header("Time variables")]
    [SerializeField] private float m_TriggerActivationTime = 0f;
    [Header("Flashing variables")]
    [SerializeField] private bool m_FlashScreen = false;
    [SerializeField] private Color m_Color;
    [SerializeField] private float m_Duration;

    private void Start()
    {
        EventsManager.Instance.AddEvent(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_OnTriggerActivation)
        {
            GetComponent<BoxCollider>().enabled = false;
            return;
        }

        GetComponent<BoxCollider>().enabled = false;
        Invoke(nameof(activateTrigger), m_TriggerActivationTime);
    }

    public void Activate()
    {
        GetComponent<BoxCollider>().enabled = false;
        Invoke(nameof(activateTrigger), m_TriggerActivationTime);
    }

    void activateTrigger()
    {
        if (m_FlashScreen)
        {
            FadingScreen.Instance.Fade(m_Duration, 0.5f, 0.5f, m_Color);
            Invoke(nameof(hideAndShow), m_Duration / 2f);
        }
        else
        {
            hideAndShow();
        }
    }

    private void hideAndShow()
    {
        m_ObjectToHide.SetActive(false);
        m_ObjectToShow.SetActive(true);
        gameObject.SetActive(false);
    }

    public string GetEventName()
    {
        return m_EventName;
    }
}
