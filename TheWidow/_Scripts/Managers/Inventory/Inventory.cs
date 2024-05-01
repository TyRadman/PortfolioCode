using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#region Interfaces
public interface i_InventoryItem_Info
{
    Sprite Icon { get; set; }
    ItemName Tag { get; set; }
    string TheItemName { get; set; }
    string Description { get; set; }
    bool IsEquipped { get; set; }
    void OnUsed();
}
#endregion

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private List<InventorySlot> m_Slots = new List<InventorySlot>();
    private i_InventoryItem_Info m_SelectedSlot;
    private int m_Index = -1;
    [HideInInspector] public bool IsOpened = false;
    [HideInInspector] public List<i_InventoryItem_Info> ItemsFunctionalities = new List<i_InventoryItem_Info>();
    private Transform m_InventoryItemsParent;
    
    [Header("References")]
    [SerializeField] private Canvas m_InventoryCanvas;
    [SerializeField] private Canvas m_GamePlayCanvas;
    [SerializeField] private Text m_NameText;
    [SerializeField] private Text m_DescriptionText;
    [SerializeField] private Button m_UseButton;
    [SerializeField] private Button m_DropButton;
    [SerializeField] private Text m_UseButtonText;
    [SerializeField] private GameObject ItemPrefab; // contains the itemInventory component
    [SerializeField] private Transform m_SlotsParent;
    [SerializeField] private Animator m_Anim;
    [Header("Error Message")]
    [SerializeField] private string m_TriggerName;
    [SerializeField] private Text m_ErrorText;
    [SerializeField] private Animator m_ErrorAnim;
    [Header("Input")]
    [SerializeField] private KeyTypes m_InputKey;
    [Header("Others")]
    [SerializeField] private float m_MinTimeScale = 0.05f;
    [SerializeField] private float m_LerpAmount = 0.2f;

    private void Awake()
    {
        Instance = this;
        // clear the text
        m_NameText.text = string.Empty;
        m_DescriptionText.text = string.Empty;
        // sets the function to the button which will have its content changed based on what slot is selected
        m_UseButton.onClick.AddListener(useButton);
    }

    void Start()
    {
        // assigning inventory related functionality to the input manager
        InputManager.Instance.SetUpButton(m_InputKey, _down: toggleInventory);
        // disabling the inventory on start in case it was open while editing
        if(m_InventoryCanvas.enabled)
        {
            m_InventoryCanvas.enabled = false;
        }
        // caching all slots
        for (int i = 0; i < m_SlotsParent.childCount; i++)
        {
            m_Slots.Add(m_SlotsParent.GetChild(i).GetComponent<InventorySlot>());
        }
        // caching the parent of all collected items programmatically
        m_InventoryItemsParent = new GameObject("Inventory Items").transform;
        // disables the use and drop buttons because the player will start with no items at the inventory
        EnableButtons(false);
    }

    void toggleInventory()
    {
        // changes the inventory state to the opposite
        IsOpened = !IsOpened;           
        // blurs the screen
        PlayerEffects.Instance.BlurAllScreen(IsOpened);
        // setting values based on the inventory's status
        bool cursorIsVisible = IsOpened;
        bool movementAllowed = !IsOpened;
        CursorLockMode cursorMode = IsOpened? CursorLockMode.Confined : CursorLockMode.Locked;
        // switches the player's canvas to the opposite state (stamina, battery etc..)
        m_GamePlayCanvas.enabled = !m_GamePlayCanvas.enabled;
        // plays animation
        m_Anim.SetBool("Opened", IsOpened);
        // changes the cursor state (hide and disable or show and enable)
        GameManager.Instance.changeCursorState(cursorIsVisible, cursorMode);
        // changes the time scale (freeze or free time)
        if (IsOpened)
        {
            StartCoroutine(changeTimeScale(m_MinTimeScale));
        }
        else
        {
            Time.timeScale = 1f;
        }

        PlayerMovement.Instance.AllowMovement(movementAllowed);                                    
    }

    public void AddItem(GameObject _item, ItemName _name)
    {
        // if the item is the first of a kind in the inventory
        if (!m_Slots.Exists(i => i.NameOfItem == _name) || m_Slots.Count == 0)
        {
            // gets the index of the first free slot in the inventory
            m_Index = getNextFreeIndex();
            // instantiates the object that holds the item's in inventory info
            var itemFunc = Instantiate(_item, m_InventoryItemsParent).GetComponent<i_InventoryItem_Info>();
            // give it its name
            itemFunc.Tag = _name;
            // occupies the slot with the item info
            m_Slots[m_Index].OccupySlot(itemFunc, _name);
            // increase the number of items in the selected slot
            m_Slots[m_Index].AddItem(1);
        }
        // if it already exists
        else
        {
            // if there is already a slot that has this item then we increase the number of items that the slot has
            m_Slots.Find(s => s.NameOfItem == _name).AddItem(1);
        }
    }

    int getNextFreeIndex()
    {
        // returns the first available slot
        return m_Slots.FindIndex(s => s.Occupied == false);                                  
    }

    #region Buttons Setup
    public void SetUpSelectionDisplay(i_InventoryItem_Info _info)
    {
        // sets the name of the item
        m_NameText.text = _info.TheItemName;
        // sets the description of the item
        m_DescriptionText.text = _info.Description;
        // sets the functionality of the selected item to the "use" button
        EnableButtons(true);
        // if it's an equipped item then change the use button text
        m_UseButtonText.text = _info.IsEquipped ? "Equipped" : "Use";
        // if the item is equipped then the use button is disabled but not deactivated (not hidden)
        m_UseButton.enabled = !_info.IsEquipped;
        m_SelectedSlot = _info;
    }

    // the main function for the use button
    private void useButton()
    {
        if (m_SelectedSlot != null)
        {
            m_SelectedSlot.OnUsed();
        }
    }

    public void EnableButtons(bool _enable)
    {
        if (_enable)
        {
            if (!m_UseButton.gameObject.activeSelf)
            {
                m_UseButton.gameObject.SetActive(true);
            }

            if (!m_DropButton.gameObject.activeSelf)
            {
                m_DropButton.gameObject.SetActive(true);
            }
        }
        else
        {
            m_UseButton.gameObject.SetActive(false);
            m_DropButton.gameObject.SetActive(false);
        }
    }
    #endregion
    public InventorySlot GetSlotByName(ItemName _name)
    {
        return m_Slots.Find(s => s.NameOfItem == _name);
    }

    public void ErrorMessage(string _message)
    {
        // print the message
        m_ErrorText.text = _message;
        // animate the fade in
        m_ErrorAnim.SetTrigger(m_TriggerName);
    }

    #region Visuals
    IEnumerator changeTimeScale(float _targetTimeScale)
    {
        while(IsOpened)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, _targetTimeScale, m_LerpAmount);
            yield return null;
        }
    }
    #endregion

    #region External Use
    public void TurnOffInventory()
    {
        if (IsOpened)
        {
            toggleInventory();
        }
    }
    #endregion
}