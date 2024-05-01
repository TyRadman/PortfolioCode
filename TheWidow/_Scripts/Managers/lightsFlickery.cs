using System.Collections;
using UnityEngine;

// a function that makes lights flicker when required
public class lightsFlickery : MonoBehaviour
{
    [SerializeField] private Light[] Lights;
    private const float MIN_S = 0.0001f, MAX_S = 0.1f, MIN_B = 1f, MAX_B = 7f;

    public void StartFlickering() => System.Array.ForEach(Lights, l => StartCoroutine(bigFlicker(l)));

    public void StopFlickering()
    {
        StopAllCoroutines();
        System.Array.ForEach(Lights, l => l.enabled = true);
    }

    IEnumerator bigFlicker(Light _light)
    {
        Coroutine smallCO;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MIN_B, MAX_B));
            smallCO = StartCoroutine(smallFlicker(_light));
            yield return new WaitForSeconds(Random.Range(MIN_B, MAX_B));
            StopCoroutine(smallCO);
        }
    }

    IEnumerator smallFlicker(Light _light)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MIN_S, MAX_S));
            _light.enabled = !_light.enabled;
        }
    }
}