using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.SkillTree
{
    /// <summary>
    /// Holds the skill and information related to it for the skill tree like the skill points required, rank, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "SP_NAME_00", menuName = Directories.SKILL_TREE + "/Skill Profile")]
    public class SkillProfile : ScriptableObject
    {
        public SkillHolder SkillHolder;
        public int SkillPointsRequired = 1;
    }
}
