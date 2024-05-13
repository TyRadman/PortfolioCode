using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Environment;
using UnityEngine;
using TankLike.Utils;
using System.Linq;
using static TankLike.Environment.RoomSpawnPoints;

namespace TankLike
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        public void SpawnPlayers()
        {
            int playersCount = GameManager.Instance.PlayersTempInfoSaver.PlayersCount;
            List<SpawnPoint> spawnPoints = GameManager.Instance.RoomsManager.CurrentRoom.Spawner.SpawnPoints.Points;
            Vector3 lastPosition = GameManager.Instance.RoomsManager.CurrentRoom.transform.position;

            for (int i = 0; i < playersCount; i++)
            {
                // get the closest not-taken spawn points to the last registered position
                SpawnPoint selectedSpawnPoint = spawnPoints.FindAll(s => !s.Taken).OrderBy(s =>
                (s.Point.position - lastPosition).sqrMagnitude).FirstOrDefault();
                selectedSpawnPoint.Taken = true;

                Vector3 position = selectedSpawnPoint.Point.position;
                lastPosition = position;
                position.y += 2f;
                GameObject player = Instantiate(_playerPrefab, position, Quaternion.identity);
                PlayerComponents components = player.GetComponent<PlayerComponents>();
                components.PlayerIndex = i;
                player.GetComponent<TankComponents>().SetUp();
                GameManager.Instance.PlayersManager.AddPlayer(components);
            }
        }
    }
}
