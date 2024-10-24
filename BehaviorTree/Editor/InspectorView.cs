using BT;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using BT.NodesView;
using UnityEngine;
using BT.Nodes;

public class InspectorView : VisualElement
{
    public new class UxmlFactor : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
    // the custom inspector
    private Editor _editor;
    private NodeView _currentNodeView;
    private static Texture2D S_BlackBoardKeyIcon;

    public InspectorView()
    {

    }

    internal void UpdateSelection(NodeView nodeView)
    {
        _currentNodeView = nodeView;
        // clear previous selection elements
        Clear();
        // destroy the previous editor
        Object.DestroyImmediate(_editor);
        
        // create a new editor and pass the class to serialize the properties of
        _editor = Editor.CreateEditor(nodeView.Node);
        // create a container to hold the values of the editor
        IMGUIContainer container = new IMGUIContainer(RenderInspector);
        // add the container as a visual element to the inspector view
        Add(container);
    }

    private void RenderInspector()
    {
        if (_editor.target == null)
        {
            return;
        }

        //RenderDisplaySettings();

        // Begin listening to changes in the inspector GUI
        EditorGUI.BeginChangeCheck();

        // Get the serialized object
        SerializedObject serializedObject = _editor.serializedObject;
        serializedObject.Update();

        // Iterate through all serialized properties
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;

        UnityEngine.Object node = _editor.target;
        List<FieldInfo> blackboardFields = new List<FieldInfo>();

        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            // Skip rendering if the field is of type BlackboardKey
            FieldInfo field = _editor.target.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (field != null && field.FieldType == typeof(BlackboardKey))
            {
                blackboardFields.Add(field);
                continue;
            }

            // Draw all other fields normally
            EditorGUILayout.PropertyField(property, true);
        }

        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < blackboardFields.Count; i++)
        {
            BlackboardKey blackboardKey = blackboardFields[i].GetValue(node) as BlackboardKey;
            string blackboardName = ObjectNames.NicifyVariableName(blackboardFields[i].Name);
            DrawBlackboardKeyDropdown(blackboardKey, blackboardName);
        }

        // Apply changes made during GUI rendering
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawBlackboardKeyDropdown(BlackboardKey blackboardKey, string name)
    {
        Texture2D icon = GetBlackboardKeyIcon();

        // Create GUIContent with the icon and the label text
        float iconWidth = 16f;
        float iconHeight = 16f;
        GUIContent dropdownLabel = new GUIContent(" " + name, icon);

        List<string> options = GetBlackboardKeys();
        // highlight the selected element
        int selectedIndex = options.IndexOf(blackboardKey.Value);

        if (selectedIndex == -1)
        {
            selectedIndex = 0;
        }

        int newSelectedIndex = EditorGUILayout.Popup(dropdownLabel, selectedIndex, options.ToArray());

        if (newSelectedIndex != selectedIndex)
        {
            // Update BlackboardKey.Value when a new option is selected
            blackboardKey.Value = options[newSelectedIndex];

            EditorUtility.SetDirty(_currentNodeView.Node);
        }
    }

    private List<string> GetBlackboardKeys()
    {
        List<string> blackboardKeys = new List<string>();
        List<ExposedProperty> properties = _currentNodeView.Node.Blackboard.Variables;

        for (int i = 0; i < properties.Count; i++)
        {
            blackboardKeys.Add(properties[i].PropertyName);
        }

        return blackboardKeys;
    }

    private Texture2D GetBlackboardKeyIcon()
    {
        if (S_BlackBoardKeyIcon != null)
        {
            return S_BlackBoardKeyIcon;
        }

        string path = BehaviorTreeSettings.CORE_DIRECTORY + "/Icons/T_KeyIcon_small.png";
        S_BlackBoardKeyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        return S_BlackBoardKeyIcon;
    }
}
