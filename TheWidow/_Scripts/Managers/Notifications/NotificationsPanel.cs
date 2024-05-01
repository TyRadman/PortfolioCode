using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsPanel : MonoBehaviour
{
    public static NotificationsPanel Instance;
    [SerializeField] private Text m_NotificationText;
    [SerializeField] private float m_DisplayDuration = 2f;
    [SerializeField] private float m_MinimumDisplayDuration = 0.5f;
    [SerializeField] private AnimationClip m_FadeInClip;
    [SerializeField] private AnimationClip m_FadeOutClip;
    [SerializeField] private Animation m_Anim;
    private WaitForSeconds m_DisplayDurationWait;
    private WaitForSeconds m_DisplayMinDurationWait;
    private WaitForSeconds m_AnimationWait;
    private Coroutine m_DisplayingCoroutine;
    private List<NotificationType> m_HoldingNotifications = new List<NotificationType>();

    [System.Serializable]
    struct NotificationMessages
    {
        public NotificationType Type;
        [TextArea(1, 2)]
        public string Message;
    }
    [SerializeField] private List<NotificationMessages> m_Messages;

    public enum NotificationType
    {
        NewObjective, ObjectiveFinished, NewItem, NewSideObjective
    }

    private void Awake()
    {
        Instance = this;
        // turn off the text on start
        playAnimation(m_FadeOutClip);
        // initialize values for the coroutines
        m_DisplayDurationWait = new WaitForSeconds(m_DisplayDuration);
        m_DisplayMinDurationWait = new WaitForSeconds(m_MinimumDisplayDuration);
        m_AnimationWait = new WaitForSeconds(m_FadeOutClip.length);
    }

    public void ShowNotification(NotificationType _notificationType)
    {
        // we add this notification to the que
        m_HoldingNotifications.Add(_notificationType);

        // if there are no notifications displayed then we display this notification
        if (m_DisplayingCoroutine == null)
        {
            m_DisplayingCoroutine = StartCoroutine(displayNotification(_notificationType));
        }
        // if the notifications window is displaying 
        else
        {
            StopCoroutine(m_DisplayingCoroutine);
            m_DisplayingCoroutine = StartCoroutine(displayNotificationShort(m_HoldingNotifications[0], false));
        }
    }

    IEnumerator displayNotification(NotificationType _type)
    {
        // print the message of the corresponding notification type
        m_NotificationText.text = m_Messages.Find(m => m.Type == _type).Message;
        // play audio
        AudioManager.Instance.PlayAudio("Notification Ping", null, true);
        // play the fade in animation
        playAnimation(m_FadeInClip);
        // we wait until the display time is over
        yield return m_DisplayDurationWait;
        // play the fade out animation
        playAnimation(m_FadeOutClip);
        // empty the coroutine to mark it's over
        m_DisplayingCoroutine = null;
        // remove the notification from the que
        m_HoldingNotifications.RemoveAt(0);
    }

    IEnumerator displayNotificationShort(NotificationType _type, bool _fadeIn)
    {
        // print the message of the corresponding notification type
        m_NotificationText.text = m_Messages.Find(m => m.Type == _type).Message;
        
        if (_fadeIn)
        {
            AudioManager.Instance.PlayAudio("Notification Ping", null, true);
            playAnimation(m_FadeInClip);
        }

        yield return m_DisplayMinDurationWait;
        playAnimation(m_FadeOutClip);
        // wait until the animation is done playing 
        yield return m_AnimationWait;
        m_DisplayingCoroutine = null;
        m_HoldingNotifications.RemoveAt(0);

        // if there is one notification left to print then we go back to the slow style, otherwise, we keep the fast run
        if (m_HoldingNotifications.Count == 1)
        {
            m_DisplayingCoroutine = StartCoroutine(displayNotification(m_HoldingNotifications[0]));
        }
        else
        {
            m_DisplayingCoroutine = StartCoroutine(displayNotificationShort(m_HoldingNotifications[0], true));
        }
    }

    private void playAnimation(AnimationClip _clip)
    {
        m_Anim.Stop();
        m_Anim.clip = _clip;
        m_Anim.Play();
    }
}
