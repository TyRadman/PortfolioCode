using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Variables
    public static DialogueManager Instance;
    [SerializeField] private Text m_DialogueText;
    [SerializeField] private Text m_StoryText;
    [SerializeField] private Animation m_Animtation;
    [SerializeField] private AnimationClip m_FadeInAnimationClip;
    [SerializeField] private AnimationClip m_FadeOutAnimationClip;
    [SerializeField] private Animator m_Anim;
    [HideInInspector] public bool Free = true; // determines whether the dialogue is displaying something currently
    private int m_PreviousPriority = 0;
    private Coroutine m_TypingCoroutine;
    #endregion

    private void Awake()
    {
        Instance = this;
        clearTexts();
    }

    private void clearTexts()
    {
        m_DialogueText.text = "";
        m_StoryText.text = "";
    }

    #region Type Message Overloads
    public void TypeMessage(string _message, bool _story = false)     // max size means that the dialogue will cover the entire screen
    {
        // if the message is empty or the dialogue text is not free (a text is being displayed) then return 
        if (!Free || _message.Length == 0)
        {
            return;
        }

        Free = false;

        if (!_story)
        {
            // the dialogue text is a two-lines text displayed at the bottom of the screen
            m_DialogueText.text = _message;
        }
        else
        {
            // the story text covers the entire screen with a black background
            m_StoryText.text = _message;
        }

        // the text fades in slowly 
        m_Anim.SetBool("fadeOut", false);
        m_TypingCoroutine = StartCoroutine(typeMessageMainMethod(_message));
    }

    public void TypeMessage(DialogueMessage _message)     // max size means that the dialogue will cover the entire screen
    {
        // if the message is empty or the dialogue text is not free (a text is being displayed) then return 
        if (!Free || _message.Message.Length == 0 || _message.Priority <= m_PreviousPriority)
        {
            return;
        }

        Free = false;
        // sets the priority of this message
        m_PreviousPriority = _message.Priority;
        // the dialogue text is a two-lines text displayed at the bottom of the screen
        m_DialogueText.text = _message.Message;
        // the text fades in slowly 
        m_Anim.SetBool("fadeOut", false);

        if(m_TypingCoroutine != null)
        {
            StopCoroutine(m_TypingCoroutine);
            m_TypingCoroutine = null;
        }

        m_TypingCoroutine = StartCoroutine(typeMessageMainMethod(_message.Message));
    }
    #endregion

    IEnumerator typeMessageMainMethod(string _message)
    {
        // the duration at which the text is displayed depends on the length of the message. The duration is the square root of the square root of the number of characters that make up the massage (I found this duration to be the most consistent and convinient so far)
        yield return new WaitForSeconds(GetMessageDuration(_message));
        // the text then fades out 
        m_Anim.SetBool("fadeOut", true);
        yield return new WaitForSeconds(1f);
        // the text is cleared and the dialogue is free the use again after the text completely fades out
        clearTexts();
        Free = true;
        // we reset the priority after we're done displaying a message
        m_PreviousPriority = -1000;
    }

    public float GetMessageDuration(string _message)
    {
        return Mathf.Sqrt(Mathf.Sqrt(_message.Length));
    }
}

[System.Serializable]
public struct DialogueMessage
{
    [TextArea(0, 5)]
    public string Message;
    public int Priority;
}