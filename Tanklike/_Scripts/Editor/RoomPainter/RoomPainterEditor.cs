using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using static Environment.LevelGeneration.RoomPainter;

    [CustomEditor(typeof(RoomPainter), true)]
    public class RoomPainterEditor : Editor
    {
        private ReorderableList paintingRulesList;

        private void Boot()
        {
            // Access the serialized property for the list
            SerializedProperty rulesProperty = serializedObject.FindProperty("PaintingRules");

            // Initialize the ReorderableList
            paintingRulesList = new ReorderableList(serializedObject, rulesProperty, true, true, true, true);

            // Set up the header
            paintingRulesList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Painting Rules");
            };

            // Render each element
            paintingRulesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = rulesProperty.GetArrayElementAtIndex(index);
                rect.y += 4;

                // Display FilterType dropdown at the top
                SerializedProperty filterType = element.FindPropertyRelative("FilterType");
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    filterType,
                    new GUIContent("Filter Type")
                );

                // Render relevant fields based on FilterType
                FilterType selectedFilter = (FilterType)filterType.enumValueIndex;
                rect.y += EditorGUIUtility.singleLineHeight + 4;

                switch (selectedFilter)
                {
                    case FilterType.IsOfType:
                        DrawIsOfTypeFields(element, rect);
                        break;

                    case FilterType.NeighbourWithinDepthByType:
                    case FilterType.NeighbourWithinDepthByTag:
                        DrawNeighbourWithinDepthFields(element, rect, selectedFilter);
                        break;

                    case FilterType.NeighbourWithinRange:
                        DrawNeighbourWithinRangeFields(element, rect);
                        break;
                }
            };

            paintingRulesList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = paintingRulesList.serializedProperty.GetArrayElementAtIndex(index);
                FilterType selectedFilter = (FilterType)element.FindPropertyRelative("FilterType").enumValueIndex;

                float baseHeight = EditorGUIUtility.singleLineHeight * 2; // Base height
                float extraHeight = 0;

                switch (selectedFilter)
                {
                    case FilterType.IsOfType:
                        extraHeight = EditorGUIUtility.singleLineHeight * 2; // For TileType
                        break;

                    case FilterType.NeighbourWithinDepthByType:
                    case FilterType.NeighbourWithinDepthByTag:
                        extraHeight = EditorGUIUtility.singleLineHeight * 3; // For Depth + TileType/Tag
                        break;

                    case FilterType.NeighbourWithinRange:
                        extraHeight = EditorGUIUtility.singleLineHeight * 4; // For MinDepth, MaxDepth, and TileType
                        break;
                }

                return baseHeight + extraHeight;
            };

        }

        public override void OnInspectorGUI()
        {
            if(serializedObject == null || paintingRulesList == null || serializedObject == null)
            {
                Boot();
            }
            DrawDefaultInspector();
            GUILayout.Space(10);
            serializedObject.Update();
            paintingRulesList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawIsOfTypeFields(SerializedProperty element, Rect rect)
        {
            // PerformOpposite dropdown
            SerializedProperty performOpposite = element.FindPropertyRelative("PerformOpposite");
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                performOpposite,
                new GUIContent("Perform Opposite")
            );

            rect.y += EditorGUIUtility.singleLineHeight + 4;

            // TileType dropdown
            SerializedProperty tileType = element.FindPropertyRelative("TileType");
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                tileType,
                new GUIContent("Tile Type")
            );
        }


        private void DrawNeighbourWithinDepthFields(SerializedProperty element, Rect rect, FilterType filterType)
        {
            // PerformOpposite dropdown
            SerializedProperty performOpposite = element.FindPropertyRelative("PerformOpposite");

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                performOpposite,
                new GUIContent(filterType == FilterType.NeighbourWithinDepthByType ? "Not Within Depth" : "Not Within Depth")
            );

            // Depth
            SerializedProperty depth = element.FindPropertyRelative("Depth");
            rect.y += EditorGUIUtility.singleLineHeight + 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                depth,
                new GUIContent("Depth")
            );

            // TileType or DestructableTag dropdown
            SerializedProperty typeProperty = element.FindPropertyRelative(filterType == FilterType.NeighbourWithinDepthByType ? "TileType" : "DestructableTag");
            rect.y += EditorGUIUtility.singleLineHeight + 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                typeProperty,
                new GUIContent(filterType == FilterType.NeighbourWithinDepthByType ? "Tile Type" : "Destructable Tag")
            );
        }

        private void DrawNeighbourWithinRangeFields(SerializedProperty element, Rect rect)
        {
            // PerformOpposite dropdown
            SerializedProperty performOpposite = element.FindPropertyRelative("PerformOpposite");
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                performOpposite,
                new GUIContent("Perform Opposite")
            );

            // MinDepth and MaxDepth
            SerializedProperty minDepth = element.FindPropertyRelative("MinDepth");
            SerializedProperty maxDepth = element.FindPropertyRelative("MaxDepth");
            rect.y += EditorGUIUtility.singleLineHeight + 4;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight),
                minDepth,
                new GUIContent("Min Depth")
            );
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width / 2 + 4, rect.y, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight),
                maxDepth,
                new GUIContent("Max Depth")
            );

            // TileType dropdown
            SerializedProperty tileType = element.FindPropertyRelative("TileType");
            rect.y += EditorGUIUtility.singleLineHeight + 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                tileType,
                new GUIContent("Tile Type")
            );
        }
    }

}
