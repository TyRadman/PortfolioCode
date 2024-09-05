using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using TankLike.UnitControllers;
using TankLike.Utils;
using TMPro;
using TankLike.UI;

namespace TankLike
{
    public class PlayerJoinManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerInputSelectionIcon> _icons;
        [SerializeField] private Image _loadingBarImage;
        [SerializeField] private GameObject _loadingBar;
        private PlayerInputManager _manager;
        [SerializeField] private PlayerInput _playerPrefab;

        private const string LOBBY_SCENE = "S_Lobby";
        private const string GAMEPLAY_SCENE = "S_Gameplay";

        public void SetUp()
        {
            _loadingBar.SetActive(false);
            _manager = GetComponent<PlayerInputManager>();

            string joinActionName = _playerPrefab.actions.FindActionMap("Lobby").FindAction("Submit").name;

            // TODO: get the icons indices from the InputManager after we bootstrap the GameManager in the main menu scene
            string joinP1IconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(joinActionName, 0).ToString();
            string joinP2IconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(joinActionName, 1).ToString();

            string joinP1 = $"<sprite={joinP1IconIndex}>";
            string joinP2 = $"<sprite={joinP2IconIndex}>";
            _icons[0].JoinText.GetComponent<TextMeshProUGUI>().text = $"Press {joinP1} or {joinP2} to join";
            _icons[1].JoinText.GetComponent<TextMeshProUGUI>().text = $"Press {joinP1} or {joinP2} to join";
        }

        //private void Awake()
        //{
        //    // Return if the GameManager was not loaded
        //    if(GameManager.Instance == null)
        //    {
        //        return;
        //    }

        //    _loadingBar.SetActive(false);
        //    _manager = GetComponent<PlayerInputManager>();

        //    string joinActionName = _playerPrefab.actions.FindActionMap("Lobby").FindAction("Submit").name;

        //    // TODO: get the icons indices from the InputManager after we bootstrap the GameManager in the main menu scene
        //    string joinP1IconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(joinActionName, 0).ToString();
        //    string joinP2IconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(joinActionName, 1).ToString();

        //    string joinP1 = $"<sprite={joinP1IconIndex}>";
        //    string joinP2 = $"<sprite={joinP2IconIndex}>";
        //    _icons[0].JoinText.GetComponent<TextMeshProUGUI>().text = $"Press {joinP1} or {joinP2} to join";
        //    _icons[1].JoinText.GetComponent<TextMeshProUGUI>().text = $"Press {joinP1} or {joinP2} to join";
        //}

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            _icons[playerInput.playerIndex].JoinText.GetComponent<TextMeshProUGUI>().enabled = false;

            if (playerInput.currentControlScheme == "Controller")
            {
                _icons[playerInput.playerIndex].ControllerIcon.SetActive(true);
            }
            else
            {
                _icons[playerInput.playerIndex].KeyboardIcon.SetActive(true);
            }

            playerInput.transform.parent = DontDestroy.i.InputParent;
            playerInput.GetComponent<PlayerInputHandler>().SetUp();
        }

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            _icons[playerInput.playerIndex].JoinText.GetComponent<TextMeshProUGUI>().enabled = true;

            if (playerInput.currentControlScheme == "Controller")
            {
                _icons[playerInput.playerIndex].ControllerIcon.SetActive(false);
            }
            else
            {
                _icons[playerInput.playerIndex].KeyboardIcon.SetActive(false);
            }
        }

        public void EnableConfirmedButton(int playerIndex, bool enable)
        {
            _icons[playerIndex].ConfirmText.SetActive(enable);
        }

        public void LoadGameplayScene()
        {
            _manager.DisableJoining();
            //_manager.enabled = false;
            _loadingBar.SetActive(true);
            StartCoroutine(LoadingSceneProcess());
        }

        private IEnumerator LoadingSceneProcess()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(GAMEPLAY_SCENE, LoadSceneMode.Additive);

            while (!loadOperation.isDone)
            {
                _loadingBarImage.fillAmount = loadOperation.progress;
                yield return null;
            }

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(LOBBY_SCENE);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
