using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using UnityEngine.InputSystem;
using System;

namespace TankLike.UI.Inventory
{
    public class WorkShopTabsNavigatable : Navigatable, IInput
    {
        private TabReferenceUI _selectedTab;
        [SerializeField] private int _selectedTabIndex = 0;
        [Header("References")]
        [SerializeField] private List<TabReferenceUI> _tabs;
        [SerializeField] private List<TabReferenceUI> _oldTabs;
        [SerializeField] private GameObject _content;
        [Header("Tab Heads Settings")]
        [SerializeField] private TabActivationSettings _enableTabSettings;
        [SerializeField] private TabActivationSettings _disableTabSettings;
        [SerializeField] private TabActivationSettings _selectedTabSettings;
        public Action OnWorkShopClosed { get; set; }


        private void Awake()
        {
            _selectedTabIndex = 0;
            _selectedTab = _tabs[_selectedTabIndex];
            _content.SetActive(false);

            // deactivate all the tabs
            foreach (TabReferenceUI tab in _tabs)
            {
                tab.Tab.HighLight(_disableTabSettings);
                tab.Tab.SetName(tab.TabName);
                tab.Content.SetActive(false);
            }
        }

        public override void SetUp()
        {
            base.SetUp();
            // close the tab (in case it was opened for editing)
            Close();

            GameManager.Instance.InputManager.EnablePlayerInput(true);
            _tabs.ForEach(t => t.Navigator.SetUp());
        }

        #region Input Set up
        public void SetUpInput(int playerIndex)
        {
            // disable all action maps for the other player
            GameManager.Instance.InputManager.DisableInputs((playerIndex + 1) % 2);
            // enable UI action map for the current player
            GameManager.Instance.InputManager.EnableUIInput(true, playerIndex);

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            //UIMap.FindAction(c.UI.Submit.name).performed += (context) => Select();
            UIMap.FindAction(c.UI.Cancel.name).performed += (context) => Return();
            UIMap.FindAction(c.UI.Tab_Left.name).performed += (context) => SwitchTabs(Direction.Left);
            UIMap.FindAction(c.UI.Tab_Right.name).performed += (context) => SwitchTabs(Direction.Right);
            UIMap.FindAction(c.UI.Exit.name).performed += CloseWorkShop;
        }

        public void DisposeInput(int playerIndex)
        {
            GameManager.Instance.InputManager.EnableInput(ActionMap.Player);
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            //UIMap.FindAction(c.UI.Submit.name).performed -= (context) => Select();
            UIMap.FindAction(c.UI.Cancel.name).performed -= (context) => Return();
            UIMap.FindAction(c.UI.Tab_Left.name).performed -= (context) => SwitchTabs(Direction.Left);
            UIMap.FindAction(c.UI.Tab_Right.name).performed -= (context) => SwitchTabs(Direction.Right);
            UIMap.FindAction(c.UI.Exit.name).performed -= CloseWorkShop;
        }

        private void CloseWorkShop(InputAction.CallbackContext _)
        {
            Close(PlayerIndex);
        }
        #endregion

        #region Open and Close
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
            SetPlayerIndex(playerIndex);
            SetUpInput(PlayerIndex);
            // load all the tabs' content
            _tabs.ForEach(t => t.Navigator.Load(playerIndex));

            // highlight the selected tab
            _selectedTab.Tab.HighLight(_enableTabSettings);

            // enable the content
            _content.SetActive(true);

            // display the content of the tab
            if (_selectedTab.Content != null)
            {
                _selectedTab.Content.SetActive(true);
            }

            // set up the tab highlighted
            _selectedTab.Navigator.Open(PlayerIndex);
            _selectedTab.Navigator.SetActiveCellAsFirstCell();
            // change the tab's text size and color to indicate that the control went over to the content of the tab
            _selectedTab.Tab.HighLight(_selectedTabSettings);
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsActive)
            {
                return;
            }

            OnWorkShopClosed?.Invoke();
            DisposeInput(PlayerIndex);
            _content.SetActive(false);
            base.Close(playerIndex);
        }
        #endregion

        #region Input Methods

        #region Navigation
        public void SwitchTabs(Direction direction)
        {
            if (!IsActive)
            {
                return;
            }

            // dehighlight the selected tab head
            _selectedTab.Tab.HighLight(_disableTabSettings);

            // deactivate the tab's content
            if (_selectedTab.Content != null)
            {
                _selectedTab.Content.SetActive(false);
            }

            // dehighlight the previously selected tab
            _selectedTab.Navigator.Close(PlayerIndex);
            // select the next tab depending on the direction
            _selectedTabIndex = Helper.AddInRange(direction == Direction.Right ? 1 : -1, _selectedTabIndex, 0, _tabs.Count - 1);
            _selectedTab = _tabs[_selectedTabIndex];
            _selectedTab.Tab.HighLight(_enableTabSettings);

            // set up the tab highlighted
            _selectedTab.Navigator.Open(PlayerIndex);
            _selectedTab.Navigator.SetActiveCellAsFirstCell();
            // change the tab's text size and color to indicate that the control went over to the content of the tab
            _selectedTab.Tab.HighLight(_selectedTabSettings);

            _selectedTab.Navigator.IsActive = true;

            // display the content of the tab
            if (_selectedTab.Content != null)
            {
                _selectedTab.Content.SetActive(true);
            }
        }
        #endregion

        #region Return
        public override void Return()
        {
            base.Return();

            if (!IsActive)
            {
                return;
            }

            Close();
        }
        #endregion

        #endregion
    }
}
