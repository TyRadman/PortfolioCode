using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.Styles
{
    // TODO: we need to remove this class
    public class StylesImage : StyleElement
    {
        [SerializeField] private Image _image;

        public override void ApplyStyle(StyleSettings selectedStyle)
        {
            return;
        }

        public override void SetUp()
        {
            base.SetUp();

            _image = GetComponent<Image>();
        }
    }
}
