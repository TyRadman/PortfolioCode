using System.Collections;
using UnityEngine;

public class FlickeryLight : MonoBehaviour
{
    private Light m_Light;
    [SerializeField] private float m_LowFrequency = 0.1f;
    [SerializeField] private float m_HighFrequency = 1f;

    private void Awake()
    {
        m_Light = GetComponent<Light>();
        StartCoroutine(lowFrequency());
        StartCoroutine(highFrequency());
    }

    IEnumerator lowFrequency()
    {
        var time = 0f;
        var selectedFrequency = Random.Range(0, m_LowFrequency);

        while (time < selectedFrequency)
        {
            time += Time.deltaTime;
            yield return null;
        }
        m_Light.enabled = !m_Light.enabled;
        StartCoroutine(lowFrequency());
    }

    IEnumerator highFrequency()
    {
        var time = 0f;
        var selectedFrequency = Random.Range(0, m_HighFrequency);

        while (time < selectedFrequency)
        {
            time += Time.deltaTime;
            yield return null;
        }
        m_Light.enabled = !m_Light.enabled;
        StartCoroutine(highFrequency());
    }
}
