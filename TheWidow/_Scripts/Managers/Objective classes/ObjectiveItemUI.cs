using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveItemUI : MonoBehaviour
{
    [SerializeField] private Text m_DescriptionText;
    [SerializeField] private Text m_NumbersText;
    [SerializeField] private Image m_Line;
    [HideInInspector] public ItemName ObjectiveTag;
    [HideInInspector] public int ID;
    private int m_MaxNumber = 0;
    private int m_CurrentNumber = 0;

    public void SetObjectiveData(ObjectiveEntity _data)
    {
        m_DescriptionText.text = _data.Description;

        ObjectiveTag = _data.ObjectiveTag;
        ID = _data.ID;
        m_MaxNumber = _data.MaxItemNumber;

        if(!m_Line.enabled)
        {
            m_Line.enabled = true;
        }

        // if the max number of items to be collected is one, then there's no need to display the counter
        if(m_MaxNumber < 2)
        {
            m_NumbersText.text = string.Empty;
            return;
        }

        m_CurrentNumber = _data.NumberCollected;
        m_NumbersText.text = $"{m_CurrentNumber} / {m_MaxNumber}";
    }

    public void ItemCollected()
    {
        m_CurrentNumber++;
        m_NumbersText.text = $"{m_CurrentNumber} / {m_MaxNumber}";
    }

    public void TurnOffLine()
    {
        m_Line.enabled = false;
    }

}
