using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    // constants
    private const float MIN_YAW_VALUE = 2f, MAX_YAW_VALUE = 4f;
    private const float MIN_BRIGHTNESS = 0f, MAX_BRIGHTNESS = -1.5f;        // Min when the slider equals zero and max when one
    // private const float 

    #region Delegates
    public delegate void OnDifficultyChanged(float _difficulty);
    public event OnDifficultyChanged E_OnDifficultyChanged = delegate { };
    #endregion

    #region Time calculation
    public float TotalTime = 0f;
    private double m_Time_seconds = 0f;
    private int m_Time_minutes = 0;
    private int m_Time_hours = 0;
    [SerializeField] public Text m_Timetext;
    #endregion

    #region Player stats
    [SerializeField] private Text m_MedicineText;
    #endregion

    static public bool IsPaused = false;
    private GameObject m_ActivatedPanel;
    [SerializeField] private Canvas m_GamePlayCanvas;
    [SerializeField] private Canvas m_PauseMenuCanvas;
    [SerializeField] private GameObject m_MainPauseMenuButtons;
    [SerializeField] private Text m_PauseMenuStatusText;
    [SerializeField] private GameObject m_PopUpContainer;

    [Header("Shared vars:")]
    [Header("Tips Variables:")]
    [SerializeField] private GameObject m_TipsMenu;
    public Image TipImage;
    public Text TipText;
    public int CurrentIndex = 0;
    [Header("Settings variables:")]
    [SerializeField] private GameObject m_SettingsMenu;
    public AudioMixer MainAudioMixer;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider ContrastSlider;
    public Slider MouseSensitivitySlider;

    public VolumeProfile PostProcessingProfile;
    public float Contrast;
    public Text SettingsNote;
    [TextArea] public string ContrastNote;
    [TextArea] public string DifficultyNote;
    [TextArea] public string DifficultyWarningNote;

    protected void Start()
    {
        m_ActivatedPanel = m_MainPauseMenuButtons;
        LoadStartSettings();
        // subscribtions
        PlayerStats.Instance.OnMedicineCollected += IncreaseMedicine;
        // to instantiate values
        IncreaseMedicine();
    }

    virtual public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.GameIsOver && !Inventory.Instance.IsOpened)
        {
            if (m_ActivatedPanel == m_MainPauseMenuButtons)
            {
                // if the game is paused
                if (!m_PauseMenuCanvas.enabled)
                {
                    togglePause(true, CursorLockMode.Confined, 0f);
                }
                else // if the game is player after pausing
                {
                    togglePause(false, CursorLockMode.Locked, 1f);
                }
            }
            else
            {
                if (m_ActivatedPanel == m_TipsMenu)
                {
                    BackFromTipsButton();
                }
                else if (m_ActivatedPanel == m_SettingsMenu)
                {
                    BackFromSettingsButton();
                }
            }
        }
    }

    public void ResumeButton() => togglePause(false, CursorLockMode.Locked, 1f);


    #region Tips button functions
    public void TipsButton()
    {
        m_PopUpContainer.SetActive(true);
        m_TipsMenu.SetActive(true);
        m_MainPauseMenuButtons.SetActive(false);
        m_ActivatedPanel = m_TipsMenu;
        m_PauseMenuStatusText.text = "Tips";
        // TipText.text = SharedManager.Instance.GameTips[0];
        // TipImage.sprite = SharedManager.Instance.GameTipsImages[0];
        CurrentIndex = 0;
    }

    public void BackFromTipsButton()
    {
        m_PopUpContainer.SetActive(false);
        m_TipsMenu.SetActive(false);
        m_MainPauseMenuButtons.SetActive(true);
        m_ActivatedPanel = m_MainPauseMenuButtons;
        m_PauseMenuStatusText.text = "[Paused]";
    }

    // shared function
    //public void SwitchTipsButton(int _direction)
    //{
    //    CurrentIndex += _direction;
    //    getIndex();
    //    //TipText.text = SharedManager.Instance.GameTips[CurrentIndex];
    //    //TipImage.sprite = SharedManager.Instance.GameTipsImages[CurrentIndex];
    //}
    #endregion

    #region Settings button functions
    public void SettingsButton()
    {
        m_PopUpContainer.SetActive(true);
        m_SettingsMenu.SetActive(true);
        m_ActivatedPanel = m_SettingsMenu;
        m_MainPauseMenuButtons.SetActive(false);
        m_PauseMenuStatusText.text = "[Settings]";
    }
    public void BackFromSettingsButton()
    {
        m_PopUpContainer.SetActive(false);
        m_SettingsMenu.SetActive(false);
        m_MainPauseMenuButtons.SetActive(true);
        m_ActivatedPanel = m_MainPauseMenuButtons;
        m_PauseMenuStatusText.text = "[Paused]";
        saveSlidersValues(MusicSlider.value, SfxSlider.value, ContrastSlider.value, MouseSensitivitySlider.value);
    }

    public void ChangeMusicVolume(float _volume) => MainAudioMixer.SetFloat("musicVolume", _volume);

    public void ChangeSfxVolume(float _volume) => MainAudioMixer.SetFloat("sfxVolume", _volume);

    public void refreshSliders(float _music, float _sfx, float _contrast, float _yaw)
    {
        MusicSlider.value = _music;
        SfxSlider.value = _sfx;
        ContrastSlider.value = _contrast;
        MouseSensitivitySlider.value = _yaw;
    }

    public void saveSlidersValues(float _music, float _sfx, float _contrast, float _yaw)
    {
        PlayerPrefs.SetFloat(Keys.Settings_Music, _music);
        PlayerPrefs.SetFloat(Keys.Settings_SFX, _sfx);
        PlayerPrefs.SetFloat(Keys.Settings_Brightness, _contrast);
        PlayerPrefs.SetFloat(Keys.Settings_MouseSensitivity, _yaw);
    }

    public virtual void ChangeContrast(float _value)
    {
        if (PostProcessingProfile.TryGet(out Exposure effect))
        {
            var contrast = Mathf.Lerp(MIN_BRIGHTNESS, MAX_BRIGHTNESS, _value);
            PlayerPrefs.SetFloat(Keys.Settings_Brightness, _value);
            effect.fixedExposure.value = contrast;
        }
    }

    public void ChangeMouseSensitivity(float _value) => PlayerMovement.Instance.YawSensitivity = _value;

    public void DefaultButton()
    {
        refreshSliders(0f, 0f, 1f, 2f);
        saveSlidersValues(0f, 0f, 1f, 2f);
    }
    #endregion

    public void ExitMainMenuButton() => SceneManager.LoadScene(0);

    public void ExitToDesktopButton() => Application.Quit();

    void togglePause(bool _cursorVisibility, CursorLockMode _cursorMode, float _timeScale)
    {
        IsPaused = !IsPaused;
        m_GamePlayCanvas.enabled = !m_GamePlayCanvas.enabled;
        m_PauseMenuCanvas.enabled = !m_PauseMenuCanvas.enabled;
        GameManager.Instance.changeCursorState(_cursorVisibility, _cursorMode);
        Time.timeScale = _timeScale;

        if (!IsPaused && !PlayerObjectsInteraction.Instance.Holding || IsPaused || !PlayerCameraEvent.Instance.AllowedToMove)
        {
            PlayerMovement.Instance.AllowMovement(!_cursorVisibility);
        }
    }

    void IncreaseMedicine()
    {
        if (m_MedicineText != null)
        {
            m_MedicineText.text = PlayerStats.Instance.MedicinesCollected + "/" + PlayerStats.Instance.MaxNumberOfMedicine;
        }
    }

    public void RetryButton() => SceneManager.LoadScene(1);

    public void LoadStartSettings()
    {
        AudioManager.Instance.StartUpValues();                                                                      // loads default audio volumes
        GameManager.Instance.SetDifficulty(PlayerPrefs.GetInt(Keys.Settings_Difficulty));                           // loads selected or default difficulty
        PlayerStats.Instance.SetDifficultyValues(GameManager.Instance.Settings.CurrentDifficulty);
        try
        {
            if (PostProcessingProfile.TryGet(out Exposure effect))
            {// get the postprocessing volume
                refreshSliders(_music: PlayerPrefs.GetFloat(Keys.Settings_Music),
                                 _sfx: PlayerPrefs.GetFloat(Keys.Settings_SFX),
                            _contrast: PlayerPrefs.GetFloat(Keys.Settings_Brightness)/*effect.fixedExposure.value*/,                      // applies the value on the slider
                                 _yaw: PlayerPrefs.GetFloat(Keys.Settings_MouseSensitivity));
            }
        }
        catch
        {
            print("Something is wrong with the settings values");
        }
    }
}