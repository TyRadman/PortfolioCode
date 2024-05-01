// batteries are used to extend the life of the flashlight
public class Battery : InteractionClass
{
    protected override void Interact()
    {
        base.Interact();
        Inventory.Instance.AddItem(InventoryVars.ItemInInventory, ItemTag);
        ItemsSpawning.Instance.CurrentNumberOfBatteries--;
        Destroy(gameObject);
    }
}