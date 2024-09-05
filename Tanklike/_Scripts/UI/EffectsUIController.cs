using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    public class EffectsUIController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem  _speedLinesParticles;

        [Header("Level Name")]

        [SerializeField] private GameObject _levelNameParent;
        [SerializeField] private float _levelNameDisplayduration = 1f;
        [SerializeField] private Animation _levelNameAnimation;
        [SerializeField] private AnimationClip _showAnimationClip;
        [SerializeField] private AnimationClip _hideAnimationClip;
        
        public void SetUp()
        {
            _levelNameParent.SetActive(false);
        }

        public void PlaySpeedLinesEffect()
        {
            _speedLinesParticles.Play();
        }

        public void StopSpeedLinesEffect()
        {
            _speedLinesParticles.Stop();
        }

        public void ShowLevelName()
        {
            StartCoroutine(ShowLevelNameProcess());
        }

        private IEnumerator ShowLevelNameProcess()
        {
            _levelNameParent.SetActive(true);

            _levelNameAnimation.clip = _showAnimationClip;
            _levelNameAnimation.Play();

            yield return new WaitForSeconds(_levelNameDisplayduration);

            _levelNameAnimation.clip = _hideAnimationClip;
            _levelNameAnimation.Play();

            yield return new WaitForSeconds(1f);

            _levelNameParent.SetActive(false);
        }
    }
}
