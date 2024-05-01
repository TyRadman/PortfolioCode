using UnityEngine;

public class Shelf : InteractionClass
{
    enum state
    {
        working, jammed, locked
    }
    [SerializeField] private state m_CurrentState = state.working;

    private bool m_IsOpened = false;
    private bool m_AllowedToOpen = true;
    private Animator m_Anim;

    void Start()
    {
        if (GetComponent<Animator>() != null)
            m_Anim = GetComponent<Animator>();
        else
            m_Anim = GetComponentInParent<Animator>();
    }

    protected override void Interact()
    {
        if (m_CurrentState == state.working)
        {
            if (m_AllowedToOpen)
            {
                m_Anim.SetBool("opened", !m_IsOpened);
                m_IsOpened = !m_IsOpened; // change the door state
                base.Interact();
                m_AllowedToOpen = false;
                Invoke(nameof(refreshReusability), m_Anim.GetCurrentAnimatorStateInfo(0).length);       // to allow it to be used after the animation is done
            }
        }
        else
        {
            DialogueManager.Instance.TypeMessage("I can't open it..");
        }
    }
    public override void DisplayActionMessage(string _temp = "") => ActionText.text = ActionMessage[m_IsOpened ? 1 : 0];

    void refreshReusability() => m_AllowedToOpen = true;

    public override void PlayAudio() => AudioManager.Instance.PlayAudio(AudioName[0], null, true);
}