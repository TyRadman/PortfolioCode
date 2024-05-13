using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    public class BossHUD : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;
        [Header("Health")]
        [SerializeField] private Image _healthFillImage;

        [Header("Damage Bar")]
        [SerializeField] private Image _damageBarImage;
        [SerializeField] private float _damageBarShrinkDelay = 1f;
        [SerializeField] private float _damageBarShrinkSpeed = 1f;
        
        private Coroutine _damageBarShrinkCoroutine;
        private float _damageBarShrinkTimer;

        public void Enable(bool value)
        {
            _parent.SetActive(value);
        }

        public void SetupHealthBar(int maxHealth)
        {
            if (_healthFillImage == null) return;

            _healthFillImage.fillAmount = 1f;
            _damageBarImage.fillAmount = 1f;
        }

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (_healthFillImage == null)
            {
                return;
            }

            _healthFillImage.fillAmount = (float)currentHealth / (float)maxHealth;

            if(_damageBarImage.fillAmount > _healthFillImage.fillAmount)
            {
                if (_damageBarShrinkCoroutine != null)
                {
                    StopCoroutine(_damageBarShrinkCoroutine);
                }

                _damageBarShrinkCoroutine = StartCoroutine(DamageBarShrinkRoutine());
            }
        }

        private IEnumerator DamageBarShrinkRoutine()
        {
            float timer = 0f;

            while (timer < _damageBarShrinkDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            while(_damageBarImage.fillAmount > _healthFillImage.fillAmount)
            {
                _damageBarImage.fillAmount -= _damageBarShrinkSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
