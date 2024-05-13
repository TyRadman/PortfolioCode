using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Styles
{
    public enum StyleName
    {
        None = 100, TabsText = 0, TabsTitle = 1, TabContentText = 2, Notifications = 3, InverntoryQuests = 4, InventoryInputKey = 5, InventoryInputAction = 6, SkillTreeSwapables = 7
    }

    public class StylesManager : MonoBehaviour
    {
        [SerializeField] private List<StyleSettings> _styles;
        [SerializeField] private List<StyleElement> _styledElements = new List<StyleElement>();

        public StyleSettings GetStyleByName(StyleName name)
        {
            return _styles.Find(s => s.StyleName == name);
        }

        public void AddElement(StyleElement element)
        {
            _styledElements.Add(element);
            List<StyleElement> stylesToRemove = new List<StyleElement>();

            // remove empty styles
            foreach (StyleElement styleElement in _styledElements)
            {
                if(styleElement == null || _styledElements.Exists(s => s == styleElement))
                {
                    stylesToRemove.Add(styleElement);
                }
            }

            stylesToRemove.ForEach(s => _styledElements.Remove(s));
        }

        public void Refresh()
        {
            foreach (StyleElement item in _styledElements)
            {
                StyleSettings selectedStyle = _styles.Find(s => s.StyleName == item.StyleName);

                if (selectedStyle != null) item.ApplyStyle(selectedStyle);
            }
        }

        public void CacheAllElements()
        {
            _styledElements.Clear();

            var elements = FindObjectsOfType<StyleElement>();

            foreach (StyleElement item in elements)
            {
                _styledElements.Add(item);
            }
        }
    }
}
