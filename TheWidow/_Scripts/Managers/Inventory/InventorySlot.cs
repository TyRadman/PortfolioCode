using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [HideInInspector] public ItemName NameOfItem;
    [SerializeField] private Text m_NumberText;
    [HideInInspector] public bool Occupied = false;
    private i_InventoryItem_Info m_ItemInfo;
    public Image Icon;
    [HideInInspector] public int ItemsNumber;

    private void Awake()
    {
        Icon.enabled = false;
        m_NumberText.enabled = false;
    }

    // the function triggered when the item is pressed
    public void SelectSlot()
    {
        // sets the item's info and displays it
        if (m_ItemInfo != null)
        {
            Inventory.Instance.SetUpSelectionDisplay(m_ItemInfo);
        }
        else
        {
            Inventory.Instance.EnableButtons(false);
        }
    }

    public void OccupySlot(i_InventoryItem_Info _info, ItemName _name)
    {
        m_ItemInfo = _info;
        NameOfItem = _name;
        Occupied = true;

        if (!Icon.enabled)
        {
            Icon.enabled = true;
        }

        Icon.sprite = m_ItemInfo.Icon;
    }

    // to set the number of a particular item that 
    public void AddItem(int _amount)
    {
        ItemsNumber += _amount;

        if(ItemsNumber < 2)
        {
            m_NumberText.enabled = false;
        }
        else
        {
            m_NumberText.enabled = true;
            m_NumberText.text = ItemsNumber.ToString();
        }

        // if there are no more items
        if(ItemsNumber == 0)
        {
            Icon.enabled = false;
            m_NumberText.enabled = false;
            NameOfItem = ItemName.none;
            Occupied = false;
            m_ItemInfo = null;
        }
    }
}
