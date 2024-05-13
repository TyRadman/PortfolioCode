using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using TMPro;

namespace TankLike.UI.Inventory
{
    /// <summary>
    /// Also known as the inventory, but can be used for any tabs manager (but we won't do so)
    /// </summary>
    public class TabsManager : Navigatable
    {
        public enum InputState
        {
            TabHeads, TabContent
        }

        private TabReferenceUI _selectedTab;
        [SerializeField] private int _selectedTabIndex = 0;
        [Header("References")]
        [SerializeField] private List<TabReferenceUI> _tabs;
        [SerializeField] private List<TabReferenceUI> _oldTabss;
        [SerializeField] private GameObject _content;
        [SerializeField] private TextMeshProUGUI _switchLeftKeyText;
        [SerializeField] private TextMeshProUGUI _switchRightKeyText;
        [Header("Tab Heads Settings")]
        [SerializeField] private TabActivationSettings _enableTabSettings;
        [SerializeField] private TabActivationSettings _disableTabSettings;
        [SerializeField] private TabActivationSettings _selectedTabSettings;

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
            _tabs.ForEach(t => t.Navigator.SetUp());
        }

        #region Open and Close
        public override void Open(int playerIndex = 0)
        {
            SetPlayerIndex(playerIndex);
            // disable gameplay and enable UI input
            GameManager.Instance.InputManager.EnableUIInput(true);

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

            base.Open(playerIndex);
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsActive) return;

            GameManager.Instance.InputManager.EnablePlayerInput(true);

            _content.SetActive(false);
            ResetTabs();
            base.Close(playerIndex);
        }
        #endregion

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

            // Open the highlighted tab
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

        public void ResetTabs(int playerIndex = 0)
        {
            if (!IsActive)
            {
                return;
            }

            // dehighlight the selected tab head
            _selectedTab.Tab.HighLight(_disableTabSettings);
            // deactivate the tab's content
            if (_selectedTab.Content != null) _selectedTab.Content.SetActive(false);

            // dehighlight the previously selected tab
            _selectedTab.Navigator.Close(PlayerIndex);
            // select the next tab depending on the direction
            _selectedTabIndex = 0;
            _selectedTab = _tabs[_selectedTabIndex];
            _selectedTab.Tab.HighLight(_enableTabSettings);

            // change the tab's text size and color to indicate that the control went over to the content of the tab
            _selectedTab.Tab.HighLight(_selectedTabSettings);

            // display the content of the tab
            if (_selectedTab.Content != null)
            {
                _selectedTab.Content.SetActive(true);
            }
        }
    }
}
