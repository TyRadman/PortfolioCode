using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UI.Inventory;
using TankLike.UI;
using TankLike.Environment;

namespace TankLike
{
    public class ShopsManager : MonoBehaviour
    {
        [SerializeField] private TabsManager _inventory;
        [SerializeField] private List<WorkShopTabsNavigatable> _workshops;
        [SerializeField] private ToolsNavigator _toolsShop;
        [field: SerializeField] public Workshop_InteractableArea WorkShopArea;

        public void SetUp()
        {
            if (_inventory != null)
            {
                _inventory.SetUp();
            }

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                if (_workshops[i] != null)
                {
                    _workshops[i].SetUp();
                }
            }

            if (_toolsShop != null)
            {
                _toolsShop.SetUp();
            }
        }

        public void SetWorkShop(Workshop_InteractableArea workshop)
        {
            WorkShopArea = workshop;
        }
    }
}
