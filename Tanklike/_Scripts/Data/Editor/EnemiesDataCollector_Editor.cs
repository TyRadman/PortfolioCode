using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.Data.DataEditor
{
    [CustomEditor(typeof(EnemiesDataCollector))]
    public class EnemiesDataCollector_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EnemiesDataCollector collector = (EnemiesDataCollector)target;
            
            if(GUILayout.Button("Collect Enemies Components"))
            {
                collector.CacheAllEnemies();
            }

            if (GUILayout.Button("Save"))
            {
                collector.SaveDataToCSV();
            }
        }
    }
}
