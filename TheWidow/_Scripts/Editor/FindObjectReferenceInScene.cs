using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindObjectReferenceInScene : EditorWindow
{
    GameObject targetObject;

    [MenuItem("Tools/Find References In Scene")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectReferenceInScene>("Find References In Scene");
    }

    void OnGUI()
    {
        return;
        GUILayout.Label("Find References In Scene", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Find References"))
        {
            FindReferencesInScene();
        }
    }

    void FindReferencesInScene()
    {
        if (targetObject == null)
        {
            Debug.LogError("No target object selected.");
            return;
        }

        List<GameObject> referencingObjects = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    // Skip null components
                    continue;
                }

                SerializedObject so = new SerializedObject(component);
                SerializedProperty sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == targetObject)
                        {
                            referencingObjects.Add(obj);
                        }
                    }
                }
            }
        }

        Debug.Log("Found " + referencingObjects.Count + " references in scene.");

        foreach (GameObject referencingObject in referencingObjects)
        {
            Debug.Log("Referenced by: " + referencingObject.name, referencingObject);
        }
    }
}
