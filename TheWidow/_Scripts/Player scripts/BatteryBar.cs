using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    public static BatteryBar Instance;
    [SerializeField] private Transform m_BarsParent;
    [SerializeField] private Gradient m_BatteryGradient;
    [SerializeField] private bool m_Active = true;
    private List<Image> m_BatteryBars = new List<Image>();
    private int m_BatteryBarActiveIndex = 0;
    private float m_ChunkSize;
    [Header("Charging Variables")]
    [SerializeField] private float m_ChargingDuration = 1f;
    private bool m_IsCharging = false;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < m_BarsParent.childCount; i++)
        {
            m_BatteryBars.Add(m_BarsParent.GetChild(i).GetComponent<Image>());
        }

        m_BatteryBarActiveIndex = m_BatteryBars.Count - 1;
        m_ChunkSize = 1f / m_BatteryBars.Count;
        updateColors(1f);
    }

    /// <summary>
    /// Updates the battery bar
    /// </summary>
    /// <param name="_amout">A value from 0 to 1 representing the battery life.</param>
    public void UpdateBatteryBar(float _amout)
    {
        if (m_IsCharging)
        {
            return;
        }

        m_BatteryBarActiveIndex = (int)(_amout / m_ChunkSize);

        // if the battery is full then skip
        if (m_BatteryBarActiveIndex == m_BatteryBars.Count - 1)
        {
            return;
        }

        // disbale the pervious battery bar (going backwards)
        m_BatteryBars[m_BatteryBarActiveIndex + 1].enabled = false;
        updateColors(_amout);
    }

    private void updateColors(float _amount)
    {
        m_BatteryBars.ForEach(b => b.color = m_BatteryGradient.Evaluate(_amount));
    }

    public void AddBatteryLife(float _amout)
    {
        m_IsCharging = true;
        StartCoroutine(chargingProcess(_amout));
    }

    IEnumerator chargingProcess(float _amout)
    {
        // subtract final number of active bars from the currently active bars to get the number of iterations we need
        print(_amout);
        int expectedActiveBarsNum = (int)(_amout / m_ChunkSize);
        int currentlyActiveBarsNum = m_BatteryBars.FindAll(b => b.enabled).Count;
        int iterationNum = expectedActiveBarsNum - currentlyActiveBarsNum;
        int startingIndex = m_BatteryBarActiveIndex;
        WaitForSeconds wait = new WaitForSeconds(m_ChargingDuration / iterationNum);

        if(iterationNum + currentlyActiveBarsNum > m_BatteryBars.Count - 1)
        {
            iterationNum = m_BatteryBars.Count - 1 - currentlyActiveBarsNum;
        }

        // make sure the battery isn't charging when the inventory
        while (Inventory.Instance.IsOpened)
        {
            yield return null;
        }

        print($"Iterations number is {m_BatteryBarActiveIndex + iterationNum} starting from {m_BatteryBarActiveIndex}");
        for (int i = startingIndex; i < startingIndex + iterationNum; i++)
        {
            m_BatteryBars[i].enabled = true;
            m_BatteryBarActiveIndex = i;

            yield return wait;
        }

        m_IsCharging = false;
        updateColors(_amout);
    }
}