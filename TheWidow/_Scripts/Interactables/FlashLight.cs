using UnityEngine;

public class FlashLight : InteractionClass, I_ObservedObject
{
    // the I_observedObject interface is mutual between all objects that the player can hold and observe
    #region Interface variables
    public bool ItemHeld { get; set; }
    public ItemInfo Info { get => m_Info; set { m_Info = value; } }
    [UnityEngine.SerializeField] private ItemInfo m_Info;
    #endregion

    [SerializeField] private float m_DistanceToCamera = 0.5f;
    protected override void Interact()
    {
        base.Interact();
        // the observation process
        PlayerObjectsInteraction.Instance.Observe(transform, ItemHeld, m_DistanceToCamera);
        ItemHeld = !ItemHeld;

        // if the item is held then the player can't interact with anything
        if (ItemHeld)
        {
            CanInteract = false;
        }
    }

    public void AfterObservationInteraction()
    {
        // add item to inventory
        Inventory.Instance.AddItem(InventoryVars.ItemInInventory, ItemTag);
        // a flashlight tip is displayed after we're done observing it
        TipsManager.Instance.DisplayTip(Tips.TipsTag.FlashLight);     
        // the player picks up the flashlight
        FindObjectOfType<PlayerActions>().GetFlashLight();
        // so that we dont interact with it again
        gameObject.GetComponent<BoxCollider>().enabled = false;         
        // clear the interaction keys text
        ClearText();
        Destroy(gameObject);
    }
}