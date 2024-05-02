using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class menuManager : MonoBehaviour
{
    public AudioSource Audio;
    public Animator cameraAnim;
    public Animator cubeAnim;
    public AudioMixer myAudioMixer;
    private int currentIndex;

    public MeshRenderer runningCube;

    // loading bar variables
    public Image loadingBar;
    [SerializeField] private AudioClip purchaseAudio;
    [TextArea]
    public string[] advices;
    public Text advice;
    public Text loadingPercent;

    [Header("Budget Variables")]
    [SerializeField] private Text coins;
    [SerializeField] private Text highScore;
    [SerializeField] private GameObject warning;
    [SerializeField] private GameObject buyButton;

    [Header("Cube selection references")]
    [SerializeField] private Text cubeName;
    [SerializeField] private Text cubeDescription;
    [SerializeField] private Text priceTag;
    [SerializeField] private MeshRenderer showOffCube;
    [SerializeField] private GameObject selectingButton;
    [SerializeField] private GameObject buyingButton;

    private void Awake()
    {
        currentIndex = PlayerPrefs.GetInt(Keys.SelectedCube);
        var currentCube = CubesSelector.Instance.GetCube(currentIndex);
        setUpCubeSelection(currentCube);
    }

    void Start()
    {
        cameraAnim = Camera.main.GetComponent<Animator>();

        // sets the main cube's color to the last selected color
        runningCube.material = CubesSelector.Instance.GetCube(PlayerPrefs.GetInt(Keys.SelectedCube)).Material;

        coins.text = PlayerPrefs.GetInt(Keys.TotalCoins).ToString();

        highScore.text = PlayerPrefs.GetFloat("highScore").ToString("0");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt(Keys.TotalCoins, PlayerPrefs.GetInt(Keys.TotalCoins) + 100);
            coins.text = PlayerPrefs.GetInt(Keys.TotalCoins).ToString();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerPrefs.SetInt(Keys.TotalCoins, 0);
            coins.text = PlayerPrefs.GetInt(Keys.TotalCoins).ToString();
        }

    }

    private void setUpCubeSelection(CubeInfo cube)
    {
        cubeName.text = cube.CubeName;
        cubeDescription.text = cube.CubeDescription;
        showOffCube.material = cube.Material;

        if (cube.locked)
        {
            priceTag.text = cube.price.ToString();
        }
        else
        {
            priceTag.text = "";
        }

        buyingButton.SetActive(cube.locked);
        selectingButton.SetActive(!cube.locked);

    }

    #region Menu Buttons functions
    // MAIN BUTTONS
    public void playButton()
    {
        cameraAnim.SetBool("play", true);
        cubeAnim.SetBool("play", true);

        // sets an advice
        advice.text = advices[Random.Range(0, advices.Length)];

        // delays the loading progress just in case the phone was extremely fast
        Invoke("play", 2.3f);
    }

    void play()
    {
        StartCoroutine(loadPlayScene());
    }

    IEnumerator loadPlayScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("PlayScene");

        while (operation.isDone == false)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingPercent.text = (progress * 100f).ToString("0") + "%";
            loadingBar.fillAmount = progress;

            yield return null;
        }
    }


    public void SelectButton()
    {
        setUpCubeSelection(CubesSelector.Instance.GetCube(currentIndex));
        cameraAnim.SetBool("cube", true);
    }

    public void exitButton()
    {
        cameraAnim.SetBool("exit", true);
        Application.Quit();
    }

    public void resetValues()
    {
        // resets the high score
        PlayerPrefs.SetFloat("highScore", 0f);

        // resets the coins a player owns
        PlayerPrefs.SetInt("totalCoins", 0);

        CubesSelector.Instance.ResetCubeData();
    }

    public void creditsButton()
    {
        cameraAnim.SetBool("optionsToMenu", true);
        cameraAnim.SetBool("backToMenu", false);
    }

    public void backFromCredits()
    {
        cameraAnim.SetBool("optionsToMenu", false);
        cameraAnim.SetBool("backToMenu", true);
    }
    #endregion

    #region Cube selection Buttons
    public void nextButton()
    {
        currentIndex++;

        if(currentIndex > CubesSelector.Instance.GetMaxCubes() - 1)
        {
            currentIndex = 0;
        }

        setUpCubeSelection(CubesSelector.Instance.GetCube(currentIndex));
    }


    public void previousButton()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = CubesSelector.Instance.GetMaxCubes() - 1;
        }

        setUpCubeSelection(CubesSelector.Instance.GetCube(currentIndex));
    }


    public void backFromCube()
    {
        PlayerPrefs.SetInt(Keys.SelectedCube, currentIndex);
        PlayerPrefs.Save();

        // sets the color to the running cube
        runningCube.material = CubesSelector.Instance.GetCube(PlayerPrefs.GetInt(Keys.SelectedCube)).Material;

        // plays the animation
        cameraAnim.SetBool("cube", false);
    }
    #endregion

    public void playAudio(AudioClip clip)
    {
        Audio.clip = clip;
        Audio.Play();
    }

    public void purchase()
    {
        var cube = CubesSelector.Instance.GetCube(currentIndex);

        if(cube.price > PlayerPrefs.GetInt(Keys.TotalCoins))
        {
            // do some fancy stuff
            return;
        }

        PlayerPrefs.SetInt(Keys.TotalCoins, PlayerPrefs.GetInt(Keys.TotalCoins) - cube.price);
        cube.locked = false;
        coins.text = PlayerPrefs.GetInt(Keys.TotalCoins).ToString();
        setUpCubeSelection(cube);
        playAudio(purchaseAudio);

        CubesSelector.Instance.UnlockCube(cube);
    }

    public void toggleAudioVolume(string exposedParameter)
    {
        // to carry the value gotten from the audio mixer's exposed parameter
        float volume;
        myAudioMixer.GetFloat(exposedParameter, out volume);

        // if its not turned off the turn it off else turn it on
        if (volume != -80)
        {
            myAudioMixer.SetFloat(exposedParameter, -80);
        }
        else
        {
            myAudioMixer.SetFloat(exposedParameter, 0);
        }
    }

    public void toggleButtonIcon(Text text)
    {
        if(text.color != Color.red)
        {
            text.color = Color.red;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
