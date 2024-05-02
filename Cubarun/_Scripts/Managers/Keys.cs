using System.Collections;
using UnityEngine;
/// <summary>
/// All shared information can be found here
/// </summary>
public class Keys : MonoBehaviour
{
    public static Keys instance;
    public static string SelectedCube = "theSelectedCube";
    public static string TotalCoins = "totalCoins";
    public static string HighScore = "highScore";
    public static string GameReset = "modified";

    public Color GettingHitScreenColor;
    [SerializeField] private GameObject CoinPerfab;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetCoin()
    {
        return CoinPerfab;
    }
}