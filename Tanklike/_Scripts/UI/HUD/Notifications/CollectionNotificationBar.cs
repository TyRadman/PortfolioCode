using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Notifications
{
    public class CollectionNotificationBar : MonoBehaviour
    {
        public bool IsAvailable { get; private set; }
        public bool CanAddTo { get; private set; } = true;
        public RectTransform Rect;
        public NotificationType Type { get; private set; }
        [SerializeField] private TextMeshProUGUI _notificationText;
        [SerializeField] private Image _notificationIcon;

        [Header("Animations")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;

        private Coroutine _countDownCoroutine;
        private WaitForSeconds _countDownWait = new WaitForSeconds(4);

        private int _itemAmount = 0;

        public void SetUp()
        {
            IsAvailable = true;
        }

        public void Display(string text, Sprite iconSprite, NotificationType notificationType, int amount)
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

            if (IsAvailable)
            {
                IsAvailable = false;

                this.PlayAnimation(_animation, _showClip);

                transform.SetAsLastSibling();
            }

            this.StopCoroutineSafe(_countDownCoroutine);
            _countDownCoroutine = StartCoroutine(CountDownProcess());
        }

        private IEnumerator CountDownProcess()
        {
            yield return _countDownWait;

            this.PlayAnimation(_animation, _hideClip);
            IsAvailable = true;
            CanAddTo = true;
        }
    }
}
