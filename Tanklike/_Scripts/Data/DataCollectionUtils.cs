using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.Data.Utils
{
#if UNITY_EDITOR
    public static class DataCollectionUtils
    {
        public static List<T> FindAllObjectsOfTypeInFiles<T>() where T : Component
        {
            AssetDatabase.Refresh();
            List<T> prefabs = new List<T>();
            string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

            foreach (string prefabGUID in allPrefabs)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if(prefab.TryGetComponent(out T component))
                {
                    prefabs.Add(component);
                }

            }

            return prefabs;
        }

        public static List<T> FindAllScriptableObjectsOfTypeInFiles<T>() where T : ScriptableObject
        {
            AssetDatabase.Refresh();
            List<T> prefabs = new List<T>();
            string[] allPrefabs = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets" });

            foreach (string prefabGUID in allPrefabs)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                ScriptableObject prefab = AssetDatabase.LoadAssetAtPath<ScriptableObject>(prefabPath);

                if (prefab is T)
                {
                    prefabs.Add(prefab as T);
                }

            }

            return prefabs;
        }
    }
#endif
}
