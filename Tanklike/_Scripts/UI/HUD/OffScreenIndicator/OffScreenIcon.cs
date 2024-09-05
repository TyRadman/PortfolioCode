using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    /// <summary>
    /// The icon of the screen indicator. It points towards the player when they're off-screen. It's mainly responsible for animating the icon as well as setting its color.
    /// </summary>
    public class OffScreenIcon : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;
        [SerializeField] private Image _indicatorImage;

        public void ShowIcon()
        {
            PlayAnimation(_showClip);
        }

        public void HideIcon(float speed = 1f)
        {
            PlayAnimation(_hideClip);
        }

        private void PlayAnimation(AnimationClip clip, float speed = 1f)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            if(speed != 1f)
            {
                _animation[clip.name].speed = speed;
            }

            _animation.clip = clip;
            _animation.Play();
        }

        public void SetColor(Color color)
        {
            _indicatorImage.color = color;
        }
    }
}
