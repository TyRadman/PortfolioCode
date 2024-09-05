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
        [SerializeField] private bool _spawnWorkshopAnyways = true;

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
            
            //TODO: temp, must remove the players count check
            int playersCount = GameManager.Instance.PlayersTempInfoSaver.PlayersCount;

            if ((_spawnWorkshop && playersCount == 2) || _spawnWorkshopAnyways)
            {
                InteractableArea workshop = Instantiate(_workshop, shopRoom.transform);
                workshop.transform.eulerAngles = Vector3.zero;
                Vector3 workshopPosition = shopRoom.Spawner.SpawnPoints.GetRandomSpawnPoint(true).position;
                workshop.transform.position = workshopPosition;
                GameManager.Instance.ShopsManager.SetWorkShop(workshop.GetComponent<Workshop_InteractableArea>());
            }
        }
    }
}
