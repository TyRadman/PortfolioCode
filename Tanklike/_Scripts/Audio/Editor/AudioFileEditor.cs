using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace TankLike.Sound
{
    [CustomEditor(typeof(Audio))]
    //[CanEditMultipleObjects]
    public class AudioFileEditor : Editor
    {
        SerializedProperty audioName;
        SerializedProperty clip;
        SerializedProperty volumeMultiplier;
        SerializedProperty oneShot;
        SerializedProperty _pitch;
        SerializedProperty _hasRandomPitch;
        SerializedProperty _minPitch;
        SerializedProperty _maxPitch;

        private void OnEnable()
        {
            // Bind the SerializedProperties
            audioName = serializedObject.FindProperty("AudioName");
            clip = serializedObject.FindProperty("Clip");
            volumeMultiplier = serializedObject.FindProperty("VolumeMultiplier");
            oneShot = serializedObject.FindProperty("OneShot");
            _minPitch = serializedObject.FindProperty("_minPitch");
            _maxPitch = serializedObject.FindProperty("_maxPitch");
            _pitch = serializedObject.FindProperty("_pitchValue");
            _hasRandomPitch = serializedObject.FindProperty("_randomPitch");
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            EditorGUILayout.PropertyField(audioName);
            EditorGUILayout.PropertyField(clip);
            EditorGUILayout.PropertyField(volumeMultiplier);
            EditorGUILayout.PropertyField(oneShot);
            EditorGUILayout.PropertyField(_pitch);
            EditorGUILayout.PropertyField(_hasRandomPitch);

            if (_hasRandomPitch.boolValue)
            {
                EditorGUILayout.PropertyField(_minPitch);
                EditorGUILayout.PropertyField(_maxPitch);

                if (_minPitch.floatValue > _maxPitch.floatValue)
                {
                    _minPitch.floatValue = _maxPitch.floatValue;
                }
            }

            serializedObject.ApplyModifiedProperties();

            Audio audio = (Audio)target;

            if (audio.name == audio.AudioName)
                return;

            if (audio.AudioName.Length > 0)
            {
                var fileName = string.Join("", audio.name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                audio.AudioName = string.Concat(fileName);
            }
        }
    }
}
