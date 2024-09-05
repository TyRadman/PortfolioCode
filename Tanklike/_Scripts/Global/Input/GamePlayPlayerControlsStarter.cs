using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankLike.UnitControllers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace TankLike
{
    public class GamePlayPlayerControlsStarter : MonoBehaviour
    {
        [SerializeField] private PlayerTempInfoSaver _playersInfoSaver;
        [SerializeField] private PlayerInputHandler _playerPrefab;
        [Range(1, 2)] public int PlayersCount = 1;
        
        public void SetUp()
        {
            _playersInfoSaver.InputHandlers.Clear(); 

            for (int i = 0; i < PlayersCount; i++)
            {
                PlayerInputHandler player = Instantiate(_playerPrefab, transform);
                _playersInfoSaver.AddPlayerInputHandler(player);
            }

            // if there is only one player, then add every possible device as an input device for them
            if (PlayersCount == 1)
            {
                InputDevice[] keyboard = InputSystem.devices.Where(device =>
                 device is Keyboard || device is Mouse || device is Gamepad).ToArray();
                _playersInfoSaver.InputHandlers[0].Playerinputs.SwitchCurrentControlScheme("SinglePlayer", keyboard);
            }
        }
    }
}
