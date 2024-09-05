using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TankLike.Sound
{
    public class AudioManager : MonoBehaviour, IManager
    {
        [SerializeField] private AudioDatabase _database;
        [SerializeField] private AudioSource _oneShotSource;
        //[SerializeField] private bool _hasAudio = true;
        [SerializeField] private AudioPool _pooler;
        [SerializeField] private AudioMixer _mainAudioMixer;
        [SerializeField] private AudioSource _bgMusicSource;

        [field: SerializeField, Header("Subcomponents")] public UIAudio UIAudio { get; private set; }

        public bool IsActive { get; private set; }

        private const string SFX_VOLUME = "SFXVolume";
        private const string BG_MUSIC_VOLUME = "BGMusicVolume";
        private const float FADE_OUT_DURATION = 1f;
        private const float FADE_IN_DURATION = 1f;

        #region IManager
        public void SetUp()
        {
            _pooler.SetUpPools();
        }

        public void Dispose()
        {
            _pooler.Dispose();
        }
        #endregion


        public AudioSource Play(Audio audioFile)
        {
            if (audioFile == null)
            {
                return null;
            }

            AudioSource source = null;

            if (!audioFile.OneShot)
            {
                source = PlayAudio(audioFile);
            }
            else
            {
                PlayOneShotAudio(audioFile);
            }

            return source;
        }

        private AudioSource PlayAudio(Audio audio)
        {
            AudioSource source = _pooler.GetAvailableSource();
            source.loop = audio.Loop;
            source.clip = audio.Clip;
            source.volume = audio.VolumeMultiplier;
            source.pitch = audio.Pitch;
            source.Play();

            return source;
        }

        private void PlayOneShotAudio(Audio audio)
        {
            _oneShotSource.PlayOneShot(audio.Clip, audio.VolumeMultiplier);
        }

        public void SwitchBGMusic(Audio audio)
        {
            AudioSource source = _bgMusicSource;
            source.clip = audio.Clip;
            source.volume = audio.VolumeMultiplier;
            source.pitch = audio.Pitch;
            source.Play();
        }

        public void FadeOutBGMusic()
        {
            StartCoroutine(StartFade(_mainAudioMixer, BG_MUSIC_VOLUME, FADE_OUT_DURATION, 0f));
        }

        public void FadeInBGMusic()
        {
            StartCoroutine(StartFade(_mainAudioMixer, BG_MUSIC_VOLUME, FADE_IN_DURATION, 1f));
        }

        public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
        {
            float currentTime = 0;
            float currentVol;

            audioMixer.GetFloat(exposedParam, out currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
                yield return null;
            }

            yield break;
        }
    }
}
