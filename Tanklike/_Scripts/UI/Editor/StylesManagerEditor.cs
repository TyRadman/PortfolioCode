using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.UI.Styles
{
    [CustomEditor(typeof(StylesManager))]
    public class StylesManagerEditor : UnityEditor.Editor
    {
        private StylesManager _styles;

        private void OnEnable()
        {
            _styles = (StylesManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //StylesManager styles = (StylesManager)target;

            if (GUILayout.Button("Refresh"))
            {
                _styles.Refresh();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Cache"))
            {
                _styles.CacheAllElements();
            }
        }
    }
}
