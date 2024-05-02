using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectID : MonoBehaviour
{
    private ParticleSystem effect;
    public string EffectName;

    private void Start()
    {
        effect = GetComponent<ParticleSystem>();
    }

    public void SetColor(Color color)
    {
        effect.startColor = color;
    }

    public void UseEffect(Vector3 position)
    {
        transform.parent = null;
        transform.position = position;
        effect.Play();
    }

    public bool IsAvailable()
    {
        return !effect.isPlaying;
    }

    public void PlayForDuration(float duration, Transform parent)
    {
        // sets its position under the parent
        transform.parent = parent;
        transform.localPosition = Vector3.zero;

        // sets the duration
        var main = effect.main;
        main.duration = duration;
        
        effect.Play();
    }
}
