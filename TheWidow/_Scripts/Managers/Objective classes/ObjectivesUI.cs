using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform m_SidePanel;
    [SerializeField] private Animation m_Anim;
    [SerializeField] private AnimationClip m_FadeInClip;
    [SerializeField] private AnimationClip m_FadeOutClip;
    [SerializeField] private GameObject m_MainObjectivesTitle;
    [SerializeField] private GameObject m_SideObjectivesTitle;
    [SerializeField] private List<ObjectiveItemUI> m_MainObjectivesElements = new List<ObjectiveItemUI>();
    [SerializeField] private List<ObjectiveItemUI> m_SideObjectivesElements = new List<ObjectiveItemUI>();
    [Header("Values")]
    [SerializeField] private KeyTypes m_KeyCode;
    [SerializeField] private float m_SlidingSpeed = 2f;
    [SerializeField] private float m_HorizontalPosition;
    [SerializeField] private float m_TimeToDisplayText = 0.1f;
    [HideInInspector] public bool IsOpened = false;
    private bool m_CanChangePanelState = true;
    private Vector2 m_PreviousHorizontalPosition;
    private Vector2 m_NewHorizontalPosition;


    private void Start()
    {
        // set functions that get triggered when the specified key is pressed. The panel is opened when the key is down and closed when it's up
        //InputManager.Instance.SetUpButton(m_KeyCode, _down: OpenPanel, _up: ClosePanel);
        InputManager.Instance.SetUpButton(m_KeyCode, _down: TogglePanel);
        // caches the initial position of the objectives panel
        m_PreviousHorizontalPosition = m_SidePanel.anchoredPosition;
        // creates a vector 3 that represents the new position of the panel when toggled
        m_NewHorizontalPosition = new Vector2(m_HorizontalPosition, m_SidePanel.anchoredPosition.y);
        // we fade out the text at start so that it fades in when it's opened
        playAnimation(m_FadeOutClip);
    }

    #region Display Objectives
    public void UpdateElements(List<ObjectiveEntity> _Objectives)
    {
        printObjectives(m_MainObjectivesElements, m_MainObjectivesTitle, _Objectives.FindAll(o => o.ObjectiveType == ObjectiveTypes.Main));
        printObjectives(m_SideObjectivesElements, m_SideObjectivesTitle, _Objectives.FindAll(o => o.ObjectiveType == ObjectiveTypes.Side));
    }

    private void printObjectives(List<ObjectiveItemUI> _texts, GameObject _title, List<ObjectiveEntity> _objs)
    {
        // turn everything off if there are no objectives
        if (_objs.Count == 0)
        {
            _title.SetActive(false);
            _texts.FindAll(t => t.gameObject.activeSelf).ForEach(t => t.gameObject.SetActive(false));
        }
        else
        {
            if (!_title.gameObject.activeSelf)
            {
                _title.gameObject.SetActive(true);
            }

            for (int i = 0; i < _texts.Count; i++)
            {
                // if all required objectives have been displayed then turn off the other texts
                if (i > _objs.Count - 1)
                {
                    if (_texts[i].gameObject.activeSelf)
                    {
                        _texts[i].gameObject.SetActive(false);
                    }

                    return;
                }

                // if we have an objective to display and the text was off then turn it on
                if (!_texts[i].gameObject.activeSelf)
                {
                    _texts[i].gameObject.SetActive(true);
                }

                // print the objective description onto the text
                _texts[i].SetObjectiveData(_objs[i]);
            }
        }
    }

    public void RemoveObjective(int _id, ObjectiveTypes _type)
    {
        ObjectiveItemUI selectedSlot;

        // we catch the specific displayed objective
        if(_type == ObjectiveTypes.Main)
        {
            selectedSlot = m_MainObjectivesElements.Find(o => o.ID == _id);
        }
        else
        {
            selectedSlot = m_SideObjectivesElements.Find(o => o.ID == _id);
        }

        selectedSlot.gameObject.SetActive(false);
    }

    public List<ObjectiveItemUI> GetTextElementsWithTag(ItemName _tag)
    {
        var allObjs = m_SideObjectivesElements.FindAll(o => o.ObjectiveTag == _tag);
        allObjs.AddRange(m_MainObjectivesElements.FindAll(o => o.ObjectiveTag == _tag));
        return allObjs;
    }
    #endregion

    #region Opening and closing panel
    // displays the objectives panel
    public void TogglePanel()
    {
        if(!m_CanChangePanelState)
        {
            return;
        }

        m_CanChangePanelState = false;

        if (IsOpened)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        StartCoroutine(openPanel());
    }

    // hides the objectives panel
    public void ClosePanel()
    {
        StartCoroutine(closePanel());
    }

    IEnumerator openPanel()
    {
        float time = 0f;

        while(time < m_SlidingSpeed)
        {
            time += Time.deltaTime;
            // we interpolate the position of the objectives panel from its original position to its new postion
            m_SidePanel.anchoredPosition = Vector2.Lerp(m_PreviousHorizontalPosition, m_NewHorizontalPosition, time / m_SlidingSpeed);

            yield return null;
        }

        // play the fade in animation
        playAnimation(m_FadeInClip);
        // after the fade in animation is finished, we declare the objectives panel opened
        yield return new WaitForSeconds(m_FadeInClip.length);
        IsOpened = true;
        m_CanChangePanelState = true;
    }

    IEnumerator closePanel()
    {
        IsOpened = false;
        float time = 0f;
        // we play the fade out animation right before the objectives panel is closed
        playAnimation(m_FadeOutClip);

        while (time < m_SlidingSpeed)
        {
            time += Time.deltaTime;
            m_SidePanel.anchoredPosition = Vector2.Lerp(m_NewHorizontalPosition, m_PreviousHorizontalPosition, time / m_SlidingSpeed);

            yield return null;
        }

        m_CanChangePanelState = true;
    }

    private void playAnimation(AnimationClip _clip)
    {
        // stop whatever animation was playing
        m_Anim.Stop();
        // set the clip we want to play
        m_Anim.clip = _clip;
        // play the clip
        m_Anim.Play();
    }
    #endregion
}
