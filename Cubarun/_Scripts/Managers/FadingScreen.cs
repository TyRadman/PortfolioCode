using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingScreen : MonoBehaviour
{
    public static FadingScreen Instance;
    [SerializeField] private Image panel;
    [SerializeField] private Text text;
    private Color startColor;
    private Color endColor;
    private float fadingOutSpeed;
    private bool fadeOutEnabled = true;

    private Color textStartColor;
    private Color textEndColor;
    private float textFadingOutSpeed;
    private bool textFadeOutEnabled = true;
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Makes the screen fade in and out with specified parameters
    /// </summary>
    /// <param name="duration">The duration of the fully faded screen</param>
    /// <param name="fadingInSpeed">The duration of the fade in</param>
    /// <param name="fadeOutSpeed">The duration of the fade out</param>
    /// <param name="color">The color of the screen during the process</param>
    /// <param name="fadeOut">Determines whether there's a fade out</param>
    public void Fade(float duration, float fadingInSpeed, float fadeOutSpeed, Color color, bool fadeOut = true)
    {
        startColor = new Color(color.r, color.g, color.b, 0f);
        endColor = color;
        fadeOutEnabled = fadeOut;
        fadingOutSpeed = fadeOutSpeed;
        StartCoroutine(fadingProcess(duration, fadingInSpeed, fadeOutSpeed));
    }

    #region Release Function overloads
    /// <summary>
    /// Releases the last half fade (only fades out after an incomplete fade in)
    /// </summary>
    public void Release()
    {
        StartCoroutine(fading(endColor, startColor, fadingOutSpeed));
    }
    /// <summary>
    /// Releases the last half fade with a new color
    /// </summary>
    /// <param name="newColor">The new color of the fade out</param>
    public void Release(Color newColor)
    {
        startColor = newColor;
        StartCoroutine(fading(endColor, startColor, fadingOutSpeed));
    }
    /// <summary>
    /// Fades out without have a fade in before it
    /// </summary>
    /// <param name="newColor">The color of the screen</param>
    /// <param name="fadeOutDuration">The fade out duration</param>
    public void Release(Color newColor, float fadeOutDuration)
    {
        startColor = newColor;
        endColor = new Color(newColor.r, newColor.g, newColor.b, 0f);
        StartCoroutine(fading(startColor, endColor, fadeOutDuration));
    }
    #endregion

    #region Helping functioons
    private IEnumerator fadingProcess(float duration, float inSpeed, float outSpeed)
    {
        StartCoroutine(fading(startColor, endColor, inSpeed));

        if (fadeOutEnabled)
        {
            yield return new WaitForSeconds(duration);
            StartCoroutine(fading(endColor, startColor, outSpeed));
        }
    }

    private IEnumerator fading(Color colorOne, Color colorTwo, float blendSpeed)
    {
        float time = 0;
        while (time < blendSpeed)
        {
            time += Time.deltaTime;
            panel.color = Color.Lerp(colorOne, colorTwo, time / blendSpeed);
            yield return null;
        }
    }
    #endregion

    #region Text Fading
    public void FadeText(string message, float duration, float fadingInSpeed, float fadeOutSpeed, Color color, bool fadeOut = true)
    {
        text.text = message;
        textStartColor = new Color(color.r, color.g, color.b, 0f);
        textEndColor = color;
        textFadeOutEnabled = fadeOut;
        textFadingOutSpeed = fadeOutSpeed;
        StartCoroutine(textFadingProcess(duration, fadingInSpeed, fadeOutSpeed));
    }

    private IEnumerator textFadingProcess(float duration, float inSpeed, float outSpeed)
    {
        StartCoroutine(textFading(textStartColor, textEndColor, inSpeed));

        if (textFadeOutEnabled)
        {
            yield return new WaitForSeconds(duration);
            StartCoroutine(textFading(textEndColor, textStartColor, outSpeed));
        }
    }

    private IEnumerator textFading(Color colorOne, Color colorTwo, float blendSpeed)
    {
        float time = 0;

        while (time < blendSpeed)
        {
            time += Time.deltaTime;
            text.color = Color.Lerp(colorOne, colorTwo, time / blendSpeed);
            yield return null;
        }
    }
    #endregion
}
