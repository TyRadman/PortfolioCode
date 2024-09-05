 using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TankLike.UI
{
    using UnitControllers;

    public class ResultsUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameoverScreen;
        [SerializeField] private MenuSelectable _firstSelectedItem;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _victoryText;

        private MenuSelectable _currentSelectable;

        private const float START_DELAY = 2f;

        private void Awake()
        {
            _gameoverScreen.SetActive(false);
        }

        public void DisplayGameoverScreen()
        {
            _victoryText.enabled = false;
            _gameOverText.enabled = true;
            StartCoroutine(DisplayGameoverScreenProcess());
        }

        public void DisplayVictoryScreen(TankComponents tank)
        {
            _victoryText.enabled = true;
            _gameOverText.enabled = false;
            StartCoroutine(DisplayGameoverScreenProcess());
        }

        private IEnumerator DisplayGameoverScreenProcess()
        {
            yield return new WaitForSeconds(START_DELAY);

            // setup the input using the first player's index
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                SetUpInput(i);
            }

            // show the pause menu content 
            _gameoverScreen.SetActive(true);
            // disable player input and enable UI input
            GameManager.Instance.InputManager.EnableUIInput();
            // highlight the first button in the list (the resume button)
            _currentSelectable = _firstSelectedItem;
            _currentSelectable.Highlight(true);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            // for the UI
            UIMap.FindAction(c.UI.Submit.name).performed += Select;
     
            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;
        }

        public void Select(InputAction.CallbackContext _)
        {
            _currentSelectable.InvokeAction();
        }

        public void NavigateUp(InputAction.CallbackContext _)
        {
            Navigate(Direction.Up);
        }

        public void NavigateDown(InputAction.CallbackContext _)
        {
            Navigate(Direction.Down);
        }

        public void NavigateLeft(InputAction.CallbackContext _)
        {
            Navigate(Direction.Left);
        }

        public void NavigateRight(InputAction.CallbackContext _)
        {
           Navigate(Direction.Right);
        }
        #endregion

        #region Button Methods
        public void HighLightSelectable(MenuSelectable cell)
        {
            // dehighlight the previous cell
            _currentSelectable.Highlight(false);
            // set the new cell and highlight it
            _currentSelectable = cell;
            cell.Highlight(true);
        }

        public void Navigate(Direction direction)
        {
            _currentSelectable.InvokeAction(direction);
        }

        public void Restart()
        {
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                GameManager.Instance.PlayersManager.GetPlayer(i).Restart();
            }

            if (DontDestroy.i != null)
            {
                DontDestroy.i.ResetInputParent();
            }

            SceneManager.LoadScene(2);
        }

        public void LoadMainMenu()
        {
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                GameManager.Instance.PlayersManager.GetPlayer(i).Restart();
            }

            if (DontDestroy.i != null)
            {
                DontDestroy.i.ResetInputParent();
            }

            SceneManager.LoadScene(0);
        }

        public void LoadLobby()
        {
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                GameManager.Instance.PlayersManager.GetPlayer(i).Restart();
            }

            if (DontDestroy.i != null)
            {
                DontDestroy.i.ResetInputParent();
            }

            SceneManager.LoadScene(1);
        }

        public void Quit()
        {
            Application.Quit();
        }
        #endregion
    }
}
