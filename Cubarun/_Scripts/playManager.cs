//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class playManager : MonoBehaviour
//{
//    public static playManager Instance;
//    private Rigidbody rb;

//    // player status
//    [HideInInspector]
//    public bool playerLost = false;
//    [HideInInspector]
//    public int collectedCoins = 0;
//    [HideInInspector]
//    public float score;
//    public float scoreMultiplier;
//    [HideInInspector]
//    public float coinMultiplier = 10f;

//    [Header("Menus")]
//    // restart menu panel
//    public GameObject restartMenu;
//    public GameObject revivePanel;
//    public Image reviveBar;
//    public float reviveDuration;
//    public int intitialRevivingCost;
//    private int reviveCounter = 0;
//    public Text revivingPriceTag;
//    public GameObject notEnoughCoinsForReviving;
//    public Text totalCoins_revive;


//    public Text menuScoreText;
//    public Text menuCoinsText;
//    public Text menuDistanceText;
//    public Text menuHighScoreText;
//    public Text menuHighScoreTextLetters;
//    public Animation highScoreAnimation;

//    // starting menu panel
//    public GameObject startingMenu;

//    // pause menu panel
//    public GameObject pauseMenu;

//    [Header("Effect panels")]
//    public GameObject redPanel;
//    private Animation redPanelAnimation;
//    public GameObject winningFadeOut;
//    private Animation winningFadeAnimation;
//    public GameObject revivePanelBlank;

//    [Header("UI screen texts")]
//    public Text coinsText;
//    public Text scoreText;
//    public Text distanceText;
//    public Text test;

//    [Header("Screen buttons")]
//    public Button pauseButton;
//    public GameObject speedUpButton;
//    public GameObject slowDownButton;
//    public GameObject leftButton;
//    public GameObject rightButton;

//    [Header("Super powers refrences")]
//    public GameObject[] superPowers;

//    // player prefabs string values
//    [HideInInspector]
//    public string highScore = "highScore";
//    private string totalCoins = "totalCoins";
//    private string selectedCube = "selectedCube";
//    public int selectedIndex;

//    // dying variables
//    public PhysicMaterial groundMaterial;
//    private BoxCollider box;
//    private CapsuleCollider capsule;

//    // starting variables
//    private playerMovement player;

//    // mains 
//    public bool isPaused = false;
//    private Vector3 startingPosition;
//    private Quaternion startingRotation; 
//    private float startingDrag;
//    private RigidbodyConstraints startingConstraints;
//    private float additionalDistance = 0f;

//    [SerializeField] private Material[] playerMaterials;
//    [SerializeField] private MeshRenderer PlayerMesh;
//    private void Start()
//    {
//        // can be deleted later, I did this just to see the buttons in the editor without disabling them because these two lines will do it for me
//        // delete before publishing
//        speedUpButton.SetActive(false);
//        slowDownButton.SetActive(false);
//        leftButton.SetActive(false);
//        rightButton.SetActive(false);
        
//        coinsText.text = "";
//        scoreText.text = "";
//        distanceText.text = "";

//        startingMenu.SetActive(true);

//        // refrencing via code
//        rb = FindObjectOfType<playerMovement>().GetComponent<Rigidbody>();
//        player = FindObjectOfType<playerMovement>();
//        redPanelAnimation = redPanel.GetComponent<Animation>();
//        winningFadeAnimation = winningFadeOut.GetComponent<Animation>();

//        box = player.GetComponent<BoxCollider>();
//        capsule = player.GetComponent<CapsuleCollider>();

//        startingPosition = player.transform.position;
//        startingDrag = rb.drag;
//        startingConstraints = rb.constraints;
//    }

//    private void Update()
//    {
//        if (startingMenu.activeSelf)
//        {
//            if (Input.touchCount > 0 || Input.GetKey(KeyCode.Space))
//            {
//                // starting menu taken down
//                startingMenu.SetActive(false);
                
//                displayUIElements();
                
//                // cube starts moving
//                player.isMoving = true;
//            }
//        }
//        else
//        {
//            if (!playerLost)
//            {
//                // keeping track of distance and score eac frame
//                distanceText.text = (transform.position.x + additionalDistance).ToString("0");

//                if (player.isMoving)
//                {
//                    addScore(player.speed * scoreMultiplier);
//                }
//            }
//        }
//    }

//    public void gotHit()
//    {
//        // displays the red death effect
//        redPanelPop();

//        capsule.enabled = false;
//        box.enabled = true;

//        // restoring original friction so that the cube doesn't slide infinitely after death
//        groundMaterial.dynamicFriction = 0.7f;
        
//        applyLosingChanges();
//    }

//    public void fallenDown()
//    {
//        applyLosingChanges();
//    }

