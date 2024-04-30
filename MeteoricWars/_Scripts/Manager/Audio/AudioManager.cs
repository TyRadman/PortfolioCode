using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class AudioManager : Singlton<AudioManager>
    {
        //[SerializeField] private Audio m_Audios;
        [SerializeField] private AudioSource m_Source;

        public void PlayAudio(Audio _audio)
        {
            if (_audio == null)
            {
                print("No audio passed");
                return;
            }
            
            m_Source.pitch = _audio.Pitch;
            m_Source.loop = _audio.Loop;

            if (_audio.PitchMode == PitchMode.Random)
            {
                m_Source.pitch = 1 + _audio.PitchRandomizingRange.RandomValue();
            }

            m_Source.PlayOneShot(_audio.Clips[0], _audio.Volume);
        }
    }
}