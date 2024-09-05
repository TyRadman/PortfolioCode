using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    using UnityEngine.UI;
    using Utils;

    /// <summary>
    /// A bar that can change its max value based on the values provided.
    /// </summary>
    public class ResizableBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _barTransform;
        [SerializeField] private Image _barFillImage;
        private float _minFillAmount;
        [SerializeField] private float _maxSize;
        [SerializeField] private Vector2 _positionRange;

        /// <summary>
        /// Sets the max size based on the value provided.
        /// </summary>
        /// <param name="size">A float value from 0 to 1.</param>
        public void SetMaxSize(float size, bool fillHealthBar = false)
        {
            _minFillAmount = 1 - size;
            float xPosition = _positionRange.Lerp(1 - size);
            _barTransform.anchoredPosition = new Vector2(xPosition, _barTransform.anchoredPosition.y);

            if (fillHealthBar)
            {
                SetValue(1f);
            }
        }

        /// <summary>
        /// Set the bar value.
        /// </summary>
        /// <param name="value">A float value from 0 to 1.</param>
        public void SetValue(float value)
        {
            _barFillImage.fillAmount = Mathf.Lerp(_minFillAmount, 1f, value);
        }
    }
}
