using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private float score;
    private int coins;
    private float scoreMultiplier = 10f;
    [SerializeField] private float coinValue = 20f;
    public static PlayerStats Instance;

    [Header("HUD UI references")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinsText;
    [Header("End Menu references")]
    [SerializeField] private Text scoreText_EM;
    [SerializeField] private Text coinsText_EM;
    [SerializeField] private Text highScoreText;


    private void Awake()
    {
        Instance = this;
    }

    public void StartCountingScore()
    {
        StartCoroutine(addScore());
    }

    public void AddCoin()
    {
        coins++;
        coinsText.text = coins.ToString();
        score += coinValue;
    }

    IEnumerator addScore()
    {
        while (!GamePlayManager.Instance.GameOver)
        {
            score += Time.deltaTime * scoreMultiplier;
            scoreText.text = score.ToString("0");

            yield return null;
        }
    }

    public void UpdateMultiplier(float newMultiplier)
    {
        scoreMultiplier *= newMultiplier;
    }

    public void SetEndMenuStats()
    {
        scoreText_EM.text = score.ToString("0");
        coinsText_EM.text = coins.ToString();

        if(score > PlayerPrefs.GetFloat(Keys.HighScore))
        {
            PlayerPrefs.SetFloat(Keys.HighScore, score);
        }

        highScoreText.text = PlayerPrefs.GetFloat(Keys.HighScore).ToString("0");
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt(Keys.TotalCoins, PlayerPrefs.GetInt(Keys.TotalCoins) + coins);
    }
}
