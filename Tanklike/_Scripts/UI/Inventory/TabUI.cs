using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TankLike.UI;

namespace TankLike.UI.Inventory
{
    public class TabUI : SelectableCell
    {
        [SerializeField] private TextMeshProUGUI _tabText;
        [SerializeField] private Image _tabBG;
        private const float TRANSITION_DURATION = 0.2f;

        public void HighLight(TabActivationSettings setting)
        {
            // if the tab is turned off, then change its values without lerping
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Transition(setting));
            }
            else
            {
                InstantTransition(setting);
            }
        }

        private IEnumerator Transition(TabActivationSettings setting)
        {
            float time = 0f;
            Transform tabTransform = transform;
            Vector3 startTabSize = tabTransform.localScale;
            Vector3 endTabSize = Vector3.one * setting.TabScale;
            Color startTabColor = _tabBG.color;
            Color endTabColor = setting.TabColor;
            Color startTextColor = _tabText.color;
            Color endTextColor = setting.TabTextColor;
            float startTextSize = _tabText.fontSize;
            float endTextSize = setting.TabFontSize;

            while (time < TRANSITION_DURATION)
            {
                time += Time.deltaTime;
                float t = time / TRANSITION_DURATION;
                // lerp the scale
                tabTransform.localScale = Vector3.Lerp(startTabSize, endTabSize, t);
                // tab color
                _tabBG.color = Color.Lerp(startTabColor, endTabColor, t);
                // text Color
                _tabText.color = Color.Lerp(startTextColor, endTextColor, t);
                // text size
                _tabText.fontSize = Mathf.Lerp(startTextSize, endTextSize, t);
                yield return null;
            }

            InstantTransition(setting);
        }

        private void InstantTransition(TabActivationSettings setting)
        {
            transform.localScale = Vector3.one * setting.TabScale;
            _tabBG.color = setting.TabColor;
            _tabText.color = setting.TabTextColor;
            _tabText.fontSize = setting.TabFontSize;
        }

        public void SetName(string name)
        {
            _tabText.text = name;
        }
    }
}
