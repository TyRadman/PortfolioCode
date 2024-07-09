using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : Singleton<DebugManager>
{
    [SerializeField] private TextMeshProUGUI Text;

    public static void Print(string message)
    {
        Instance.Text.text = $"{message} at {Time.time}\n";
    }
}
