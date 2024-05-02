using System.Collections;
using UnityEngine;

/// <summary>
/// A class that has some useful mini methods
/// </summary>
public class HandBook : MonoBehaviour
{
    private static HandBook Instance;

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// Returns the string with a color in Rich text fields like the editor or the UI.Text
    /// </summary>
    /// <param name="text">The text to covert</param>
    /// <param name="color">The desired color</param>
    /// <returns></returns>
    public static string RichTextColor(string text, Color color)
    {
        string colorInHex = ColorUtility.ToHtmlStringRGB(color);
        return "<color=#" + colorInHex + ">" + text + "</color>";
    }

    public static int RandonSign()
    {
        return Random.Range(0f, 1f) > 0.5f ? -1 : 1;
    }

    public static float Threshold(float value)
    {
        return value + Random.Range(-value / 2, value / 2);
    }
}