using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class CrossHairVisuals : MonoBehaviour
    {
        private int _showTriggerHash = Animator.StringToHash("Show");
        private int _hideTriggerHash = Animator.StringToHash("Hide");
        private int _shootTriggerHash = Animator.StringToHash("Shoot");
        private int _rechargeHash = Animator.StringToHash("ShotRecharge");
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _outerLayer;
        [SerializeField] private SpriteRenderer _innerLayer;
        [SerializeField] private SpriteRenderer _centerLayer;

        public void SetColor(Color color)
        {
            _outerLayer.color = color;
            _innerLayer.color = color;
            _centerLayer.color = color;
        }

        public void ShowCrossHair()
        {
            TriggerAnimation(_showTriggerHash);
        }

        public void HideCrossHair()
        {
            TriggerAnimation(_hideTriggerHash);
        }

        public void PlayShootAnimation()
        {
            TriggerAnimation(_shootTriggerHash);
        }

        public void PlayOnShotReloadAnimation()
        {
            TriggerAnimation(_rechargeHash);
        }

        private void TriggerAnimation(int triggerID)
        {
            _animator.SetTrigger(triggerID);
        }
    }
}
