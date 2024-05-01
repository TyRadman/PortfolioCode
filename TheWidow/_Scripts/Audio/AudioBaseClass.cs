using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioBaseClass
{
    public string Name;
    public GameObject Object;
    public List<AudioClip> Clip;
    public AudioSource Source;
    public bool Loop;
    public float Volume = 1f;
}