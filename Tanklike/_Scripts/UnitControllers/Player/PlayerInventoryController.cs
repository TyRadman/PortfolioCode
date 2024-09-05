using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerInventoryController : MonoBehaviour, IInput, IController
    {
        public bool IsActive { get; private set; }
        private PlayerComponents _components;

        public void SetUp(PlayerComponents components)
        {
            _components = components;
            SetUpInput(_components.PlayerIndex);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Inventory.name).performed += OpenInventory;

            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Inventory.name).performed += CloseInventory;
            UIMap.FindAction(c.UI.Exit.name).performed += CloseInventory;
            UIMap.FindAction(c.UI.Cancel.name).performed += CloseInventory;
            UIMap.FindAction(c.UI.Tab_Right.name).performed += TabRight;
            UIMap.FindAction(c.UI.Tab_Left.name).performed += TabLeft;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Inventory.name).performed -= OpenInventory;

            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Inventory.name).performed -= CloseInventory;
            UIMap.FindAction(c.UI.Exit.name).performed -= CloseInventory;
            UIMap.FindAction(c.UI.Cancel.name).performed -= CloseInventory;
            UIMap.FindAction(c.UI.Tab_Right.name).performed -= TabRight;
            UIMap.FindAction(c.UI.Tab_Left.name).performed -= TabLeft;
        }
        #endregion

        private void OpenInventory(InputAction.CallbackContext _)
        {
            // disable the Pause menu controller so that the inputs don't interfere 
            _components.UIController.EnablePauseMenuController(false);
            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.InputManager.EnableUIInput(_components.PlayerIndex);
            GameManager.Instance.Inventory.Open(_components.PlayerIndex);
        }

        public void CloseInventory(InputAction.CallbackContext _)
        {
            if (!GameManager.Instance.Inventory.IsActive)
            {
                return;
            }

            print("Performed");
            // enable the input for the pause manager
            _components.UIController.EnablePauseMenuController(true);
            GameManager.Instance.InputManager.EnablePlayerInput();
            GameManager.Instance.Inventory.Close(_components.PlayerIndex);
        }

        public void TabRight(InputAction.CallbackContext _)
        {
            GameManager.Instance.Inventory.SwitchTabs(Direction.Right);
        }

        public void TabLeft(InputAction.CallbackContext _)
        {
            GameManager.Instance.Inventory.SwitchTabs(Direction.Left);
        }

        #region IController
        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {
            DisposeInput(_components.PlayerIndex);
        }

        public void Restart()
        {
            DisposeInput(_components.PlayerIndex);
        }
        #endregion
    }
}
