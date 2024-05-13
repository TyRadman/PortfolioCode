using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.UI.Styles
{
    [CreateAssetMenu(fileName = "Text Style", menuName = "UI/ Styles/ Text Style")]
    public class StyleSettings_Text : StyleSettings
    {
        public float FontSize;
        public TMP_FontAsset Font;
        public Color FontColor;
        public bool Bold;
        public bool Italic;
    }
}
