using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class audioManager : MonoBehaviour
{
    public enum AudioNames
    {
        Coin, Death, Button
    }
    [System.Serializable]
    public class AudioFile
    {
        public AudioNames Name;
        public AudioClip Clip;
    }
    [SerializeField] private List<AudioFile> Audios = new List<AudioFile>();

    public static audioManager instance;
    public AudioClip[] musicClips;
    public AudioClip[] effectsClips;
    private AudioSource musicPlayer;
    private AudioSource source;
    private static int theScene;


    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            // singleton implementation
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
            }

            theScene = SceneManager.GetActiveScene().buildIndex;

            DontDestroyOnLoad(gameObject);


            musicPlayer = GetComponent<AudioSource>();

            source = GameObject.Find("sound effects").GetComponent<AudioSource>();

            musicPlayer.clip = musicClips[0];

            musicPlayer.Play();
        }
    }

    public void PlayClip(AudioNames clipName)
    {
        source.PlayOneShot(Audios.Find(a => a.Name == clipName).Clip);
    }

    public void playAudio(AudioClip clip)
    {
        //effectsPlayer.PlayOneShot(clip);
    }

}
