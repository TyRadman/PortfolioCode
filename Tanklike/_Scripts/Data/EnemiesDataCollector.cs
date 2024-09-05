using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace TankLike.Data
{
    using UnitControllers;
    using Data.Utils;
    using Combat;

#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "EnemiesData_", menuName = Directories.DATA_COLLECTION + "Enemies Data")]
    public class EnemiesDataCollector : ScriptableObject
    {
        [SerializeField] private List<EnemyData> _allEnemies = new List<EnemyData>();
        [SerializeField] private string _fileName = "EWS_Enemies.csv";
        private const string PATH = "/Data/";

        public void CacheAllEnemies()
        {
            _allEnemies.Clear();
            _allEnemies = DataCollectionUtils.FindAllScriptableObjectsOfTypeInFiles<EnemyData>();
        }

        public void SaveDataToCSV()
        {
            string[] data = new string[_allEnemies.Count + 1];

            data[0] += "Enemy Type, ";
            data[0] += "Experience Per Kill, ";
            data[0] += "Rank";

            for (int i = 0; i < _allEnemies.Count; i++)
            {
                EnemyData enemy = _allEnemies[i];

                data[i + 1] += $"{enemy.EnemyType}, ";
                data[i + 1] += $"{enemy.ExperiencePerKill}, ";
                data[i + 1] += $"{enemy.Rank}";
            }

            CSVWriter.WriteCSV(data, Path.Combine(PATH, _fileName));
        }
    }
#endif
}
