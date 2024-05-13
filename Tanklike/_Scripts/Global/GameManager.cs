using System.Collections.Generic;
using TankLike.UI;
using TankLike.Utils;
using TankLike.UI.Styles;
using TankLike.UI.Notifications;
using TankLike.Combat;
using TankLike.Minimap;
using TankLike.LevelGeneration;
using TankLike.Environment.LevelGeneration;
using TankLike.UI.Map;
using TankLike.Sound;
using TankLike.UnitControllers;
using TankLike.ScreenFreeze;
using TankLike.UI.PauseMenu;
using UnityEngine;
using TankLike.UI.DamagePopUp;
using TankLike.UI.Inventory;
using TankLike.UI.HUD;
using TankLike.SkillTree;
using TankLike.Cam;
using UnityEngine.Events;

namespace TankLike
{
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
        [field: SerializeField] public PlayerSpawner PlayerSpawner { get; private set; }
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
        #endregion

        #region UI Controllers
        [field: SerializeField, Header("UI Controllers")] public HUDController HUDController { get; private set; }
        [field: SerializeField] public NotificationsManager NotificationsManager { get; private set; }
        [field: SerializeField] public MinimapManager MinimapManager { get; private set; }
        [field: SerializeField] public EffectsUIController EffectsUIController { get; private set; }
        [field: SerializeField] public LevelMapDisplayer LevelMap { get; private set; }
        [field: SerializeField] public ResultsUIController ResultsUIController { get; private set; }
        [field: SerializeField] public TabsManager Inventory { get; private set; }
        [field: SerializeField] public ToolsNavigator ToolShopUI { get; private set; }
        [field: SerializeField] public SkillTreesManager SkillTreesManager { get; private set; }
        [field: SerializeField] public WorkShopTabsNavigatable WorkShopManager { get; private set; }
        #endregion

        #region Databases
        [field: SerializeField, Header("Databases")] public ToolsDatabase ToolsDatabase { get; private set; }
        [field: SerializeField] public AmmunitionDatabase BulletsDatabase { get; private set; }
        [field: SerializeField] public EnemiesDatabase EnemiesDatabase { get; private set; }
        [field: SerializeField] public ConstantsManager Constants { get; private set; }
        [field: SerializeField] public PlayerTempInfoSaver PlayersTempInfoSaver { get; private set; }
        [field: SerializeField] public BossesDatabase BossesDatabase { get; private set; }

        #endregion

        protected Transform _spawnablesParent;

        [Header("Debug")]
        [SerializeField] private bool _generateLevel;
        [SerializeField] private bool _enableDebug = true;
        [SerializeField] private bool _showCursor = true;
        [SerializeField] private bool _testingScene;

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
            _spawnablesParent = new GameObject("Spawnables").transform;
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

        protected virtual void Start()
        {
            _onGameStarted?.Invoke();

            if (_generateLevel)
            {
                LevelGenerator.GenerateLevel();
            }

            DestructiblesManager.SetUp();
            NotificationsManager.SetUp();
            AudioManager.SetUp();
            InputManager.SetUp();
            PauseMenuManager.SetUp();
            DamagePopUpManager.SetUp();

            if (!_testingScene)
            {
                PlayerSpawner.SpawnPlayers();
            }

            CameraManager.SetUp(_testingScene);
            ObstaclesVanisher.SetUp();
            PoolingManager.Setup(_spawnablesParent);
            InteractableAreasManager.SetUp();
            CollectableManager.SetUp();
            VisualEffectsManager.SetUp(BulletsDatabase);
            EnemiesManager.SetUp(EnemiesDatabase);
            BossesManager.SetUp(BossesDatabase);
                      
            BossKeysManager.SetUp();
            ShopsManager.SetUp();
            QuestsManager.SetUp();

            InputManager.EnablePlayerInput(true);
            OffScreenIndicator.SetUp();
            HUDController.SetUp();
        }

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
