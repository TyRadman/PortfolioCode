using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    public void playAudios(AudioSource source, AudioClip clip)
    {
        source.clip = clip; // setting the audio source's clip
        source.Play(); // playing the clip of the audio source
    }
}