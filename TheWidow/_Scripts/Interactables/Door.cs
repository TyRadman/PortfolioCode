using UnityEngine;

public class Door : InteractionClass
{
    private enum Type
    {
        Door, Shelf
    }

    [SerializeField] private Type m_ObjectType;

    [TextArea]
    public Key.KeyType UnlockingKey = Key.KeyType.none;
    private GameObject m_WalkingBlocker;
    private Animator m_Anim;
    private AudioSource m_AudioSource;
    public delegate void OnObjectInteracted(Key.KeyType _type);
    public event OnObjectInteracted OnObjectInteraction;
    public string LockedMessage;
    private const string OPENED = "opened";
    private const string TRY_TO_OPEN_TRIGGER = "tryToOpen";
    public bool IsLocked = false;
    [HideInInspector] public bool m_IsOpened = false;                                   // the current state of the door
    private bool m_AllowedToOpen = true;                                                // false during the opening process

    public override bool IsActivated
    {
        get => m_IsActivated;
        set
        {
            m_IsActivated = value;
            OnObjectInteraction?.Invoke(value ? UnlockingKey : Key.KeyType.none);
        }
    }

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Anim = GetComponentInParent<Animator>();

        if (m_ObjectType == Type.Door)
        {
            m_WalkingBlocker = transform.GetChild(1).gameObject;
            m_WalkingBlocker.SetActive(IsLocked);
        }

        InventoryVars.m_InventoryUsage = IsLocked;
        addToKeyDoors();
    }

    // adds this to the inventory functionality since keys can't be used unless a door that can be opened with that key is close enough to the player
    void addToKeyDoors()
    {
        InventoryKey[] keysHeld = FindObjectsOfType<InventoryKey>();

        if (keysHeld.Length > 0)
        {
            for (int i = 0; i < keysHeld.Length; i++)
            {
                keysHeld[i].AddDoor(this);
                OnObjectInteraction += keysHeld[i].CheckKeys;
            }
        }
    }

    protected override void Interact()
    {
        if (m_AllowedToOpen)
        {
            base.Interact();                                                        // performs the the virtual function
            if (!IsLocked && m_AllowedToOpen)                                       // functionalities related to the door
            {
                m_AllowedToOpen = false;
                Invoke(nameof(refreshReusability), m_Anim.GetCurrentAnimatorStateInfo(0).length);         // enabling and disabling player to open or close the door while its
                ChangeDoorState(false);                                                                   // turns the door's state to its opposite state
                makeDoorNoise();                                                                          // alert enemy if possible otherwise catch
            }
            else                                                                                          // if the door is locked
            {
                m_Anim.SetTrigger(TRY_TO_OPEN_TRIGGER);                                                           // plays the locked door animation
                DialogueManager.Instance.TypeMessage(LockedMessage);                                                    // plays the dialogue message
            }
        }
    }

    public void UnlockDoor()
    {
        IsLocked = false;
        InventoryKey.Doors.Remove(this);                                                        // remove this from the doors that new keys will subscribe to
        OnObjectInteraction(Key.KeyType.none);                                                            // sets the requirement to open this door as none
        System.Array.ForEach(FindObjectsOfType<InventoryKey>(), k => OnObjectInteraction -= k.CheckKeys); // unsubscribes all methods from this door
        InventoryVars.m_InventoryUsage = false;                                                                         // no longer checks for activation
        InteractionEvent -= activationCheck;                                                              // unsubscribes function that toggles activation (the bool that allows the player to interact using the inventory)
        AudioManager.Instance.PlayAudio(AudioName[3], m_AudioSource, true);                               // plays the audio
        Destroy(m_WalkingBlocker);                                                                        // destroys the walking blocker that prevents the enemy from passing by the door
    }

    public override void DisplayActionMessage(string _temp = "") => ActionText.text = ActionMessage[m_IsOpened ? 1 : 0];

    public override void PlayAudio()
    {
        if (!IsLocked)
        {
            AudioManager.Instance.PlayAudio(AudioName[m_IsOpened ? 1 : 0], m_AudioSource, true);
        }
        else
        {
            AudioManager.Instance.PlayAudio(AudioName[2], m_AudioSource, true);
        }
    }

    public void ChangeDoorState(bool _play = true)
    {
        if (_play)
        {
            PlayAudio();
        }

        m_Anim.SetBool(OPENED, !m_IsOpened);
        m_IsOpened = !m_IsOpened;
    }

    void makeDoorNoise()
    {
        // if the hearing distance between the enemy and the destination was within the range then the enemy will hear and the condition will return true
        if (EnemiesManager.Instance.EnemiesExistInLevel())
        {
            EnemiesManager.Instance.TriggerHearing(transform.position);
        }
    }
 
    // so that the door's state can't be changed while it's being opened and closed
    void refreshReusability()
    {
        m_AllowedToOpen = true;
    }
}