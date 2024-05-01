public class Key : InteractionClass
{
    public delegate void UsingKey(KeyType type);
    private static bool m_EverPickedKey = false;
    public enum KeyType
    {
        mainDoorKey,
        normalKey,
        none
    }
    public KeyType TheKeyType;

    protected override void Interact()
    {
        if (!m_EverPickedKey)   // tutorial and tips 
        {
            m_EverPickedKey = true;
            TipsManager.Instance.DisplayTip(Tips.TipsTag.Inventory);
        }

        base.Interact();
        ClearText();
        Inventory.Instance.AddItem(InventoryVars.ItemInInventory, ItemTag);
        Destroy(gameObject);
    }
}