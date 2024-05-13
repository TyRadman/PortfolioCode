using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    public class ShopsBuilder : MonoBehaviour
    {
        [SerializeField] private InteractableArea _shop;
        [SerializeField] private InteractableArea _workshop;
        [SerializeField] private bool _spawnShop = true;
        [SerializeField] private bool _spawnWorkshop = true;

        public void BuildShops()
        {
            if (!_spawnShop && !_spawnWorkshop)
            {
                return;
            }

            Room shopRoom = GameManager.Instance.RoomsManager.Rooms.Find(r => r.RoomType == RoomType.Shop);

            if (_spawnShop)
            {
                InteractableArea shop = Instantiate(_shop, shopRoom.transform);
                Vector3 shopPosition = shopRoom.Spawner.SpawnPoints.GetRandomSpawnPoint(true).position;
                shop.transform.eulerAngles = Vector3.zero;
                shop.transform.position = shopPosition;
            }

            if (_spawnWorkshop)
            {
                InteractableArea workshop = Instantiate(_workshop, shopRoom.transform);
                workshop.transform.eulerAngles = Vector3.zero;
                Vector3 workshopPosition = shopRoom.Spawner.SpawnPoints.GetRandomSpawnPoint(true).position;
                workshop.transform.position = workshopPosition;
                GameManager.Instance.ShopsManager.SetWorkShop(workshop.GetComponent<Workshop_InteractableArea>());
            }
        }

        public void SetSpawnWorkshop(bool spawn)
        {
            _spawnWorkshop = spawn;
        }
    }
}
