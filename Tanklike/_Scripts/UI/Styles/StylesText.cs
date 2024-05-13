using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.UI.Styles
{
    public class StylesText : StyleElement
    {
        private TextMeshProUGUI _text;

        public override void ApplyStyle(StyleSettings selectedStyle)
        {
            base.ApplyStyle(selectedStyle);

            StyleSettings_Text style = (StyleSettings_Text)selectedStyle;

            if (_text == null) _text = GetComponent<TextMeshProUGUI>();

            _text.font = style.Font;
            _text.fontSize = style.FontSize;
            _text.color = style.FontColor;
        }

        public override void SetUp()
        {
            base.SetUp();

            _text = GetComponent<TextMeshProUGUI>();
        }
    }
}
