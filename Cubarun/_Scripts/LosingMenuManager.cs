using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosingMenuManager : MonoBehaviour
{
    public static LosingMenuManager Instance;

    [Header("Losing Window")]
    [SerializeField] private Color losingFadeColor;
    [SerializeField] private float losingFadeBlendSpeed = 0.5f;
    [SerializeField] private float losingFadeDuration = 1f;
    [SerializeField] private GameObject losingMenuWindow;

    private void Awake()
    {
        Instance = this;
    }
    // here we lost for sure
    public void ShowLosingWindow(GameObject revivingWind)
    {
        PlayerStats.Instance.SetEndMenuStats();
        StartCoroutine(showLosingWindow(revivingWind));
    }

    private IEnumerator showLosingWindow(GameObject revivingWind)
    {
        FadingScreen.Instance.Fade(losingFadeDuration, losingFadeBlendSpeed, losingFadeBlendSpeed, losingFadeColor, false);
        yield return new WaitForSeconds(losingFadeBlendSpeed);
        revivingWind.FadeObject(losingFadeBlendSpeed, false);
        losingMenuWindow.SetActiveWithFade(1f, true);
    }

    #region Buttons Functions
    public void RestartButton()
    {
        FadingScreen.Instance.Release(Color.white);
        losingMenuWindow.SetActiveWithFade(0.2f, false);
        Invoke(nameof(loadPlayScene), 1f);
    }

    private void loadPlayScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitButton()
    {
        FadingScreen.Instance.Release(Color.white);
        losingMenuWindow.SetActiveWithFade(0.2f, false);
        Invoke(nameof(loadMainMenuScene), 1f);
    }

    private void loadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
