using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TankLike.Sound
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioDatabase _database;
        [SerializeField] private AudioSource _oneShotSource;
        [SerializeField] private bool _hasAudio = true;
        [SerializeField] private AudioPool _pooler;
        [SerializeField] private AudioMixer _mainAudioMixer;
        [SerializeField] private AudioSource _bgMusicSource;

        private const string SFX_VOLUME = "SFXVolume";
        private const string BG_MUSIC_VOLUME = "BGMusicVolume";
        private const float FADE_OUT_DURATION = 1f;
        private const float FADE_IN_DURATION = 1f;

        public void SetUp()
        {
            _pooler.SetUpPools();
        }

        public void Play(Audio audioFile)
        {
            if (!_hasAudio) return;

            Audio audio = _database.Audios[audioFile.AudioName];

            if (!audio.OneShot)
            {
                PlayAudio(audio);
            }
            else
            {
                PlayOneShotAudio(audio);
            }
        }

        private void PlayAudio(Audio audio)
        {
            AudioSource source = _pooler.GetAvailableSource();
            source.clip = audio.Clip;
            source.volume = audio.VolumeMultiplier;
            source.pitch = audio.Pitch;
            source.Play();
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
