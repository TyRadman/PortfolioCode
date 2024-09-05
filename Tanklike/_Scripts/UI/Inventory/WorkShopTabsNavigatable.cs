using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using UnityEngine.InputSystem;
using System;

namespace TankLike.UI.Workshop
{
    using Signifiers;
    using Inventory;
    using TankLike.Sound;

    public class WorkShopTabsNavigatable : Navigatable, IInput
    {
        private TabReferenceUI _selectedTab;
        [SerializeField] private int _selectedTabIndex = 0;
        [Header("References")]
        [SerializeField] private List<TabReferenceUI> _tabs;
        [SerializeField] private List<TabReferenceUI> _oldTabs;
        [SerializeField] private GameObject _content;
        [SerializeField] private UIActionSignifiersController _menuActionSignifiersController;
        [Header("Tab Heads Settings")]
        [SerializeField] private TabActivationSettings _enableTabSettings;
        [SerializeField] private TabActivationSettings _disableTabSettings;
        [SerializeField] private TabActivationSettings _selectedTabSettings;
        public Action OnWorkShopClosed { get; set; }

        private const string RETURN_ACTION_TEXT = "Return";

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

            //GameManager.Instance.InputManager.EnablePlayerInput();
            _tabs.ForEach(t => t.Navigator.SetUp());
            _tabs.ForEach(t => t.Navigator.SetUpActionSignifiers(_menuActionSignifiersController));
        }

        #region Input Set up
        public void SetUpInput(int playerIndex)
        {
            GameManager.Instance.InputManager.DisableInputs((playerIndex + 1) % 2);
            GameManager.Instance.InputManager.EnableUIInput(playerIndex);

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            // TODO: Add callback context to the methods
            UIMap.FindAction(c.UI.Cancel.name).performed += (context) => Return();
            UIMap.FindAction(c.UI.Tab_Left.name).performed += (context) => SwitchTabs(Direction.Left);
            UIMap.FindAction(c.UI.Tab_Right.name).performed += (context) => SwitchTabs(Direction.Right);
            UIMap.FindAction(c.UI.Exit.name).performed += CloseWorkShop;

            SetUpSignifiers();
        }

        public override void SetUpSignifiers()
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(PlayerIndex, ActionMap.UI);

            // add the actions that are mutual between windows to the signifiers controller
            int returnActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Cancel.name, PlayerIndex);
            _menuActionSignifiersController.DisplaySignifier(RETURN_ACTION_TEXT, Helper.GetInputIcon(returnActionIconIndex));
            _menuActionSignifiersController.SetLastSignifierAsParent();
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

            _menuActionSignifiersController.ClearAllSignifiers();
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

            // Play navigate menu audio
            // TODO: where did the UI Audio go?
            AudioManager audioManager = GameManager.Instance.AudioManager;
            //audioManager.Play(audioManager.UIAudio.NavigateMenuAudio);
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
