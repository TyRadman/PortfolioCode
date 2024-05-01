using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipsManager : MonoBehaviour
{
    public static UITipsManager Instance;
    [SerializeField] private Text m_TipTitle;
    [SerializeField] private Text m_PageTitle;
    [SerializeField] private Text m_TipInfo;
    [SerializeField] private Text m_PageNumber;
    [SerializeField] private Image m_TipImage;
    private int m_CurrentIndex = 0;
    private const string TIPS = "Tips";
    private const string INVENTORY = "Inventory";
    [Header("Windows references")]
    [SerializeField] private float m_WindowsTransitionSpeed = 1f;
    [SerializeField] private CanvasGroup m_InventoryWindow;
    [SerializeField] private CanvasGroup m_TipsNoteBookWindow;
    private bool m_CanSwitch = true;

    private void Awake()
    {
        Instance = this;
    }

    public void Enable()
    {
        m_PageTitle.text = TIPS;
        SetUpText(Tips.Instance.GetTip(m_CurrentIndex));
    }

    public void SetUpText(Tips.Tip _tip)
    {
        m_TipTitle.text = _tip.Title;
        m_TipInfo.text = _tip.Info;
        m_TipImage.sprite = _tip.Image;
        m_PageNumber.text = (m_CurrentIndex * 2 + 1).ToString("0");
    }

    public void NextPage()
    {
        if (Tips.Instance.NumberOfTips() > 0)
        {
            AudioManager.Instance.PlayAudio("Paper", null, true);
            m_CurrentIndex = (m_CurrentIndex + 1) % Tips.Instance.NumberOfTips(); ;
            SetUpText(Tips.Instance.GetTip(m_CurrentIndex));
        }
    }

    public void PreviousPage()
    {
        if (Tips.Instance.NumberOfTips() > 0)
        {
            AudioManager.Instance.PlayAudio("Paper", null, true);

            m_CurrentIndex--;

            if(m_CurrentIndex < 0)
            {
                m_CurrentIndex = Tips.Instance.NumberOfTips() - 1;
            }

            SetUpText(Tips.Instance.GetTip(m_CurrentIndex));
        }
    }

    public void SwitchWindows(bool _toInventory)
    {
        if(!m_CanSwitch || _toInventory && m_InventoryWindow.gameObject.activeSelf || !_toInventory && m_TipsNoteBookWindow.gameObject.activeSelf)
        {
            return;
        }

        m_CanSwitch = false;

        if (_toInventory)
        {
            StartCoroutine(windowsSwitchingProcess(m_TipsNoteBookWindow, m_InventoryWindow));
        }
        else
        {
            StartCoroutine(windowsSwitchingProcess(m_InventoryWindow, m_TipsNoteBookWindow));
        }
    }

    private IEnumerator windowsSwitchingProcess(CanvasGroup _winA, CanvasGroup _winB)
    {
        var time = 0f;
        _winB.gameObject.SetActive(true);

        // just for fun :), could've created two texts and made them blend
        var chunckSize = m_WindowsTransitionSpeed / (TIPS.Length + INVENTORY.Length);
        int counter = 0;
        bool remove = true;
        int index = 0;
        var selectedWord = m_PageTitle.text == TIPS ? INVENTORY : TIPS;

        while(time < m_WindowsTransitionSpeed)
        {
            time += Time.unscaledDeltaTime;
            var t = time / m_WindowsTransitionSpeed;
            _winA.alpha = Mathf.Lerp(1f, 0f, t);
            _winB.alpha = Mathf.Lerp(0f, 1f, t);

            // changing the title, again, for fun
            if (time > counter * chunckSize && index < selectedWord.Length)
            {
                counter++;

                if (remove)
                {
                    m_PageTitle.text = m_PageTitle.text.Remove(m_PageTitle.text.Length - 1, 1);

                    if(m_PageTitle.text.Length == 0)
                    {
                        remove = false;
                    }
                }
                else
                {
                    m_PageTitle.text += selectedWord[index++];
                }
            }

            yield return null;
        }

        m_CanSwitch = true;
        _winA.gameObject.SetActive(false);
    }
}
