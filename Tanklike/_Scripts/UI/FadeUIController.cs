using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class FadeUIController : MonoBehaviour
    {
        [SerializeField] private Image _fadeOutImage;
        [field: SerializeField, Range(0.1f, 5f)] public float FadeOutDuration { get; private set; } = 1f;
        [field: SerializeField, Range(0.1f, 5f)] public float FadeInDuration { get; private set; } = 1f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Color _fadeOutStartColor;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _fadeInAnimationClip;
        [SerializeField] private AnimationClip _fadeOutAnimationClip;

        private void Start()
        {
            _fadeOutImage.color = _fadeOutStartColor;
            _canvasGroup.alpha = 0f;
        }

        [ContextMenu("FadeOut")]
        public void StartFadeOut()
        {
            PlayAnimation(_fadeOutAnimationClip, FadeOutDuration);
        }

        [ContextMenu("FadeIn")]
        public void StartFadeIn()
        {
            PlayAnimation(_fadeInAnimationClip, FadeInDuration);
        }

        private void PlayAnimation(AnimationClip clip, float duration = 1f)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            _animation.clip = clip;
            _animation[clip.name].speed = 1 / duration;
            _animation.Play();
        }
    }
}
