using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Space]
    [Header("Start Menu Vars:")]
    static public MainMenuManager Instance;
    [SerializeField] private Animator m_Anim;
    public GameObject OptionsAndTipsMenu;
    public GameObject OptionsWindow;
    public GameObject TipsWindow;
    public Transform TableLight;
    public GameObject SkipIntroText;
    [SerializeField] private Text SettingsNote;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider ContrastSlider;
    public Slider DifficultySlider;
    public VolumeProfile PostProcessing;
    [SerializeField] private AudioMixer MainAudioMixer;
    private Coroutine m_SkippingButton;
    private const string NAME_OF_MENU_ANIMATION = "idle menu";
    [Header("Tips variables")]
    private int m_TipsIndex = 0;
    [SerializeField] private Image m_TipsImage;
    [SerializeField] private Text m_TipsText;

    void Awake()
    {
        InitiatePlayerPrefs();
     
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        CursorState(true);
        m_Anim.SetBool("startIntro", true);
        ApplyDefaults();
        m_SkippingButton = StartCoroutine(startingInput());
    }

    public static void CursorState(bool _visible)
    {
        Cursor.visible = _visible;
        Cursor.lockState = _visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    #region Skip Button Functionality
    IEnumerator startingInput()
    {
        while (m_Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != NAME_OF_MENU_ANIMATION)
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(skipButton());
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_Anim.SetBool("skip", true);
                StopCoroutine(m_SkippingButton);
            }

            yield return null;
        }

        StopCoroutine(m_SkippingButton);
    }

    IEnumerator skipButton()
    {
        SkipIntroText.SetActive(true);
        yield return new WaitForSeconds(2f);
        SkipIntroText.SetActive(false);
    }
    #endregion

    #region Button functionalities

    public void PlayButton() => FindObjectOfType<LoadingScreen>().StartLoadingScreen(1);

    public void CreditsButton() => m_Anim.SetTrigger("toCredits");

    public void BackFromCreditsButton() => m_Anim.SetTrigger("fromCredits");

    public void ExitToDesktopButton() => Application.Quit();

    public void refreshSliders(float _music, float _sfx, float _difficulty, float _contrast)
    {
        MusicSlider.value = _music;
        SfxSlider.value = _sfx;
        DifficultySlider.value = _difficulty;
        ContrastSlider.value = _contrast;
    }

    public void saveSlidersValues(float _music, float _sfx, int _difficulty, float _contrast)
    {
        PlayerPrefs.SetFloat(Keys.Settings_Music, _music);
        PlayerPrefs.SetFloat(Keys.Settings_SFX, _sfx);
        PlayerPrefs.SetInt(Keys.Settings_Difficulty, _difficulty);
        PlayerPrefs.SetFloat(Keys.Settings_Brightness, _contrast);
    }

    public void ChangeMusicVolume(float _volume) => MainAudioMixer.SetFloat("musicVolume", _volume);

    public void ChangeSfxVolume(float _volume) => MainAudioMixer.SetFloat("sfxVolume", _volume);

    public void DefaultButton()
    {
        refreshSliders(0f, 0f, 0, 1f);
        saveSlidersValues(0f, 0f, 0, 1f);
    }

    public void ToOptionsButton()
    {
        m_Anim.SetTrigger("toOptions");
        StartCoroutine(displayWindow(m_Anim.GetCurrentAnimatorClipInfo(0).Length, true));
        refreshSliders(PlayerPrefs.GetFloat(Keys.Settings_Music), PlayerPrefs.GetFloat(Keys.Settings_SFX),
            PlayerPrefs.GetInt(Keys.Settings_Difficulty), PlayerPrefs.GetFloat(Keys.Settings_Brightness));
    }

    public void ToTipsButton()
    {
        m_Anim.SetTrigger("toOptions");
        StartCoroutine(displayWindow(m_Anim.GetCurrentAnimatorClipInfo(0).Length, false));
    }

    public void BackFromOptionsAndTipsButton()
    {
        m_Anim.SetTrigger("fromOptions");                                                                            // camera animation
        StartCoroutine(backFromOptionsAndTips());                                                                    // smoothly removing the UI
        saveSlidersValues(MusicSlider.value, SfxSlider.value, (int)DifficultySlider.value, ContrastSlider.value);    // saves the values that were modified
    }

    IEnumerator displayWindow(float _duration, bool _options)
    {
        yield return new WaitForSeconds(_duration);
        OptionsAndTipsMenu.SetActive(true);

        if (_options)
        {
            OptionsWindow.SetActive(true);
            TipsWindow.SetActive(false);
        }
        else
        {
            setTipsInfo(SharedManager.Instance.GetTipWithIndex(m_TipsIndex));
            TipsWindow.SetActive(true);
            OptionsWindow.SetActive(false);
        }
    }

    IEnumerator backFromOptionsAndTips()
    {
        var menuAnim = OptionsAndTipsMenu.GetComponent<Animator>();
        menuAnim.SetTrigger("out");
        var duration = menuAnim.GetCurrentAnimatorClipInfo(0).Length;

        yield return new WaitForSeconds(duration);

        OptionsAndTipsMenu.SetActive(false);
    }

    public void ChangeContrast(float _value)
    {
        if (PostProcessing.TryGet(out Exposure effect))
        {
            effect.fixedExposure.value = 2f - _value * 2f;
            PlayerPrefs.SetFloat(Keys.Settings_Brightness, _value);
        }
    }

    void ApplyDefaults()
    {
        refreshSliders(PlayerPrefs.GetFloat(Keys.Settings_Music), PlayerPrefs.GetFloat(Keys.Settings_SFX),
               PlayerPrefs.GetInt(Keys.Settings_Difficulty), PlayerPrefs.GetFloat(Keys.Settings_Brightness));
        MainAudioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat(Keys.Settings_Music));
        MainAudioMixer.SetFloat("sfxVolume", PlayerPrefs.GetFloat(Keys.Settings_SFX));
    }
    #endregion

    void InitiatePlayerPrefs()
    {
        if (PlayerPrefs.GetString("FirstTime") == "")
        {
            PlayerPrefs.SetString("FirstTime", "true");
            DefaultButton();
            PlayerPrefs.SetFloat(Keys.Settings_MouseSensitivity, 2);
        }
    }

    void setTipsInfo(Tips.Tip _tip)
    {
        m_TipsImage.sprite = _tip.Image;
        m_TipsText.text = _tip.Info;
    }

    public void NextTipsButton()
    {
        m_TipsIndex++;

        if (m_TipsIndex > SharedManager.Instance.GetTipsNum() - 1)
        {
            m_TipsIndex = 0;
        }

        setTipsInfo(SharedManager.Instance.GetTipWithIndex(m_TipsIndex));
    }

    public void PreviousTipsButton()
    {
        m_TipsIndex--;

        if (m_TipsIndex < 0)
        {
            m_TipsIndex = SharedManager.Instance.GetTipsNum() - 1;
        }

        setTipsInfo(SharedManager.Instance.GetTipWithIndex(m_TipsIndex));
    }
}