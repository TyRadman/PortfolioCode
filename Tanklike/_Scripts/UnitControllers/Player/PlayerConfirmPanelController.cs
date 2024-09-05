using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerConfirmPanelController : MonoBehaviour, IInput, IController
    {
        private PlayerComponents _component;

        public bool IsActive { get; private set; }

        public void SetUp(PlayerComponents components)
        {
            _component = components;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Submit.name).performed += Select;
            UIMap.FindAction(c.UI.Cancel.name).performed += Return;
            UIMap.FindAction(c.UI.Exit.name).performed += Return;

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Submit.name).performed -= Select;
            UIMap.FindAction(c.UI.Cancel.name).performed -= Return;
            UIMap.FindAction(c.UI.Exit.name).performed -= Return;

            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
        }
        #endregion

        #region Button Methods
        public void Select(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Select();
        }

        public void NavigateUp(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Up);
        }

        public void NavigateDown(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Down);
        }

        public void NavigateLeft(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Left);
        }

        public void NavigateRight(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Right);
        }

        public void Return(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Return();
        }
        #endregion

        #region IController
        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void Restart()
        {
            DisposeInput(_component.PlayerIndex);
        }

        public void Dispose()
        {
            DisposeInput(_component.PlayerIndex);
        }
        #endregion
    }
}
