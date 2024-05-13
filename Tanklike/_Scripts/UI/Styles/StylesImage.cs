using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.Styles
{
    public class StylesImage : StyleElement
    {
        [SerializeField] private Image _image;

        public override void ApplyStyle(StyleSettings selectedStyle)
        {
            return;
            base.ApplyStyle(selectedStyle);

            StyleSettings_Image style = (StyleSettings_Image)selectedStyle;

            _image.color = style.Color;
        }

        public override void SetUp()
        {
            base.SetUp();

            _image = GetComponent<Image>();
        }
    }
}
