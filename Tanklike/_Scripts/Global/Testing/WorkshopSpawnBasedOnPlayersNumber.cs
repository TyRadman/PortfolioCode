using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class WorkshopSpawnBasedOnPlayersNumber : MonoBehaviour
    {
        public void EnableShopBasedOnPlayersCount()
        {
            bool spawnShop = GameManager.Instance.InputManager.ControlsStarter.PlayersCount != 1;
            GameManager.Instance.LevelGenerator.ShopsBuilder.SetSpawnWorkshop(spawnShop);
        }
    }
}
