using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 3735 lines of code so far
public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;
    // the widow that'll hold the place to avoid error and will transferred to the triggering jump scare
    [Header("General Variables")]
    public int HidingPlaceDuration = 2;         // how many times can a single hiding spot save you
    [Tooltip("In minutes")]
    public int TimeToSpawnEnemy = 1;    // in minutes
    [HideInInspector] public bool GameIsOver = true;
    [HideInInspector] public bool EnemySpawned = false;
    [Header("Death Animation")]
    [SerializeField] private CameraObject m_DeathEvent;
    [Header("References")]
    [SerializeField] private GameObject m_GamePlayCanvas;
    [SerializeField] private GameObject EndingWall;
    [SerializeField] private GameObject EndingDoor = null;
    [SerializeField] private GameObject m_EndingScreen;
    [SerializeField] private Animator m_EndingScreenAnim;
    public DifficultyModifier[] DifficultyValues;
    [HideInInspector] public DifficultyModifier CurrentDifficulty;
    [HideInInspector] public List<DifficultyModifier> CurrentDifficulties = new List<DifficultyModifier>();


    [System.Serializable]
    struct Message
    {
        [TextArea]
        public string MessageText;
        public float Duration;
    }
    #endregion

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }

        changeCursorState(false, CursorLockMode.Locked);
        SetDifficulty(PlayerPrefs.GetInt(Keys.Settings_Difficulty));
    }

    void Start()
    {
        FadingScreen.Instance.BlankScreen(Color.black);
        // prevents the player from moving, for the story
        PlayerMovement.Instance.AllowedToMove = false;                        
        ItemsSpawning.Instance.SpawnAllItems();
        FindObjectOfType<StoryTelling>().StartStory();      // plays the story
    }

    public void RemoveBlackScreen()
    {
        FadingScreen.Instance.FadeOut(Color.black);
        GameIsOver = false;
    }

    public void AllowPlayerToMove()
    {
        PlayerMovement.Instance.AllowedToMove = true;
    }

    #region Game over functions
    public void GameOver()
    {
        GameIsOver = true;
        // turn off any UI
        Inventory.Instance.TurnOffInventory();

        FindObjectOfType<Statistics>().AssignStatistics();
        deathAnimation();
    }

    private void deathAnimation()
    {
        PlayerCameraEvent.Instance.RotateToObjects(new CameraObject[] {m_DeathEvent}, true);
        StartCoroutine(displayBlackPanel_Enum()); // displays the game over black panel
    }
    #endregion

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void changeCursorState(bool _visibility, CursorLockMode _mode)
    {
        Cursor.visible = _visibility;
        Cursor.lockState = _mode;
    }

    public void GameEnding()
    {
        GameIsOver = true;
        FindObjectOfType<Statistics>().AssignStatistics();
        m_EndingScreen.SetActive(true);
        m_EndingScreenAnim.SetBool("Loss", false);
        Invoke(nameof(enableCursor), 3f);
    }

    void enableCursor()
    {
        changeCursorState(true, CursorLockMode.Confined);
    }

    IEnumerator displayBlackPanel_Enum()
    {
        m_GamePlayCanvas.SetActive(false);
        yield return new WaitForSeconds(3f);
        changeCursorState(true, CursorLockMode.None);
        m_EndingScreen.SetActive(true);
        m_EndingScreenAnim.SetBool("Loss", true);
    }

    public void SetDifficulty(int _index)
    {
        CurrentDifficulty = DifficultyValues[_index];
    }
}