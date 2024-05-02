using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogChanger : MonoBehaviour
{
    public Color[] colorsToSwitch;
    [HideInInspector] public bool enable;
    [HideInInspector] public Color originalColor;
    [SerializeField] private float switchingTime = 5f;
    [SerializeField] private float sameColorTime = 10f;
    private int colorIndex = 0;

    private void Awake()
    {
        if (enable)
        {
            StartCoroutine(colorsSwitching());
        }
    }

    IEnumerator colorsSwitching()
    {
        while (true)
        {
            yield return new WaitForSeconds(HandBook.Threshold(sameColorTime));

            StartCoroutine(switchColor());
        }
    }

    IEnumerator switchColor()
    {
        colorIndex++;

        if (colorIndex > colorsToSwitch.Length - 1)
        {
            colorIndex = 0;
        }

        float time = 0f;
        float totalTime = HandBook.Threshold(switchingTime);
        Color color = Camera.main.backgroundColor;
        Color selectedColor = colorsToSwitch[colorIndex];

        while(time < totalTime)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(color, selectedColor, time / totalTime);

            RenderSettings.fogColor = newColor;
            Camera.main.backgroundColor = newColor;

            yield return null;
        }
    }

    public Color GetOriginalColor()
    {
        return originalColor;
    }
}
