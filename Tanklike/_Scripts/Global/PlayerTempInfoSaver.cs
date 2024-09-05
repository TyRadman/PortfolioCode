using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    /// <summary>
    /// Saves the data of the players between scenes. Like number of players, the tanks they chose, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayersInfoSaver", menuName = Directories.MAIN + "Players Info Saver")]
    public class PlayerTempInfoSaver : ScriptableObject
    {
        [field: SerializeField] public List<PlayerInputHandler> InputHandlers { get; private set; } = new List<PlayerInputHandler>();
        [field: SerializeField] public int PlayersCount = 0;
        [field: SerializeField] public float GameDifficulty { get; private set; } = 0.5f;
        [field: SerializeField] public RoomType StartRoomType { get; private set; } = RoomType.Normal;

        public void DisableAllInputs()
        {
            InputHandlers.ForEach(i => i.Controls?.Disable());
        }

        public void AddPlayerInputHandler(PlayerInputHandler handler)
        {
            if (InputHandlers.Exists(i => i == handler))
            {
                return;
            }

            if (InputHandlers.Exists(i => i == null))
            {
                InputHandlers.RemoveAll(i => i == null);
            }

            InputHandlers.Add(handler);
            PlayersCount = InputHandlers.Count;
        }

        public void RemovePlayerInputHandler(PlayerInputHandler handler)
        {
            InputHandlers.Remove(handler);
        }

        public bool HasPlayer()
        {
            return InputHandlers.Count > 0 && !InputHandlers.Exists(p => p == null);
        }

        public void SetDifficulty(float difficulty)
        {
            GameDifficulty = difficulty;
        }

        public void SetStartRoom(RoomType startRoomType)
        {
            StartRoomType = startRoomType;
        }
    }
}
