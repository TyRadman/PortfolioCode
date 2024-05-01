using UnityEngine;
using UnityEngine.UI;

public class TestingManager : MonoBehaviour
{
    public static TestingManager Instance;

    [Header("References")]
    [SerializeField] private GameObject m_PostProcessing;
    [SerializeField] private GameObject m_TestingWindow;
    [SerializeField] private Text m_LogMessage;
    [SerializeField] private GameObject m_MusicBox;
    [SerializeField] private Text m_PauseText;
    [Header("Values")]
    [SerializeField] private bool m_EnablePostProcessing;

    private void Awake()
    {
        Instance = this;
        m_TestingWindow.SetActive(false);
    }

    private void Start()
    {
        m_PostProcessing.SetActive(m_EnablePostProcessing);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            m_TestingWindow.SetActive(!m_TestingWindow.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_MusicBox.SetActive(!m_MusicBox.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_PauseText.enabled = !m_PauseText.enabled;
            Time.timeScale = m_PauseText.enabled ? 0f : 1f;
        }
    }

    public void Log(string _message)
    {
        m_LogMessage.text += $"[{Time.time}] {_message} \n";
    }
}
