using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankLike
{
    using UI;
    using Utils;
    using UI.Styles;
    using UI.Notifications;
    using Combat;
    using Minimap;
    using LevelGeneration;
    using Environment.LevelGeneration;
    using UI.Map;
    using Sound;
    using UnitControllers;
    using ScreenFreeze;
    using UI.PauseMenu;
    using UI.DamagePopUp;
    using UI.Inventory;
    using UI.HUD;
    using SkillTree;
    using Cam;
    using UI.Workshop;
    using UnityEngine.SceneManagement;
    using TankLike.UI.MainMenu;

    public class GameManager : Singleton<GameManager>
    {
        #region Managers
        [SerializeField] private UnityEvent _onGameStarted;
        [field: SerializeField, Header("Managers")] public PoolingManager PoolingManager { get; private set; }
        [field: SerializeField] public PlayersManager PlayersManager { get; private set; }
        [field: SerializeField] public ReportManager ReportManager { get; private set; }
        [field: SerializeField] public CollectableManager CollectableManager { get; private set; }
        [field: SerializeField] public CameraManager CameraManager { get; private set; }
        [field: SerializeField] public InputManager InputManager { get; private set; }
        [field: SerializeField] public InteractableAreasManager InteractableAreasManager { get; private set; }
        [field: SerializeField] public DebugManager DebugManager { get; private set; }
        [field: SerializeField] public VisualEffectsManager VisualEffectsManager { get; private set; }
        [field: SerializeField] public EnemiesManager EnemiesManager { get; private set; }
        [field: SerializeField] public StylesManager StylesManager { get; private set; }
        [field: SerializeField] public QuestsManager QuestsManager { get; private set; }
        [field: SerializeField] public RoomsManager RoomsManager { get; private set; }
        [field: SerializeField] public BossKeysManager BossKeysManager { get; private set; }
        [field: SerializeField] public SummonsManager SummonsManager { get; private set; }
        [field: SerializeField] public LevelGenerator LevelGenerator { get; private set; }
        [field: SerializeField] public ObstaclesVanisher ObstaclesVanisher { get; private set; }
        [field: SerializeField] public ShopsManager ShopsManager { get; private set; }
        [field: SerializeField] public BulletsManager BulletsManager { get; private set; }
        [field: SerializeField] public DestructiblesManager DestructiblesManager { get; private set; }
        [field: SerializeField] public GameplayRoomGenerator GameplayRoomGenerator { get; private set; }
        [field: SerializeField] public AudioManager AudioManager { get; private set; }
        [field: SerializeField] public ScreenFreezer ScreenFreezer { get; private set; }
        [field: SerializeField] public PauseMenuManager PauseMenuManager { get; private set; }
        [field: SerializeField] public TankFallingPartsManager FallingPartsManager { get; private set; }
        [field: SerializeField] public DamagePopUpManager DamagePopUpManager { get; private set; }
        [field: SerializeField] public PlayersOffScreenIndicator OffScreenIndicator{ get; private set; }
        [field: SerializeField] public BossesManager BossesManager { private set; get; }
        [field: SerializeField] public TeleportationManager TeleportationManager { private set; get; }
        #endregion

        #region UI Controllers
        [field: SerializeField, Header("UI Controllers")] public Canvas MainCanvas { get; private set; }
        [field: SerializeField] public HUDController HUDController { get; private set; }
        [field: SerializeField] public NotificationsManager NotificationsManager { get; private set; }
        [field: SerializeField] public MinimapManager MinimapManager { get; private set; }
        [field: SerializeField] public EffectsUIController EffectsUIController { get; private set; }
        [field: SerializeField] public FadeUIController FadeUIController { get; private set; }
        [field: SerializeField] public LevelMapDisplayer LevelMap { get; private set; }
        [field: SerializeField] public ResultsUIController ResultsUIController { get; private set; }
        [field: SerializeField] public TabsManager Inventory { get; private set; }
        [field: SerializeField] public ToolsNavigator ToolShopUI { get; private set; }
        [field: SerializeField] public SkillTreesManager SkillTreesManager { get; private set; }
        [field: SerializeField] public WorkShopTabsNavigatable WorkShopManager { get; private set; }
        [field: SerializeField] public ConfirmPanel ConfirmPanel { get; private set; }
        #endregion

        #region Databases
        [field: SerializeField, Header("Databases")] public ToolsDatabase ToolsDatabase { get; private set; }
        [field: SerializeField] public AmmunitionDatabase BulletsDatabase { get; private set; }
        [field: SerializeField] public EnemiesDatabase EnemiesDatabase { get; private set; }
        [field: SerializeField] public ConstantsManager Constants { get; private set; }
        [field: SerializeField] public PlayerTempInfoSaver PlayersTempInfoSaver { get; private set; }
        [field: SerializeField] public BossesDatabase BossesDatabase { get; private set; }
        [field: SerializeField] public PlayersDatabase PlayersDatabase { get; private set; }
        [field: SerializeField] public InputIconsDatabase InputIconsDatabase { get; private set; }

        #endregion

        private StateMachine<GameStateType> _stateMachine;
        protected Transform _spawnablesParent;

        [Header("Debug")]
        [SerializeField] private bool _generateLevel;
        [SerializeField] private bool _enableDebug = true;
        [SerializeField] private bool _showCursor = true;
        [SerializeField] private bool _testingScene;
        [SerializeField] private int _frameRate = 60;

        [HideInInspector] public Dictionary<string, bool> FoldoutStates = new Dictionary<string, bool>();
        // an event that test scripts can subscribe to

        public void SetParentToSpawnables(GameObject obj)
        {
            obj.transform.parent = _spawnablesParent;
        }

        public void SetParentToRoomSpawnables(GameObject obj)
        {
            obj.transform.parent = RoomsManager.CurrentRoom.SpawnablesParent;
        }

        private void Awake()
        {
            // Set managers references
            InputManager.SetReferences(InputIconsDatabase);
            VisualEffectsManager.SetReferences(BulletsDatabase);
            EnemiesManager.SetReferences(EnemiesDatabase);
            BossesManager.SetReferences(BossesDatabase);
            PlayersManager.SetReferences(PlayersDatabase);

            // Init game state machine
            InitStateMachine();

            // TESTING: Load scene using the scene starter when not starting from the bootstrap scene
            SceneStarter sceneStarter = FindObjectOfType<SceneStarter>();

            if (sceneStarter != null)
            {
                sceneStarter.StartScene();
            }
            //else
            //{
            //    _stateMachine.SetInitialState(GameStateType.SplashScreen);
            //}

            _spawnablesParent = new GameObject("Spawnables").transform;

            // Debug options
            Debug.unityLogger.logEnabled = _enableDebug;

            if (_showCursor)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        //protected virtual void Start()
        //{
        //    Application.targetFrameRate = _frameRate;
        //    QualitySettings.vSyncCount = 0;
        //    _onGameStarted?.Invoke();

        //    RoomsManager.SetUp();

        //    if (_generateLevel)
        //    {
        //        LevelGenerator.GenerateLevel();
        //    }

        //    DestructiblesManager.SetUp();
        //    NotificationsManager.SetUp();
        //    AudioManager.SetUp();
        //    InputManager.SetUp();
        //    DamagePopUpManager.SetUp();
        //    VisualEffectsManager.SetUp();

        //    PlayersManager.SetUp();
        //    EnemiesManager.SetUp();
        //    BossesManager.SetUp();

        //    if (!_testingScene)
        //    {
        //        PlayerSpawner.SpawnPlayers();
        //    }

        //    CameraManager.SetUp(_testingScene);
        //    ObstaclesVanisher.SetUp();
        //    //PoolingManager.Setup(_spawnablesParent);
        //    InteractableAreasManager.SetUp();
        //    CollectableManager.SetUp();

        //    BossKeysManager.SetUp();
        //    ShopsManager.SetUp();
        //    QuestsManager.SetUp();

        //    InputManager.EnablePlayerInput();
        //    OffScreenIndicator.SetUp();
        //    PauseMenuManager.SetUp();
        //    HUDController.SetUp();
        //    EffectsUIController.SetUp();
        //}

        private void InitStateMachine()
        {
            _stateMachine = new StateMachine<GameStateType>();
            Dictionary<GameStateType, IState> states = new Dictionary<GameStateType, IState>();

            var splashScreenState = new SplashScreenGameState(_stateMachine);
            states.Add(GameStateType.SplashScreen, splashScreenState); 
            
            var mainMenuState = new MainMenuGameState(_stateMachine);
            states.Add(GameStateType.MainMenu, mainMenuState);

            var abilitySelectionState = new AbilitySelectionGameState(_stateMachine);
            states.Add(GameStateType.AbilitySelection, abilitySelectionState);

            var lobbyState = new LobbyGameState(_stateMachine);
            states.Add(GameStateType.Lobby, lobbyState);

            var gameplayState = new GameplayGameState(_stateMachine);
            states.Add(GameStateType.Gameplay, gameplayState);

            var bossesTestState = new BossesTestGameState(_stateMachine);
            states.Add(GameStateType.BossesTest, bossesTestState);      

            _stateMachine.Init(states);
        }

        public void ChangeGameState(GameStateType state)
        {
            _stateMachine.ChangeState(state);
        }

        [SerializeField] private TMPro.TextMeshProUGUI _debugText;

        public void OnGameOver()
        {
            ResultsUIController.DisplayGameoverScreen();
            CameraManager.PlayerCameraFollow.StopCameraFollowProcess();
        }
    }

    public enum Direction
    {
        Up = 0, Left = 1, Down = 2, Right = 3, None = 4
    }
}
