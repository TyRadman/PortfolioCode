using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT
{
    public class BlackboardVariablesContainer : ScriptableObject
    {
        public BehaviorTree BehaviorTree;

#if UNITY_EDITOR
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public Vector2 Size { get; set; } = new Vector2(300f, 300f);
#endif
        [field: SerializeField] public List<ExposedProperty> Variables { get; set; } = new List<ExposedProperty>();
        
        public void RemoveProperty(ExposedProperty propertyToRemove)
        {
            if (BehaviorTree == null || propertyToRemove == null)
            {
                Debug.LogError("Invalid parent or sub-object.");
                return;
            }

            Variables.Remove(propertyToRemove);

            // Remove the sub-object from the parent asset
            AssetDatabase.RemoveObjectFromAsset(propertyToRemove);

            // Destroy the sub-object
            DestroyImmediate(propertyToRemove, true);

            // Save the changes
            AssetDatabase.SaveAssets();
        }


        public T GetValue<T>(string key)
        {
            if (!Variables.Exists(p => p.PropertyName == key))
            {
                Debug.LogError($"No property with the name {key} found in the blackboard");
                return default(T);
            }

            Debug.Log($"successfully got {key} from blackboard");
            return (T)Variables.Find(p => p.PropertyName == key).GetValue();
        }

        public void SetValue<T>(string key, T value)
        {
            ExposedProperty property = Variables.Find(v => v.PropertyName == key);

            if(property == null)
            {
                Debug.LogError($"Blackboard doesn't contain a property with the key {key}");
                return;
            }

            property.SetValue(value);
        }

        public Rect GetDimensions()
        {
            return new Rect(Position, Size);
        }

        public void AddProperty(ExposedProperty property)
        {
            if (Variables == null)
            {
                Variables = new List<ExposedProperty>();
            }

            Variables.Add(property);
        }
    }
}
