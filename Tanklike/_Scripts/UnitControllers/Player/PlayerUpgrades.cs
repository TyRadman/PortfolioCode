using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UI.SkillTree;
using TankLike.SkillTree;

namespace TankLike.UnitControllers
{
    public class PlayerUpgrades : MonoBehaviour, IController
    {
        [SerializeField] private int _skillPoints;

        private PlayerExperience _experience;
        [SerializeField] private SkillTreeHolder _skillTreePrefab;

        public bool IsActive { get; private set; }

        public void SetUp(PlayerExperience experience)
        {
            _experience = experience;
            _experience.OnLevelUp += AddSkillPoint;
        }

        public void AddSkillPoint()
        {
            _skillPoints++;
            //Debug.Log("GOT SKILL POINT!");
        }

        public void AddSkillPoints(int amount)
        {
            _skillPoints += amount;
        }

        public int GetSkillPoints()
        {
            return _skillPoints;
        }

        public SkillTreeHolder GetSkillTree()
        {
            return _skillTreePrefab;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
            _experience.OnLevelUp -= AddSkillPoint;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
