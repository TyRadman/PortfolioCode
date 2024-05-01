using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    static public AudioManager Instance;
    public List<AudioBaseClass> Audios = new List<AudioBaseClass>();
    public AudioBaseClass AudiosPlaying;
    public AudioMixer MainAudioMixer;

    private string m_LastPlayedEnemyAudio;
    private string m_StartingThemeName = "StartingTheme";
    private string m_BackGroundThemeName = "bgMusic";
    private string m_ChasingMusicName = "ChasingMusic";

    #region Coroutines
    public Coroutine VolumeOnChange;
    public Coroutine VolumeOnChangeChase;
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (AudioBaseClass _audio in Audios)// passes the values from the audio base class to the audiosource attached to each one of its elements
        {
            if (_audio.Source == null)// if the object doesn't have an already set audio source then we add one
            {
                _audio.Source = _audio.Object.AddComponent<AudioSource>();
            }

            if (!_audio.Loop)// if the audio source has only one clip then it is set to it
            {
                _audio.Source.clip = _audio.Clip[0];
            }
        }
    }

    void Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                {
                    PlayAudio(m_StartingThemeName, null, false);
                    break;
                }
            case 1:
                {
                    PlayAudio(m_BackGroundThemeName, null, false);   // the back ground music is played the moment this scene starts
                    break;
                }
        }
    }


    public void PlayEnemyAudio(string _name, bool _shot)
    {
        AudioBaseClass audioSelected = Audios.Find(audio => audio.Name == _name);  // finds the audio by the name
        int randomIndex = Random.Range(0, audioSelected.Clip.Count);    // selects a random index 
        audioSelected.Source.volume = audioSelected.Volume;             // sets the volume of the audiosource according to the audioBaseClass's volume

        if (_shot)  // if it is just one shot, then we play it as one shot and return
        {
            audioSelected.Source.PlayOneShot(audioSelected.Clip[randomIndex]);
            m_LastPlayedEnemyAudio = _name;                           
            return;
        }

        if (m_LastPlayedEnemyAudio == _name)    // if the selected track is the same as the one played before
        {
            if (!audioSelected.Source.isPlaying)        // if there is no audio playing then we set the new clip and play it
            {
                audioSelected.Source.clip = audioSelected.Clip[randomIndex];
                audioSelected.Source.Play();
            }
        }
        else                                            // if it is a different track then we stop the previous one, set the new one and play it
        {
            audioSelected.Source.Stop();
            audioSelected.Source.clip = audioSelected.Clip[randomIndex];
            audioSelected.Source.Play();
        }

        m_LastPlayedEnemyAudio = _name;                                 
    }

    /// <summary>
    /// Plays an audio clip after a specified time
    /// </summary>
    /// <param name="_name">Name of the audio to play.</param>
    /// <param name="_source">What audio source will play the audio. If set to null then the audio source assigned to the audio base class will be used instead.</param>
    /// <param name="_oneShot">Is it oneshot?</param>
    /// <param name="_delay">The delay after which the audio is played.</param>
    /// <param name="_desiredVolume">A volume for the audio. It's 1f by default.</param>
    public void PlayAudioWithDelay(string _name, AudioSource _source, bool _oneShot, float _delay,float _desiredVolume = 1f)
    {
        StartCoroutine(playDelayed(_name, _source, _oneShot, _delay, _desiredVolume));
    }

    private IEnumerator playDelayed(string _name, AudioSource _source, bool _oneShot, float _delay, float _desiredVolume = 1f)
    {
        yield return new WaitForSeconds(_delay);
        PlayAudio(_name, _source, _oneShot, _desiredVolume);
    }

    /// <summary>
    /// Plays an audio track that is chosen by the name with specified parameters
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_source"></param>
    /// <param name="_oneShot"></param>
    /// <param name="_volume"></param>
    public void PlayAudio(string _name, AudioSource _source, bool _oneShot, float _volume = 1f)
    {
        AudioBaseClass audioPlayed = Audios.Find(audio => audio.Name == _name);// gets the requested elemnt of the array according to the condition fed

        if (audioPlayed == null)                      // if the element does not exist then it's ignored with a log message. Meaning we need to add it in the inspector
        {
            Debug.LogWarning("track doesn't exist");
            return;
        }

        if (_source != null && _name.Contains("Door"))// in case we want to customize the source. The door condition is temporary. It's simply used to make sure every door's audio is played by its own audiosource
        {
            audioPlayed.Source = _source;
        }

        audioPlayed.Source.volume = audioPlayed.Volume;// setting the volume

        if (audioPlayed.Loop) // if the track is a looping track then 
        {
            audioPlayed.Source.clip = audioPlayed.Clip[Random.Range(0, audioPlayed.Clip.Count)];// pick a random clip from the clips that this track has
            audioPlayed.Source.Play();// play the audio
            StartCoroutine(playForWhile(audioPlayed.Source.clip.length, _name));                // this function is recursive as it calls PlayAudio again and again
            AudiosPlaying = audioPlayed;                                                        // sets this track as the currently playing one so that we can detect it and stop it when desired
            return;
        }

        if (_volume != 1f)                                                    // if we want to change the volume only for this particular audio
        {
            audioPlayed.Source.volume = _volume;
        }

        var selectedClip = audioPlayed.Clip[Random.Range(0, audioPlayed.Clip.Count)];// if it doesn't loop then the audio source plays the only clip it has

        if (_oneShot)
        {
            audioPlayed.Source.PlayOneShot(selectedClip, _volume);
        }
        else
        {
            audioPlayed.Source.clip = selectedClip;
            audioPlayed.Source.Play();
        }

        audioPlayed.Source.volume = audioPlayed.Volume;                              // returning the old volume
    }

    IEnumerator playForWhile(float duration, string name)
    {
        yield return new WaitForSeconds(duration);      // waits until the audio is done playing 
        PlayAudio(name, null, false);                   // calls the playAudio function again
    }

    public void PlayChasingMusic()
    {
        AudiosPlaying.Source.volume = 0f;       // shuts all other music playing and plays the chasing music
        PlayAudio(m_ChasingMusicName, null, false);
    }

    public void PlayBGMusicAfterVollion()       // resumes normal music
    {
        AudiosPlaying.Source.volume = 1f;
        Audios.Find(a => a.Name == (m_ChasingMusicName)).Source.Stop();
    }

    // loads the values that were originally saved in the settings
    public void StartUpValues()
    {
        MainAudioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat(Keys.Settings_Music));  // loads the music volume
        MainAudioMixer.SetFloat("sfxVolume", PlayerPrefs.GetFloat(Keys.Settings_SFX));      // loads the sfx volume
    }
}