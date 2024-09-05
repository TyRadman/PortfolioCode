using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class CrossHairVisuals : MonoBehaviour
    {

        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _outerLayer;
        [SerializeField] private SpriteRenderer _innerLayer;
        [SerializeField] private SpriteRenderer _centerLayer;

        [Header("Aim")]
        [SerializeField] private GameObject _innerAim;
        [SerializeField] private GameObject _outerAim;
        [SerializeField] private Animator _aimAnimator;

        private readonly int _showTriggerHash = Animator.StringToHash("Show");
        private readonly int _hideTriggerHash = Animator.StringToHash("Hide");
        private readonly int _shootTriggerHash = Animator.StringToHash("Shoot");
        private readonly int _rechargeHash = Animator.StringToHash("ShotRecharge");
        private readonly int _isAimingHash = Animator.StringToHash("IsAiming");

        public void SetUp()
        {
            _innerAim.SetActive(false);
            _outerAim.SetActive(false);
        }

        public void SetColor(Color color)
        {
            _outerLayer.color = color;
            _innerLayer.color = color;
            _centerLayer.color = color;
            _innerAim.GetComponent<MeshRenderer>().material.color = color;
            _outerAim.GetComponent<MeshRenderer>().material.color = color;
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

        public void PlayActiveAimAnimation()
        {
            _innerAim.SetActive(true);
            _outerAim.SetActive(true);
            _aimAnimator.SetBool(_isAimingHash, true);
        }        
        
        public void PlayInActiveAimAnimation()
        {
            _innerAim.SetActive(true);
            _outerAim.SetActive(false);
            _aimAnimator.SetBool(_isAimingHash, false);
        }

        public void StopAiming()
        {
            _aimAnimator.SetBool(_isAimingHash, false);
            _innerAim.SetActive(false);
            _outerAim.SetActive(false);
        }

        private void TriggerAnimation(int triggerID)
        {
            _animator.SetTrigger(triggerID);
        }
    }
}
