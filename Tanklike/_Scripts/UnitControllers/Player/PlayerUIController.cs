using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerUIController : MonoBehaviour, IController
    {
        [field: SerializeField] public PlayerInventoryController InventoryController { set; get; }
        [field: SerializeField] public PlayerPauseMenuController PauseMenuController { set; get; }
        private PlayerComponents _components;

        public void SetUp(PlayerComponents components)
        {
            _components = components;
            InventoryController.SetUp(components);
            PauseMenuController.SetUp(components);
        }

        public void EnableInventoryController(bool enable)
        {
            if (enable)
            {
                InventoryController.SetUpInput(_components.PlayerIndex);
            }
            else
            {
                InventoryController.DisposeInput(_components.PlayerIndex);
            }
        }

        public void EnablePauseMenuController(bool enable)
        {
            if (enable)
            {
                PauseMenuController.SetUpInput(_components.PlayerIndex);
            }
            else
            {
                PauseMenuController.DisposeInput(_components.PlayerIndex);
            }
        }

        #region IController
        public bool IsActive => throw new System.NotImplementedException();

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void Dispose()
        {
        }

        public void Restart()
        {
        }
        #endregion
    }
}
