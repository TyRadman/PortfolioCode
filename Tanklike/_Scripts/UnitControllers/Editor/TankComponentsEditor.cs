using UnityEditor;
using UnityEngine;

namespace TankLike.UnitControllers.Editor
{
    [CustomEditor(typeof(TankComponents), true)] //true to support inheritance
    public class TankComponentsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TankComponents tank = (TankComponents)target;

            if (GUILayout.Button("Get Components"))
            {
                Undo.RecordObject(tank, "Get Components"); // For undo functionality
                tank.GetComponents();
                EditorUtility.SetDirty(tank); // Mark the object as dirty to ensure changes are saved
            }
        }
    }
}
