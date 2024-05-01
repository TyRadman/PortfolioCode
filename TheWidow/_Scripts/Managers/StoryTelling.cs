using System.Collections;
using UnityEngine;

public class StoryTelling : MonoBehaviour
{
    [SerializeField] private bool m_PlayStory = false;
    [Tooltip("When to remove black screen\n(How many story messages will be displayed before the black screen is removed)")]
    [SerializeField] private int m_MessageIndexToRemoveBlackScreen;
    [Tooltip("When to enable player to move\n(How many messages will be displayed before the player can move)")]
    [SerializeField] private int m_MessageIndexToEnableMovement;
    [Header("Story")]
    [SerializeField] private Message[] m_StoryBlocks;

    #region Starting message structure
    [System.Serializable]
    public struct Message
    {
        [TextArea]
        public string MessageText;
        public float Duration;
    }
    [Header("Conversation")]
    [SerializeField] private Message[] m_Messages;
    [SerializeField] private Message m_EndingMessage;
    #endregion

    [Header("Story skip variables")]
    [SerializeField] private GameObject m_SkipText;
    [SerializeField] private float m_DisplayTime = 2f;
    private Coroutine m_SkipInputReader_CO;
    private bool m_SkipStory = false;        // once turned off, the story will stop playing

    public void StartStory()
    {
        if (m_StoryBlocks.Length > 0 && m_PlayStory)
        {
            StartCoroutine(storyTell());
        }
        else
        {
            StartCoroutine(startingConversation());
        }
    }

    private IEnumerator skipStoryInput()
    {
        while (!m_SkipStory)
        {
            if (Input.anyKeyDown)
            {
                m_SkipText.SetActive(true);
                Invoke(nameof(hideSkipText), m_DisplayTime);        // hides the skip text
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                m_SkipStory = true;            // we didn't use the return keyword because if the story wasn't skipped, this coroutine will keep iterating forever
                hideSkipText();
            }

            yield return null;
        }
    }

    private void hideSkipText()
    {
        m_SkipText.SetActive(false);
    }

    IEnumerator storyTell()
    {
        StartCoroutine(skipStoryInput());       // if the stories are played then a coroutine will read the inputs to skip the story if wanted

        int index = 0;
        var wait = new WaitForSeconds(0.1f);

        while (index < m_StoryBlocks.Length && !m_SkipStory)        // if we have stories and we don't want to skip it
        {
            if (DialogueManager.Instance.Free)
            {
                DialogueManager.Instance.TypeMessage(m_StoryBlocks[index++].MessageText, true);
            }

            yield return wait;
        }

        m_SkipStory = true; // after the story is over it is considered skipped so that the reading input coroutine stops

        yield return new WaitForSeconds(2f);

        StartCoroutine(startingConversation());
    }

    IEnumerator startingConversation()                                      // the conversation at the beginning 
    {
        int index = 0;
        var wait = new WaitForSeconds(0.1f);
        bool blackScreenRemoved = false;

        while (index < m_Messages.Length)
        {
            if (index == m_MessageIndexToRemoveBlackScreen && !blackScreenRemoved)
            {
                GameManager.Instance.RemoveBlackScreen();
                blackScreenRemoved = true;
            }

            if (index == m_MessageIndexToEnableMovement)
            {
                GameManager.Instance.AllowPlayerToMove();
            }

            if (DialogueManager.Instance.Free)
            {
                DialogueManager.Instance.TypeMessage(m_Messages[index++].MessageText);
            }

            yield return wait;
        }

        TipsManager.Instance.DisplayTip(Tips.TipsTag.Movement);
        // in case we want it after the messages are over
        if (m_MessageIndexToRemoveBlackScreen >= m_Messages.Length)
        {
            GameManager.Instance.RemoveBlackScreen();
        }

        if (m_MessageIndexToEnableMovement >= m_Messages.Length)
        {
            GameManager.Instance.AllowPlayerToMove();
        }
    }

    public static IEnumerator DisplayMessages(string[] _messages)           // can be used by external classes other than the game manager
    {
        int index = 0;
        var wait = new WaitForSeconds(0.1f);

        while (index < _messages.Length)
        {
            if (DialogueManager.Instance.Free)
            {
                DialogueManager.Instance.TypeMessage(_messages[index++]);
            }

            yield return wait;
        }
    }
}