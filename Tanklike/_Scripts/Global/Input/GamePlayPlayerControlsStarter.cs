using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike
{
    public class GamePlayPlayerControlsStarter : MonoBehaviour
    {
        public enum Scheme
        {
            Keyboard = 0, Controller = 1
        }

        [SerializeField] private List<Scheme> _playersScheme;
        [SerializeField] private PlayerTempInfoSaver _playersInfoSaver;
        [SerializeField] private PlayerInputHandler _playerPrefab;
        [Range(1, 2)] public int PlayersCount = 1;
        
        public void SetUp()
        {
            for (int i = 0; i < PlayersCount; i++)
            {
                //_inputManager.JoinPlayer();
                PlayerInputHandler player = Instantiate(_playerPrefab, transform);
                _playersInfoSaver.AddPlayerInputHandler(player);
            }
        }
    }
}
