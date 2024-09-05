using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TankLike.UI.MainMenu
{
    using TankLike.MainMenu;
    using TankLike.Sound;
    using TankLike.UI.Signifiers;
    using TankLike.UnitControllers;
    using TankLike.Utils;

    public class MainMenuUIController : MonoBehaviour, ISignifiersDisplayer

    {
        [SerializeField] private GameObject _content;
        [SerializeField] private List<GameObject> _panels;
        [SerializeField] private MenuSelectable _firstSelectedItem;
        [SerializeField] private UIActionSignifiersController _menuActionSignifiersController;

        [Header("Panels")]
        [SerializeField] private MainMenuSettingsManager _settingsPanel;
        
           
        private MainMenuInputManager _inputManager;
        private MenuSelectable _currentSelectable;
        private PlayerInputActions _controls;
        private string _currentControlScheme;

        public ISignifierController SignifierController { get; set; }

        private const string SUBMIT_ACTION_TEXT = "Select";
        private const string KEYBOARD_SCHEME = "Keyboard";
        private const string CONTROLLER_SCHEME = "Controller";

        private const string MAIN_MENU_SCENE = "S_MainMenu";
        private const string ABILITY_SELECTION_SCENE = "S_AbilitySelection";

        public void SetUp(MainMenuInputManager inputManager)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //// Get reference to the MainMenuInputManager from the scene
            //_inputManager = FindObjectOfType<MainMenuInputManager>();
            //_inputManager.SetUp(this);

            _inputManager = inputManager;

            EnableFirstPanel();
            _currentSelectable.Highlight(true);

            _controls = new PlayerInputActions();
            // Set KEYBOARD as a default control scheme
            _currentControlScheme = KEYBOARD_SCHEME;
            SignifierController = _menuActionSignifiersController;
            SetUpActionSignifiers(SignifierController);
        }

        private void EnableFirstPanel()
        {
            _panels.ForEach(p => p.SetActive(false));
            _panels[0].SetActive(true);
            _currentSelectable = _firstSelectedItem;
        }

        #region Input
        public void SetUpInputForInputHandler(PlayerInputHandler inputHandler)
        {
            InputActionMap lobbyActionMap = inputHandler.Playerinputs.actions.FindActionMap(_controls.Lobby.Get().name);
            lobbyActionMap.FindAction(_controls.Lobby.Navigation_Up.name).performed += OnNavigationUp;
            lobbyActionMap.FindAction(_controls.Lobby.Navigation_Down.name).performed += OnNavigationDown;
            lobbyActionMap.FindAction(_controls.Lobby.Navigation_Left.name).performed += OnNavigationLeft;
            lobbyActionMap.FindAction(_controls.Lobby.Navigation_Right.name).performed += OnNavigationRight;
            lobbyActionMap.FindAction(_controls.Lobby.Submit.name).performed += Submit;

            // Update the input signifiers with the new control scheme
            _currentControlScheme = inputHandler.Playerinputs.currentControlScheme;
            _menuActionSignifiersController.ClearAllSignifiers();
            SetUpActionSignifiers(SignifierController);
        }

        private void OnNavigationUp(InputAction.CallbackContext _)
        {
            Navigate(Direction.Up);
        }

        private void OnNavigationDown(InputAction.CallbackContext _)
        {
            Navigate(Direction.Down);
        }

        private void OnNavigationLeft(InputAction.CallbackContext _)
        {
            Navigate(Direction.Left);
        }

        private void OnNavigationRight(InputAction.CallbackContext _)
        {
            Navigate(Direction.Right);
        }

        private void Submit(InputAction.CallbackContext _)
        {
            Select();
        }
        #endregion

        #region Button Methods

        public void Select()
        {
            _currentSelectable.InvokeAction();
        }

        public void Navigate(Direction direction)
        {
            // Play navigate menu audio
            AudioManager audioManager = GameManager.Instance.AudioManager;
            audioManager.Play(audioManager.UIAudio.NavigateMenuAudio);

            _currentSelectable.InvokeAction(direction);
        }

        public void HighLightSelectable(MenuSelectable cell)
        {
            // dehighlight the previous cell
            _currentSelectable.Highlight(false);
            // set the new cell and highlight it
            _currentSelectable = cell;
            cell.Highlight(true);
        }

        public void OnStartGame()
        {
            // remove the input profiles
            if(_inputManager != null)
            {
                _inputManager.RemoveInputHandlers();
            }

            // load the next scene
            StartCoroutine(LoadingSceneProcess());
        }

        private IEnumerator LoadingSceneProcess()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(ABILITY_SELECTION_SCENE, LoadSceneMode.Additive);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(MAIN_MENU_SCENE);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }
        #endregion

        #region ISignifiersDisplayer
        public void SetUpActionSignifiers(ISignifierController signifierController)
        {
            int schemeIndex = _currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;

            if(_controls == null)
                Debug.Log("I ANM NULL controls");
            if(GameManager.Instance == null)
                Debug.Log("I ANM NULL game manager");
            if (GameManager.Instance.InputManager == null)
                Debug.Log("I ANM NULL input manager");

            int submitActionIconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(_controls.UI.Submit.name, schemeIndex);
            _menuActionSignifiersController.DisplaySignifier(SUBMIT_ACTION_TEXT, Helper.GetInputIcon(submitActionIconIndex));
        }
        #endregion
    }
}