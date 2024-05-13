using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerPauseMenuController : MonoBehaviour, IInput, IController
    {
        private PlayerComponents _component;
        public bool IsActive { get; private set; }

        public void SetUp(PlayerComponents components)
        {
            _component = components;
            SetUpInput(_component.PlayerIndex);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Pause.name).performed += Pause;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            // for the UI
            UIMap.FindAction(c.UI.Submit.name).performed += Select;
            UIMap.FindAction(c.UI.Cancel.name).performed += Resume;
            UIMap.FindAction(c.UI.Exit.name).performed += Resume;

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Pause.name).performed -= Pause;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Submit.name).performed -= Select;
            UIMap.FindAction(c.UI.Cancel.name).performed -= Resume;
            UIMap.FindAction(c.UI.Exit.name).performed -= Resume;

            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;
        }
        #endregion

        public void Pause(InputAction.CallbackContext _)
        {
            // pause the menu
            GameManager.Instance.PauseMenuManager.PauseGame(_component.PlayerIndex);
        }

        public void Select(InputAction.CallbackContext _)
        {
            GameManager.Instance.PauseMenuManager.Select();
        }

        public void NavigateUp(InputAction.CallbackContext _)
        {
            GameManager.Instance.PauseMenuManager.Navigate(Direction.Up);
        }

        public void NavigateDown(InputAction.CallbackContext _)
        {
            GameManager.Instance.PauseMenuManager.Navigate(Direction.Down);
        }

        public void NavigateLeft(InputAction.CallbackContext _)
        {
            GameManager.Instance.PauseMenuManager.Navigate(Direction.Left);
        }

        public void NavigateRight(InputAction.CallbackContext _)
        {
            GameManager.Instance.PauseMenuManager.Navigate(Direction.Right);
        }

        public void Resume(InputAction.CallbackContext _)
        {
            // resume the game
            GameManager.Instance.PauseMenuManager.ResumeGame();
        }

        #region IController
        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void Restart()
        {
        }

        public void Dispose()
        {
            DisposeInput(_component.PlayerIndex);
        }
        #endregion
    }
}
