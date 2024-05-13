using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Sound
{
    public class AudioSourcePoolable : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioSource _source;
        private Action<IPoolable> OnReleaseToPool;

        private void ResetAudioSource()
        {
            _source.clip = null;
            _source.volume = 1f;
            _source.pitch = 1f;
        }

        public bool IsAvailable()
        {
            return !_source.isPlaying;
        }

        public AudioSource GetSource()
        {
            return _source;
        }

        #region Pool
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Init(Action<IPoolable> onRelease)
        {
            OnReleaseToPool += onRelease;
        }

        public void OnRelease()
        {
            ResetAudioSource();
        }

        public void OnRequest()
        {
            throw new NotImplementedException();
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }
        #endregion
    }
}
