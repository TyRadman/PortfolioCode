using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    [SerializeField] private audioManager.AudioNames audioClipName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Effect();
        }
    }

    protected virtual void Effect()
    {
        audioManager.instance.PlayClip(audioClipName);
    }
}
