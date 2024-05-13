using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TankLike.Combat;

namespace TankLike.UI
{
    public class ToolIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Image _amountPanel;
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private TextMeshProUGUI _keyText;

        public void SetUp(Tool tool)
        {
            tool.SetToolIconUI(this);
            _iconImage.sprite = tool.GetIcon();
            _amountText.text = tool.GetAmount().ToString("0");
            _amountPanel.enabled = true;
        }

        public void SetAmountText(string amount)
        {
            _amountText.text = amount;
        }

        public void ResetIcon(Sprite emptyIcon)
        {
            _iconImage.sprite = emptyIcon;
            _amountText.text = string.Empty;
            _amountPanel.enabled = false;
        }

        public void SetCooldownOverlayFill(float amount)
        {
            _cooldownOverlay.fillAmount = amount;
        }

        public void SetKey(string key)
        {
            _keyText.text = key;
        }
    }
}
