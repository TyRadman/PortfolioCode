using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UI.Inventory;
using TankLike.UI;
using TankLike.Environment;

namespace TankLike
{
    using UI.Workshop;

    public class ShopsManager : MonoBehaviour, IManager
    {
        [SerializeField] private TabsManager _inventory;
        [SerializeField] private List<WorkShopTabsNavigatable> _workshops;
        [SerializeField] private ToolsNavigator _toolsShop;

        [field: SerializeField] public Workshop_InteractableArea WorkShopArea;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            if (_inventory != null)
            {
                _inventory.SetUp();
            }

            if (_workshops[0] != null)
            {
                _workshops[0].SetUp();
            }

            if (_toolsShop != null)
            {
                _toolsShop.SetUp();
            }
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void SetWorkShop(Workshop_InteractableArea workshop)
        {
            WorkShopArea = workshop;
        }
    }
}
