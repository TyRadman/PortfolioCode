using System.Collections;
using System.Collections.Generic;
using TankLike.UI.MainMenu;
using TankLike.UnitControllers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TankLike.MainMenu
{
    public class MainMenuInputManager : MonoBehaviour
    {
        public List<PlayerInputHandler> Inputs = new List<PlayerInputHandler>();
        [SerializeField] private PlayerInput _playerPrefab;

        private MainMenuUIController _mainMenu;

        public void SetUp(MainMenuUIController mainMenu)
        {
            _mainMenu = mainMenu;
        }

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            Debug.Log("ON JOINED");
            PlayerInputHandler inputHandler = playerInput.GetComponent<PlayerInputHandler>();
            //inputHandler.SetUp();
            Inputs.Add(inputHandler);
            _mainMenu.SetUpInputForInputHandler(inputHandler);
        }

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            Debug.Log("ON LEFT");
            PlayerInputHandler inputHandler = playerInput.GetComponent<PlayerInputHandler>();
            Inputs.Remove(inputHandler);
        }

        public void RemoveInputHandlers()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                Destroy(Inputs[i].gameObject);
            }

            Inputs.Clear();
        }
    }
}
