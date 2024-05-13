using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    public class EffectsUIController : MonoBehaviour
    {
        [field: SerializeField] public FadeUIController FadeUIController { get; private set; }
        [SerializeField] private ParticleSystem  _speedLinesParticles;

        public void PlaySpeedLinesEffect()
        {
            _speedLinesParticles.Play();
        }

        public void StopSpeedLinesEffect()
        {
            _speedLinesParticles.Stop();
        }
    }
}
