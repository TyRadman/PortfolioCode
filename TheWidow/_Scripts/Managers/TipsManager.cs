using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
    #region Variables
    public static TipsManager Instance;
    [SerializeField] private Text m_Title;
    [SerializeField] private Text m_Info;
    [SerializeField] private Animator m_Anim;
    [SerializeField] public float m_DisplayDuration = 4f;
    [SerializeField] private float m_TimeBetweenStackingTips = 1.5f;
    public Coroutine m_ClosingWindow;
    [SerializeField] private bool TipsEnabled = true;

    private List<Tips.Tip> m_Tips = new List<Tips.Tip>();
    private List<Tips.Tip> m_StackingTips = new List<Tips.Tip>();
    private Tips.Tip m_CurrentTip;
    private const float MIN_DISPLAY_TIME = 1f;
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Tips.Instance.CopyTips(m_Tips);
    }

    #region Functionality
    public void CheckStackingTips()     // called after whatever interrupted tips displaying is finished, that is the observation class so far.
    {
        if (m_StackingTips.Count == 0 || PlayerObjectsInteraction.Instance.Holding || !TipsEnabled)
        {
            return;
        }

        DisplayTip(m_StackingTips[0].Tag);
    }

    public void DisplayTip(Tips.TipsTag _tag)
    {
        Tips.Instance.UnlockTips(_tag); // unlocks that tip so that it can be displayed on pause

        var tipToDisplay = m_Tips.Find(t => t.Tag == _tag);     // cache the message with the required tag

        if (!m_StackingTips.Contains(tipToDisplay))         // if the tip doesn't exist in the stack
        {
            m_StackingTips.Add(tipToDisplay);            // adds the tip to the stack
        }

        m_CurrentTip = m_StackingTips[0];

        if (!TipsEnabled || PlayerObjectsInteraction.Instance.Holding)   // do nothing if tips are not to be displayed
        {
            return;
        }

        if (m_CurrentTip.Time < MIN_DISPLAY_TIME)              // if the remaining display time for a tip is less than minimum allowed time then just remove the tip
        {
            m_StackingTips.Remove(m_CurrentTip);
            return;
        }

        if (m_ClosingWindow == null)                                    // is there not a message currently displayed
        {
            m_ClosingWindow = StartCoroutine(PerformTip());
        }
    }

    public IEnumerator PerformTip()
    {
        m_Title.text = m_CurrentTip.Title;                  // sets the text title as the title of the current tip
        m_Info.text = m_CurrentTip.Info;                    // sets the text info as the info of the current tip
        m_Anim.SetBool("Show", true);                       // plays the animation of the window going out to view

        var selectedTip = m_StackingTips[0];                // select the last tip added to the stack to process it

        while (selectedTip.Time > 0f)
        {
            selectedTip.Time -= Time.deltaTime;             // time reduction
            yield return null;
        }

        m_StackingTips.Remove(selectedTip);                 // remove the tip from the stack if it's done displaying

        Invoke(nameof(CheckStackingTips), m_TimeBetweenStackingTips);                // if any other tips are left
        CloseWindow();                                      // closes the window
    }

    public void CloseWindow(bool _removeTip = false)        // the parameter is to determine whether closing the window will remove that tip from the stack, this is true only if ENTER is pressed
    {
        if(!TipsEnabled)
        {
            return;
        }

        if (_removeTip)
        {
            m_StackingTips.Remove(m_CurrentTip);                 // remove the tip from the stack if it's done displaying
            Invoke(nameof(CheckStackingTips), m_TimeBetweenStackingTips);                // if any other tips are left
        }

        m_Anim.SetBool("Show", false);

        if (m_ClosingWindow != null)
        {
            StopCoroutine(m_ClosingWindow);
            m_ClosingWindow = null;
        }
    }

    public void StopTipsDelayedShow()
    {
        CancelInvoke();
    }
    #endregion
}