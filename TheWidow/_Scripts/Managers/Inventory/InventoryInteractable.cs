using UnityEngine;

// the base class that holds all data for items that can be kept in the inventory
// this class also uses two interfaces, one is responsible for the attributes of the items and the second is for the basic functionalities that the items can have like being used
public class InventoryInteractable : MonoBehaviour, i_InventoryItem_Info
{
    #region Variables

    #region i_InventoryItem_Info variables
    public int Index { get; set; }
    [SerializeField] protected Sprite icon;
    [TextArea] [SerializeField] protected string description;
    [SerializeField] protected new ItemName tag;
    [SerializeField] protected string m_ItemName;
    [SerializeField] protected bool m_IsEquipped = false;
    public Sprite Icon { get => icon; set => icon = value; }
    public ItemName Tag { get => tag; set => tag = value; }
    public string TheItemName { get => m_ItemName; set => m_ItemName = value; }
    public string Description { get => description; set => description = value; }
    public bool IsEquipped { get => m_IsEquipped; set => m_IsEquipped = value; }
    #endregion

    // audio to play when the item is used
    [SerializeField] private string m_Clip;
    [SerializeField] protected string[] m_ErrorMessages;
    #endregion

    public virtual void OnUsed()
    {
        // play audio on use
        if (m_Clip != null)
        {
            AudioManager.Instance.PlayAudio(m_Clip, null, true);
        }

        InventorySlot slot = Inventory.Instance.GetSlotByName(Tag);
        slot.AddItem(-1);
    }
}