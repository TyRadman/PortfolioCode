using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.SkillTree
{
    /// <summary>
    /// Holds a list of skill profiles for the skill tree to use.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillsDatabase_", menuName = Directories.SKILL_TREE + "/Skills Database")]
    public class SkillTreeSkillsDatabase : ScriptableObject
    {
        public List<SkillProfile> Skills;
    }
}
