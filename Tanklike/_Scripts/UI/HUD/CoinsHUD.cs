using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TankLike.UI
{
    public class CoinsHUD : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private TextMeshProUGUI _coinsText;

        public void DisplayCoinsText(int coinsAmount)
        {
            _coinsText.text = $"{coinsAmount}";

            this.PlayAnimation(_animation);
        }
    }
}
