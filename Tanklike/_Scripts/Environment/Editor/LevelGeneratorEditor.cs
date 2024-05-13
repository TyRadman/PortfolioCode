using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.LevelGeneration;
using TankLike.Environment.LevelGeneration;

namespace TankLike
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LevelGenerator gen = (LevelGenerator)target;

            if(GUILayout.Button("Generate Level"))
            {
                gen.EditorGenerateLevel();
            }
        }
    }
}
