using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.HUD
{
    public class HUDSkillIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private TextMeshProUGUI _inputTypeText;
        [SerializeField] private Image _fillingImage;
        [Header("Animation")]
        [SerializeField] private Animation _activeKeyAnimation;
        [SerializeField] private AnimationClip _activeClip;
        [SerializeField] private AnimationClip _inactiveClip;

        public void SetIconSprite(Sprite icon)
        {
            _iconImage.sprite = icon;
            _fillingImage.sprite = icon;
        }

        public void SetFillAmount(float amount)
        {
            _fillingImage.fillAmount = amount;
        }

        public void SetKey(string key)
        {
            _keyText.text = key;
        }

        public void PlayAnimation(bool play)
        {
            if (play && _activeKeyAnimation.isPlaying) return;

            //print($"{play} at {Time.time}");
            if (play) _activeKeyAnimation.clip = _activeClip;
            else _activeKeyAnimation.clip = _inactiveClip;

            _activeKeyAnimation.Play();
        }
    }
}
