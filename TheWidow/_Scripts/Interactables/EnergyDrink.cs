public class EnergyDrink : InteractionClass
{
    protected override void Interact()
    {
        base.Interact();
        ClearText();
        Inventory.Instance.AddItem(InventoryVars.ItemInInventory, ItemTag);
        Destroy(gameObject);
    }
}