using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class InteractionClass : MonoBehaviour, i_ObjectiveItem
{
    #region Variables
    #region Interaction arrow related variables
    public static Transform m_ActiveInteractable;
    [SerializeField] private Transform m_ArrowPointingPosition;
    #endregion
    public static bool CanInteract = true;
    public float InteractionDistance = 2f;
    // by making it static, we will have one reference to it for all interactable objects, saving us tons of memory since the text class is a big and complex one
    public static Text ActionText;            
    [Tooltip("What is displayed in the action text when the object is close enough to be interacted with.")]
    [TextArea] public string[] ActionMessage;
    [Tooltip("Audio names can be found in audio manager")]
    public string[] AudioName;
    [Tooltip("If an item can be stored in the inventory, drag its inventory based file and toggle the bool.")]
    public InventoryVariables InventoryVars;
    // whether we're close enough to interact with it
    public bool Interactable => PlayerSight.DistanceFromPlayer <= InteractionDistance;
    // being looked at and is interactable
    protected bool m_IsActivated = false;                       
    public delegate void InteractionDel();
    // an event called every time an item is hovered over by the mouse
    // used to control functionalities i.e. adding and removing functions based on needs
    public event InteractionDel InteractionEvent;

    public virtual bool IsActivated
    {
        get => m_IsActivated;
        set
        {
            m_IsActivated = value;
        }
    }
    #endregion

    #region Interaction messages
    [System.Serializable]
    class InteractionMessages
    {
        [Header("Messages")]
        public bool m_HasLookAtMessage = false;
        [TextArea] public string[] m_Messages;
        [TextArea] public string m_PreInteractionActionMessage;
        [TextArea] public string[] m_AfterInteractionMessages;
    }
    [SerializeField]
    [Tooltip("The message/s displayed after the item is interacted with.")]
    private InteractionMessages m_Messages;    // if there will be a message to display before interaction
    #endregion

    #region Initializers
    void Awake()
    {
        // temporary
        if(m_ArrowPointingPosition == null)
        {
            m_ArrowPointingPosition = transform;
        }

        ActionText = GameObject.FindGameObjectWithTag("ActionText").GetComponent<Text>();

        ClearText();                                            // clears the text on start

        InteractionEvent += displayTip;

        if (!m_Messages.m_HasLookAtMessage)
        {
            InteractionEvent += interactionMainFunc;
        }
        else
        {
            InteractionEvent += displayMessages;
        }

        if (InventoryVars.m_InventoryUsage)                       // if inventory items affect it
        {
            InteractionEvent += activationCheck;    // keep checking when it becomes activated so that inventory items are notified
        }
    }
    #endregion

    #region Objectives
    [Header("Objective Variables")]
    // objectives variables
    public ItemName ItemTag;
    [SerializeField] private bool m_IsObjective = false;
    [HideInInspector] public bool IsObjective { get => m_IsObjective; set => m_IsObjective = value; }
    #endregion

    protected virtual void Interact()
    {
        PlayAudio();
        ClearText();

        // for objectives stuff
        if (IsObjective)
        {
            ObjectivesManager.Instance.ItemCollected(ItemTag);
        }
    }

    public virtual void OnMouseOver()
    {
        if (Inventory.Instance.IsOpened) // to prevent it from making changes when the inventory is opened
        {
            return;
        }

        InteractionEvent();
    }

    public virtual void OnMouseExit()
    {
        m_ActiveInteractable = null;

        if (Inventory.Instance.IsOpened)
        {
            return;
        }

        if (InventoryVars.m_InventoryUsage && IsActivated)
        {
            IsActivated = false;
        }

        ClearText();
    }

    public virtual void DisplayActionMessage(string _message = "1")
    {
        ActionText.text = _message == "1" ? ActionMessage[0] : _message;
    }

    public virtual void PlayAudio()
    {
        if (AudioName.Length > 0)
        {
            AudioManager.Instance.PlayAudio(AudioName[0], GetComponent<AudioSource>(), true);
        }
    }

    protected void activationCheck()
    {
        IsActivated = Interactable; // ensures the door is activated (in terms of inventory interaction) only if the player can interact with it
    }        
    
    void interactionMainFunc()
    {
        if (Interactable)
        {
            if(m_ActiveInteractable == null)
            {
                m_ActiveInteractable = m_ArrowPointingPosition;
                InteractionArrow.DisplayArrow();
            }

            m_ActiveInteractable = m_ArrowPointingPosition;           // sets this object as the active one for the arrow to point at

            DisplayActionMessage();                     // displays the action text

            if (Input.GetKeyDown(KeyCode.E) && CanInteract)
            {
                Interact();
            }
        }
        else
        {
            m_ActiveInteractable = null;                // if the object is no longer interactable then the arrow will no longer point at it
            ClearText();
        }
    }

    private void displayTip()
    {
        if (Interactable)    // tips and tutorial
        {
            TipsManager.Instance.DisplayTip(Tips.TipsTag.Interaction);
            InteractionEvent -= displayTip;
        }
    }

    void displayMessages()                                                  // if an object triggers a message before getting interacted with
    {
        if (Interactable)
        {
            DisplayActionMessage(m_Messages.m_PreInteractionActionMessage);

            if (Input.GetKeyDown(KeyCode.E) && CanInteract)
            {
                StartCoroutine(StoryTelling.DisplayMessages(m_Messages.m_Messages));
                InteractionEvent -= displayMessages;                        // not more message will be displayed
                InteractionEvent += interactionMainFunc;                    // player can now pick up or interact
            }
        }
        else
        {
            ClearText();
        }
    }

    public void ClearText()
    {
        ActionText.text = "";
    }
    // use this function in children after interaction in the interaction method
    protected void AfterInteractionMessagesDisplay()
    {
        StartCoroutine(StoryTelling.DisplayMessages(m_Messages.m_AfterInteractionMessages));
    }

    public static Transform GetActiveInteractable()
    {
        return m_ActiveInteractable;
    }

    protected void SwapActionMessages()     // to change the message displayed (so far used only for the letter)
    {
        var temp = ActionMessage[0];
        ActionMessage[0] = ActionMessage[1];
        ActionMessage[1] = temp;
    }
}

#region Interfaces
public interface I_ObservedObject
{
    bool ItemHeld { get; set; }
    ItemInfo Info { get; }
    void AfterObservationInteraction();
}

public interface I_Lockable
{
    bool IsLocked { get; set; }
}
#endregion

[System.Serializable]
public class InventoryVariables
{
    public GameObject ItemInInventory;
    [HideInInspector] public bool InteractableWithInventory;
    public bool m_InventoryUsage;                               // can be interacted with using the inventory
}