using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI.PauseMenu
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _content;
        [SerializeField] private List<GameObject> _panels;
        [SerializeField] private MenuSelectable _firstSelectedItem;
        [Header("Panels")]
        [SerializeField] private PauseMenuSettings _settingsPanel;
        private float _lastTimeScale;
        private MenuSelectable _currentSelectable;
        private bool _isPaused = false;
        private int _currentPlayerIndex = -1;

        private void Awake()
        {
            _lastTimeScale = 1f;
            EnableFirstPanel();
            _content.SetActive(false);
        }

        public void SetUp()
        {

        }


        public void Select()
        {
            if(!_isPaused)
            {
                return;
            }

            _currentSelectable.InvokeAction();
        }

        public void Navigate(Direction direction)
        {
            if (!_isPaused)
            {
                return;
            }

            _currentSelectable.InvokeAction(direction);
        }

        public void SetSelectedItem(MenuSelectable selectable)
        {
            _currentSelectable = selectable;
        }

        #region Button Methods
        public void HighLightSelectable(MenuSelectable cell)
        {
            // dehighlight the previous cell
            _currentSelectable.Highlight(false);
            // set the new cell and highlight it
            _currentSelectable = cell;
            cell.Highlight(true);
        }

        public void PauseGame(int playerIndex)
        {
            if (_isPaused)
            {
                return;
            }

            // disable pausing for the other player if there is another player
            GameManager.Instance.PlayersManager.EnablePauseInputForSecondPlayer(playerIndex, false);
            // set the current player index to all the panels
            SetPlayerIndex(playerIndex);
            // display the first panel by default (resume, settings, exit)
            EnableFirstPanel();
            // stop time
            GameManager.Instance.ScreenFreezer.PauseFreeze();
            _lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            // show the pause menu
            _content.SetActive(true);
            // hide HUD
            //FIX UI handling
            GameManager.Instance.HUDController.EnableHUD(false);
            // show the pause menu content 
            _content.SetActive(true);
            // disable player input and enable UI input
            GameManager.Instance.InputManager.EnableUIInput(true);
            // highlight the first button in the list (the resume button)
            _currentSelectable = _firstSelectedItem;
            _currentSelectable.Highlight(true);
            _isPaused = true;
        }

        public void ResumeGame()
        {
            if (!_isPaused)
            {
                return;
            }

            // enable the other player's input
            GameManager.Instance.InputManager.EnablePlayerInput(true);
            // resume the time scale the way it was before 
            Time.timeScale = _lastTimeScale;
            GameManager.Instance.ScreenFreezer.ResumeFreeze();
            // hide the pause menu content
            _content.SetActive(false);
            // show HUD
            //FIX HUD handling
            GameManager.Instance.HUDController.EnableHUD(true);
            // hide the pause menu content
            _content.SetActive(false);
            // enable the player input and disable the UI input
            GameManager.Instance.InputManager.EnablePlayerInput(true);
            _isPaused = false;
        }
        #endregion

        private void EnableFirstPanel()
        {
            _panels.ForEach(p => p.SetActive(false));
            _panels[0].SetActive(true);
            _currentSelectable = _firstSelectedItem;
        }

        private void SetPlayerIndex(int playerIndex)
        {
            _settingsPanel.SetPlayerIndex(playerIndex);
            _currentPlayerIndex = playerIndex;
        }
    }
}