//    // MUTUAL CHANGES BETWEEN GETTING AND FALLING DOWN LOSES
//    void applyLosingChanges()
//    {
//        additionalDistance += player.transform.position.x;

//        // removes the drag
//        rb.drag = 0f;

//        // enables rotation to make the player look like it's falling
//        rb.constraints = RigidbodyConstraints.None;

//        // chaniging the player status
//        playerLost = true;

//        // disables movement for player
//        player.isMoving = false;

//        // summoning the revive panel after an amount of time
//        Invoke("reviveOption", 2f);
//    }

//    public void lost()
//    {
//        // checks if player got a highscore
//        setHighScore();

//        // displays score, coins and distance
//        setValues();

//        // removes the status text
//        removeUI();

//        //delays the game over function in case of reviving and to show more of the falling process
//        gameOver();
//    }

//    public void gameOver()
//    {
//        revivePanel.SetActive(false);
//        restartMenu.SetActive(true);
//    }

//    void setValues()
//    {
//        menuScoreText.text = scoreText.text;

//        menuCoinsText.text = coinsText.text;

//        menuDistanceText.text = distanceText.text;

//        menuHighScoreText.text = PlayerPrefs.GetFloat(highScore).ToString("0");
//    }

//    void removeUI()
//    {
//        scoreText.gameObject.SetActive(false);

//        coinsText.gameObject.SetActive(false);

//        distanceText.gameObject.SetActive(false);

//        pauseButton.gameObject.SetActive(false);

//        speedUpButton.SetActive(false);
//        slowDownButton.SetActive(false);
//        leftButton.SetActive(false);
//        rightButton.SetActive(false);
//    }

//    // displaying all UIs after the screen is first touched
//    void displayUIElements()
//    {
//        if (!startingMenu.activeSelf)
//        {
//            // displays the power up
//            coinsText.gameObject.SetActive(true);
//            coinsText.text = collectedCoins.ToString();
//            scoreText.gameObject.SetActive(true);
//            scoreText.text = score.ToString("0");
//            distanceText.gameObject.SetActive(true);
//            distanceText.text = (player.transform.position.x + additionalDistance).ToString("0");
//            speedUpButton.SetActive(true);
//            slowDownButton.SetActive(true);
//            leftButton.SetActive(true);
//            rightButton.SetActive(true);
//        }
//    }

//    void displayValue()
//    {
//        coinsText.gameObject.SetActive(true);
//        scoreText.gameObject.SetActive(true);
//        distanceText.gameObject.SetActive(true);
//    }

//    void setHighScore()
//    {
//        if(PlayerPrefs.GetFloat(highScore) < score)
//        {
//            menuHighScoreText.fontSize = 90;
//            PlayerPrefs.SetFloat(highScore, score);
//            highScoreAnimation.Play();
//            menuHighScoreTextLetters.fontSize = 80;
//            menuHighScoreTextLetters.text = "new highScore";
//            menuHighScoreTextLetters.color = menuHighScoreText.color;
//        }
//    }
    
//    public void coinCollected()
//    {
//        // the displayed coins
//        collectedCoins++;
//        coinsText.text = collectedCoins.ToString("0");

//        // the total coins own by player
//        PlayerPrefs.SetInt(totalCoins, PlayerPrefs.GetInt(totalCoins) + 1);
//    }

//    public void winning()
//    {
//        winningPanelPop();
//    }


//    public void retry()
//    {
//        SceneManager.LoadScene("PlayScene");
//        Time.timeScale = 1f;
//    }

//    public void exit()
//    {
//        Invoke("delayedExit", 0.1f);
//        Time.timeScale = 1f;
//    }

//    void delayedExit()
//    {
//        SceneManager.LoadScene("MainScene");
//    }

//    void redPanelPop()
//    {
//        redPanel.SetActive(true);
//        redPanelAnimation.Play();
//    }

//    void winningPanelPop()
//    {
//        winningFadeOut.SetActive(true);
//        winningFadeAnimation.Play();
//    }

//    public void addScore(float scoreToAdd)
//    {
//        score += scoreToAdd;

//        scoreText.text = score.ToString("0");
//    }

//    public void resume()
//    {
//        isPaused = false;
//        pauseMenu.SetActive(false);

//        // returning everything we want to see
//        displayUIElements();
//        displayValue();
//        pauseButton.gameObject.SetActive(true);

//        if(!startingMenu.activeSelf)
//            player.isMoving = true;
//    }

//    public void pause()
//    {
//        isPaused = true;
//        pauseMenu.SetActive(true);

//        removeUI();
        
//        Time.timeScale = 0f;

//        player.isMoving = false;
//    }
//}