using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class AbilitySelectionUIController : MonoBehaviour
    {
        [SerializeField] protected AbilitySelectionData _selectionData;

        [SerializeField] private AbilitySelectionPanel _normalShotParent;
        [SerializeField] private AbilitySelectionPanel _holdAbilityParent;
        [SerializeField] private AbilitySelectionPanel _superAbilityParent;
        [SerializeField] private AbilitySelectionPanel _boostAbilityParent;

        [Header("Loading Bar")]
        [SerializeField] private GameObject _loadingBar;
        [SerializeField] private Image _loadingBarImage;

        private AbilitySelectionPanel _currentSelectedPanel;

        private const string ABILITY_SELECTION_SCENE = "S_AbilitySelection";
        private const string LOBBY_SCENE = "S_Lobby";

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _loadingBar.SetActive(false);

            _normalShotParent.Setup();
            _holdAbilityParent.Setup();
            _superAbilityParent.Setup();
            _boostAbilityParent.Setup();

            _currentSelectedPanel = _normalShotParent;
            DisplayNextAbility(_normalShotParent);
        }

        public void DisplayNextAbility(AbilitySelectionPanel panel)
        {
            _currentSelectedPanel.gameObject.SetActive(false);
            _currentSelectedPanel = panel;
            _currentSelectedPanel.gameObject.SetActive(true);
        }

        public void LoadLobbyScene()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _loadingBar.SetActive(true);
            StartCoroutine(LoadingSceneProcess());
        }

        private IEnumerator LoadingSceneProcess()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(LOBBY_SCENE, LoadSceneMode.Additive);

            while (!loadOperation.isDone)
            {
                _loadingBarImage.fillAmount = loadOperation.progress;
                yield return null;
            }

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(ABILITY_SELECTION_SCENE);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
