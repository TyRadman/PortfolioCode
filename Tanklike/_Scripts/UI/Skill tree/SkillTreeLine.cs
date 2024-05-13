using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.SkillTree
{
    public class SkillTreeLine : MonoBehaviour
    {
        [SerializeField] private Image _lineImage;
        public RectTransform LineTransfrom;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _unlockedClip;
        [SerializeField] private AnimationClip _lockedClip;

        public void PlayLockedAnimation()
        {
            PlayAnimation(_lockedClip);
        }

        public void PlayUnlockedAnimation()
        {
            PlayAnimation(_unlockedClip);
        }

        private void PlayAnimation(AnimationClip clip)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            _animation.clip = clip;
            _animation.Play();
        }
    }
}
