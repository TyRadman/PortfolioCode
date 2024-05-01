using UnityEngine;
using UnityEngine.UI;

public class MainMenuStats : MonoBehaviour
{
    static public MainMenuStats Instance;
    public bool DisplayIntro = true;
    [SerializeField] private Text m_IntroductionNames;
    [SerializeField] private Text m_CreditsNames;
    [SerializeField] private bool m_DisplayBothNames = false;
    [TextArea(2, 4)] [SerializeField] private string m_TextWithOneName;
    [TextArea(2, 4)] [SerializeField] private string m_TextWithTwoNames;
    [TextArea(2, 4)] [SerializeField] private string m_OneNameCredits;
    [TextArea(2, 4)] [SerializeField] private string m_TwoNamesCredits;

    void Start()
    {
        Instance = this;

        if (m_DisplayBothNames)
        {
            m_CreditsNames.text = m_TwoNamesCredits;
            m_IntroductionNames.text = m_TextWithTwoNames;
        }
        else
        {
            m_CreditsNames.text = m_OneNameCredits;
            m_IntroductionNames.text = m_TextWithOneName;
        }
    }
}