using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLight : MonoBehaviour
{
    [SerializeField] private Material GroundMaterial;
    [SerializeField] private Color[] emissionColors;
    [SerializeField] private float maxEmission = 0.8f;
    [SerializeField] private float minEmission = 0.2f;
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private float colorTransitionDuration = 2f;
    private float currentIntesity;
    private Color currentColor;
    
    private void Start()
    {
        GroundMaterial.EnableKeyword("_EMISSION");

        currentColor = emissionColors[Random.Range(0, emissionColors.Length)];

        StartCoroutine(raiseIntensity());
    }

    IEnumerator raiseIntensity()
    {
        currentIntesity = minEmission;
        GroundMaterial.SetColor("_EmissionColor", currentColor * currentIntesity);
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            currentIntesity = Mathf.Lerp(minEmission, maxEmission, time / transitionDuration);
            GroundMaterial.SetColor("_EmissionColor", currentColor * currentIntesity);
            
            yield return null;
        }

        GroundMaterial.SetColor("_EmissionColor", currentColor * maxEmission);

        StartCoroutine(lowerIntensity());
    }

    IEnumerator lowerIntensity()
    {
        currentIntesity = maxEmission;
        GroundMaterial.SetColor("_EmissionColor", currentColor * currentIntesity);
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            currentIntesity = Mathf.Lerp(maxEmission, minEmission, time / transitionDuration);
            GroundMaterial.SetColor("_EmissionColor", currentColor * currentIntesity);

            yield return null;
        }

        GroundMaterial.SetColor("_EmissionColor", currentColor * minEmission);

        StartCoroutine(switchColor());
    }

    IEnumerator switchColor()
    {
        var oldColor = currentColor;
        currentColor = emissionColors[Random.Range(0, emissionColors.Length)];
        Color newColor = GroundMaterial.GetColor("_EmissionColor");
        var timer = 0f;

        while(timer < colorTransitionDuration)
        {
            timer += Time.deltaTime;
            newColor = Color.Lerp(oldColor, currentColor, timer);
            GroundMaterial.SetColor("_EmissionColor", newColor * currentIntesity);

            yield return null;
        }

        StartCoroutine(raiseIntensity());
    }
}
