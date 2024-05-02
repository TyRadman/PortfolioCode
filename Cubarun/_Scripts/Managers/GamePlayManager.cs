using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;
    private enum LevelType
    {
        Random, Preset
    }

    private bool gameOver = false;
    public bool GameOver
    {
        get => gameOver;
        set
        {
            gameOver = value;

            if (value)
            {
                GameIsOver();
            }
        }
    }
    [Header("Level Settings")]
    [SerializeField] private LevelType levelType;

    [Header("General variables")]
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private GameObject GamePlayUI;

    [Header("Reviving variables")]
    [SerializeField] private float revivingCountDownDuration = 3f;
    [SerializeField] private GameObject revivingWindow;
    [SerializeField] private Image revivingCounterBar;
    [SerializeField] private Text availableCoinsAtRevive;
    [SerializeField] private Text revivingCostText;
    [SerializeField] private int initialCost = 1000;
    [SerializeField] private float costMultiplying = 1.5f;
    [SerializeField] private Color notEnoughCoinsColor;
    [SerializeField] private Color enoughCoinsColor;
    [SerializeField] private float afterRevivingTimeToStart = 2f;
    [SerializeField] private float whiteScreenFadeInSpeed = 0.5f;
    [SerializeField] private float whiteScreenFadeOutSpeed = 1f;
    [SerializeField] private float whiteScreenDuration = 1.5f;
    [SerializeField] private Button revivingButton;
    private int revivingTimes = 0;
    private int revivingCost;

    [Header("Starting panel")]
    [SerializeField] private GameObject startingPanel;
    [SerializeField] private float fadingSpeed = 2f;
    [SerializeField] private CanvasGroup startingPanelGroup;
    private bool recieveInput = true;

    

    private void Awake()
    {
        Instance = this;
        startingPanel.SetActive(true);
        clearUI(false);
        
        if(levelType == LevelType.Random)
        {
            GetComponent<RandomLevelGenerator>().enabled = true;
            GetComponent<LevelCreator>().enabled = false;
        }
        else
        {
            GetComponent<RandomLevelGenerator>().enabled = false;
            GetComponent<LevelCreator>().enabled = true;
        }
    }

    private void Start()
    {
        FadingScreen.Instance.Release(Color.white, 1f);
    }

    private void Update()
    {
        if (recieveInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayerEffects.Instance.StartEffects();
                recieveInput = false;
                clearUI(true);
                StartCoroutine(fadePanelOut());
                PlayerStats.Instance.StartCountingScore();
            }
        }
    }

    private void GameIsOver()
    {
        // reviving and stuff
        playerMovement.Instance.PlayState(true);
        PlayerFallingDetector.TurnOff();

        if (levelType == LevelType.Random)
        {
            reviveWindow();
        }
        else
        {
            FadingScreen.Instance.Fade(0.1f, 0.1f, 0.1f, Keys.instance.GettingHitScreenColor);
            Invoke(nameof(showLosingScreen), 1f);
        }
    }

    private void showLosingScreen()
    {
        PlayerStats.Instance.SaveCoins();
        LosingMenuManager.Instance.ShowLosingWindow(revivingWindow);
    }

    private void reviveWindow()
    {
        // filling speed up and down bars
        var speedBars = FindObjectsOfType<SpeedUpAndDown>();
        System.Array.ForEach(speedBars, sb => sb.FullyCharge());
        // filling the super power
        PowerUpsManager.Instance.FullyChargeActivePowerUp();

        revivingWindow.SetActive(true);
        clearUI(false);
        var totalCoins = PlayerPrefs.GetInt(Keys.TotalCoins);
        availableCoinsAtRevive.text = totalCoins.ToString();
        revivingCost = (int)(revivingTimes++ == 0? initialCost : initialCost * revivingTimes * costMultiplying++);
        revivingCostText.color = enoughCoinsColor;

        if(revivingCost > totalCoins)
        {
            revivingCostText.color = notEnoughCoinsColor;
            revivingButton.enabled = false;
        }

        revivingCostText.text = revivingCost.ToString("0");
        StartCoroutine(revivingCountDown());
    }

    private IEnumerator revivingCountDown()
    {
        float time = 0f;

        while(time < revivingCountDownDuration)
        {
            time += Time.deltaTime;
            revivingCounterBar.fillAmount = time / revivingCountDownDuration;
            yield return null;
        }

        LosingMenuManager.Instance.ShowLosingWindow(revivingWindow);
    }


    private IEnumerator fadePanelOut()
    {
        float time = fadingSpeed;
        playerMovement.Instance.isMoving = true;
        while(time > 0f)
        {
            time -= Time.deltaTime;
            startingPanelGroup.alpha = time / fadingSpeed;
            yield return null;
        }
        startingPanel.SetActive(false);
        startingPanelGroup.alpha = 1f;
    }

    #region General Functions
    private void clearUI(bool enable)
    {
        playerCanvas.enabled = enable;
        GamePlayUI.SetActive(enable);
    }
    #endregion

    #region Buttons Functions
    public void Revive()
    {
        StopAllCoroutines();
        GameOver = false;
        PlayerPrefs.SetInt(Keys.TotalCoins, PlayerPrefs.GetInt(Keys.TotalCoins) - revivingCost);
        revivingWindow.SetActive(false);
        // fade the screen
        FadingScreen.Instance.Fade(whiteScreenDuration, whiteScreenFadeInSpeed, whiteScreenFadeOutSpeed, Color.white);
        // finish the rest once the screen is done fading in and out
        Invoke(nameof(finishReviving), afterRevivingTimeToStart - 1);
    }

    private void finishReviving()
    {
        RandomLevelGenerator.Instance.CreateRevivingSpot();
        startingPanel.SetActive(true);
        Invoke(nameof(enableInput), 2f);
    }
    private void enableInput()
    {
        recieveInput = true;
    }

    public void CancelRevive()
    {

    }
    #endregion
}
