using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.SkillTree
{
    [CreateAssetMenu(fileName = "Skill Tree", menuName = Directories.SKILL_TREE + "Skill Tree Prefab")]
    public class SkillTreePrefab : ScriptableObject
    {
        public List<Path> Paths = new List<Path>();

        public void CopyPaths(List<Path> paths)
        {
            Paths.Clear();
            Paths = new List<Path>();

            for (int i = 0; i < paths.Count; i++)
            {
                Paths.Add(paths[i]);
            }
        }
    }
}
