using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// a class to make screen fades easier
public class FadingScreen : MonoBehaviour
{
    public static FadingScreen Instance;        
    [SerializeField] private Image panel;
    private Color startColor;
    private Color endColor;
    private float fadingOutSpeed;
    private const float DEFAULT_FADE_TIME = 2f;
    private const float FREQUENCY_MIN = 0.02f;
    private const float FREQUENCY_MAX = 0.1f;
    private Coroutine m_FlickeryScreen_CO;

    private void Awake()
    {
        Instance = this;
    }

    #region Blank
    /// <summary>
    /// Covers the screen with a black panel
    /// </summary>
    /// <param name="_color">The color of the panel.</param>
    public void BlankScreen(Color _color)
    {
        panel.color = _color;
    }
    #endregion

    #region Fade In overloads
    /// <summary>
    /// A blank screen with a specified color will fade in slowly.
    /// </summary>
    /// <param name="_color">The color of the fading screen.</param>
    public void FadeIn(Color _color)
    {
        startColor = new Color(_color.r, _color.g, _color.b, 0f);
        endColor = _color;

        StartCoroutine(fading(startColor, endColor, DEFAULT_FADE_TIME));
    }
    /// <summary>
    /// A blank screen with a specified color will fade in slowly in a specified time.
    /// </summary>
    /// <param name="_color">The color of the fading screen.</param>
    /// <param name="_duration">The time taken for the screen to fade in and cover what's behind it.</param>
    public void FadeIn(Color _color, float _duration)
    {
        startColor = new Color(_color.r, _color.g, _color.b, 0f);
        endColor = _color;

        StartCoroutine(fading(startColor, endColor, _duration));
    }
    #endregion

    #region Fade Out overloads
    /// <summary>
    /// The screen will instantly be covered and then slowly fade out.
    /// </summary>
    /// <param name="_color">The color of the fading screen.</param>
    public void FadeOut(Color _color)
    {
        endColor = new Color(_color.r, _color.g, _color.b, 0f);
        startColor = _color;
        panel.color = startColor;

        StartCoroutine(fading(startColor, endColor, DEFAULT_FADE_TIME));
    }
    /// <summary>
    /// Fades the screen out with a specified duration.
    /// </summary>
    /// <param name="_color">The color of the fading screen.</param>
    /// <param name="_duration">The time taken for the screen to fade out and vanish.</param>
    public void FadeOut(Color _color, float _duration)
    {
        endColor = new Color(_color.r, _color.g, _color.b, 0f);
        startColor = _color;
        panel.color = startColor;

        StartCoroutine(fading(startColor, endColor, _duration));
    }
    #endregion

    #region Fade in and out
    /// <summary>
    /// Makes the screen fade in and out with specified parameters
    /// </summary>
    /// <param name="duration">The duration of the fully faded screen</param>
    /// <param name="fadingInSpeed">The duration of the fade in</param>
    /// <param name="fadeOutSpeed">The duration of the fade out</param>
    /// <param name="color">The color of the screen during the process</param>
    /// <param name="fadeOut">Determines whether there's a fade out</param>
    public void Fade(float duration, float fadingInSpeed, float fadeOutSpeed, Color color)
    {
        startColor = new Color(color.r, color.g, color.b, 0f);
        endColor = color;
        fadingOutSpeed = fadeOutSpeed;
        StartCoroutine(fadingProcess(duration, fadingInSpeed, fadeOutSpeed));
    }
    #endregion

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
        yield return new WaitForSeconds(duration);
        StartCoroutine(fading(endColor, startColor, outSpeed));
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

    #region Flickery screen
    /// <summary>
    /// Makes the screen flicker with a given color for a given amount of time
    /// </summary>
    /// <param name="_color">The color of the screen when it flickers</param>
    /// <param name="_duration">The duration of the flicker</param>
    public void FlickeryScreen(Color _color, float _duration, float _maxAlpha = 1f)
    {
        m_FlickeryScreen_CO = StartCoroutine(performFlicker(_color, _duration, _maxAlpha));
    }

    public void StopFlickeryScreen()
    {
        if (m_FlickeryScreen_CO != null)
        {
            StopCoroutine(m_FlickeryScreen_CO);
            m_FlickeryScreen_CO = null;
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0f);
        }
    }

    IEnumerator performFlicker(Color _color, float _duration, float _maxAlpha = 1f, float _minFreq = FREQUENCY_MIN, float _maxFreq = FREQUENCY_MAX)
    {
        var time = 0f;
        var timer = 0f;
        var switchTime = Random.Range(_minFreq, _maxFreq);
        var noAlphaColor = new Color(_color.r, _color.g, _color.b, 0f);

        while (time < _duration)
        {
            time += Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= switchTime)
            {
                timer = 0f;
                switchTime = Random.Range(_minFreq, _maxFreq);
                panel.color = panel.color.a == 0f ? _color : noAlphaColor;
            }

            yield return null;
        }

        panel.color = noAlphaColor;
    }
    #endregion
}
