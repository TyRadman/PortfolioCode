using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Notifications
{
    public class CollectionNotificationBar : MonoBehaviour
    {
        public bool IsAvailable;
        public bool CanAddTo = true;
        public int CurrentIndex = -1;
        public RectTransform Rect;
        public NotificationType Type;
        [SerializeField] private TextMeshProUGUI _notificationText;
        [SerializeField] private Image _notificationIcon;
        [SerializeField] private CanvasGroup _canvasGroup;
        public const float FADE_OUT_DURATION = 1f;
        private int _itemAmount = 0;

        public void FillInfo(string text, Sprite iconSprite, NotificationType notificationType, int amount)
        {
            _itemAmount += amount;

            if (_itemAmount <= 0)
            {
                _notificationText.text = $"{text}";
            }
            else
            {
                _notificationText.text = $"+{_itemAmount} {text}";
            }

            _notificationIcon.sprite = iconSprite;
            Type = notificationType;
        }

        public void Enable()
        {
            // if it's the first time it gets enabled, then make it fade in
            if (IsAvailable)
            {
                IsAvailable = false;
                _canvasGroup.alpha = 0f;
                gameObject.SetActive(true);
                StartCoroutine(FadeBarIn());
                transform.SetAsLastSibling();
            }
        }

        private IEnumerator FadeBarIn()
        {
            float time = 0f;

            while (time < FADE_OUT_DURATION)
            {
                time += Time.deltaTime;
                float t = time / FADE_OUT_DURATION;

                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
        }

        public void StartCountDown(float delay)
        {
            CancelInvoke();
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            Invoke(nameof(CountDown), delay);
        }

        private void CountDown()
        {
            StartCoroutine(FadeBarOut());
        }

        private IEnumerator FadeBarOut()
        {
            float time = 0f;
            _itemAmount = 0;
            CanAddTo = false;

            while (time < FADE_OUT_DURATION)
            {
                time += Time.deltaTime;
                float t = time / FADE_OUT_DURATION;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

                yield return null;
            }

            IsAvailable = true;
            CanAddTo = true;
            gameObject.SetActive(false);
        }
    }
}
