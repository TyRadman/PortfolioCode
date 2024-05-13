using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Styles
{
    public abstract class StyleElement : MonoBehaviour
    {
        public StyleName StyleName;

        public virtual void ApplyStyle(StyleSettings selectedStyle)
        {

        }

        public virtual void SetUp()
        {
            GameManager.Instance.StylesManager.AddElement(this);
        }
    }
}
