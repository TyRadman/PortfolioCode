using UnityEngine;

// a class that will have small functions that are frequently used
public class SimpleFunctions : MonoBehaviour
{
    public static SimpleFunctions Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool Chance(float SuccessChance)
    {
        return Random.Range(0f, 100f) <= SuccessChance;
    }

    public static string ColorText(string _text, string _col)
    {
        return $"<color={_col}> {_text} </color>";
    }
}