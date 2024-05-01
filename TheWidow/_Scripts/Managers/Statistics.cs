using UnityEngine;
using UnityEngine.UI;

// this class is still experimental and might get removed
public class Statistics : MonoBehaviour
{
    [SerializeField] private Text m_TotalTimeText, m_PhysicalRatingText, m_BravoryRatingText, Rank, m_TotalBottles;
    public static float StandingTime, WalkingTime, CrouchingTime, RunningTime, FlashLightTime, HidingTime;
    public enum PhysicalState
    {
        Athletic, Average, Worm, None
    }
    public static PhysicalState CurrentPhysicalState = PhysicalState.None;
    public enum BravoryState
    {
        Hercules, Fearless, Brave, BraveEnough, Pussy, Coward, Shame, None
    }
    public static BravoryState CurrentBravoryState = BravoryState.None;

    public static void GetData()
    {
        print($"Standing = {StandingTime:0},Walking = {WalkingTime:0}, Running = {RunningTime:0}, Crouching = {CrouchingTime:0}, Flashlight = {FlashLightTime:0}, Hiding = {HidingTime:0}");
    }

    public void AssignStatistics()
    {
        calculateStatistics();
        m_TotalTimeText.text = FindObjectOfType<PauseMenuManager>().m_Timetext.text;
        m_PhysicalRatingText.text = CurrentPhysicalState.ToString();
        m_BravoryRatingText.text = CurrentBravoryState.ToString();
        m_TotalBottles.text = $"{PlayerStats.Instance.MedicinesCollected} / {PlayerStats.Instance.MaxNumberOfMedicine}";
    }

    void calculateStatistics()
    {
        var total = (int)FindObjectOfType<PauseMenuManager>().TotalTime;
        var hiding = HidingTime / total;
        var crouch = CrouchingTime / total;
        // bravory calculations
        if (HidingTime == 0 && FlashLightTime / total < 0.1f)
            CurrentBravoryState = BravoryState.Hercules;
        else if (HidingTime == 0)
        {
            if (crouch < 0.01f)
                CurrentBravoryState = BravoryState.Fearless;
            else if (crouch < 0.1f)
                CurrentBravoryState = BravoryState.Brave;
            else
                CurrentBravoryState = BravoryState.BraveEnough;
        }
        else if (HidingTime / total < 0.01f)
        {
            if (crouch < 0.1f)
                CurrentBravoryState = BravoryState.Brave;
            else
                CurrentBravoryState = BravoryState.BraveEnough;
        }
        else if (hiding < 0.1)
        {
            if (crouch < 0.2)
                CurrentBravoryState = BravoryState.BraveEnough;
            else
                CurrentBravoryState = BravoryState.Pussy;
        }
        else if (hiding < 0.3)
            CurrentBravoryState = BravoryState.Pussy;
        else if (hiding < 0.4)
            CurrentBravoryState = BravoryState.Coward;
        else
            CurrentBravoryState = BravoryState.Shame;


    }
}