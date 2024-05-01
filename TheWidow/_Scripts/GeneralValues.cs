using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralValues : MonoBehaviour
{
    public static float MIN_BATTERY_VALUE;
    public static float MAX_BATTERY_VALUE;
    [Range(0f, 1f)][SerializeField] private float m_MinBatteryValue;
    [Range(0f, 1f)][SerializeField] private float m_MaxBatteryValue;

    private void Awake()
    {
        MIN_BATTERY_VALUE = m_MinBatteryValue;
        MAX_BATTERY_VALUE = m_MaxBatteryValue;
    }

    public static float BatteryValue()
    {
        return Random.Range(MIN_BATTERY_VALUE, MAX_BATTERY_VALUE);
    }
}

