using UnityEngine;
using UnityEngine.Events;

public class ObservedItem : InteractionClass, I_ObservedObject
{
    #region Interface variables
    public bool ItemHeld { get; set; }
    public ItemInfo Info { get => m_Info; set { m_Info = value; } }
    [SerializeField] private ItemInfo m_Info;
    #endregion

    [SerializeField] private float m_DistanceFromCamera;
    [SerializeField] private bool m_HasEvent = false;
    [SerializeField] private UnityEvent m_OnObservedEvent;

    private void Start()
    {
        ItemHeld = false;
    }

    protected override void Interact()
    {
        base.Interact();            // basic interaction functionalities

        PlayerObjectsInteraction.Instance.Observe(transform, ItemHeld, m_DistanceFromCamera);  // pull the object closer
        
        ItemHeld = !ItemHeld;                   // change the state

        SwapActionMessages();                   // swaps the action text with the second action text

        if (ItemHeld)
        {
            CanInteract = false;        // we can't interact with other objects if the item is held
        }
    }

    public void AfterObservationInteraction()
    {
        StartCoroutine(StoryTelling.DisplayMessages(m_Messages.m_AfterInteractionMessages));
        //AfterInteractionMessagesDisplay();                  // to display any messages there are after interacting with the object
        PlayerObjectsInteraction.Instance.ReleaseObject(); // putting the object back (not all objects are put back in the game tho, this piece of code does it)
        SwapActionMessages();                               // returns the first action text

        if (m_HasEvent)     // if there's an event to occur after interacting with this object
        {
            m_OnObservedEvent?.Invoke();
            m_HasEvent = false;                               // to ensure the event doesn't get triggered twice by the same object. This can be changed in the future
        }

        if (ItemHeld)
        {
            CanInteract = false;
        }
    }
}