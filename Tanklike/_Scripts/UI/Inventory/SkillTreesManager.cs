using System.Collections;
using System.Collections.Generic;
using TankLike.UI;
using UnityEngine;

namespace TankLike.SkillTree
{
    public class SkillTreesManager : Navigatable
    {
        [SerializeField] private Transform _skillTreesParent;
        [SerializeField] private List<SkillTreeHolder> _skillTrees = new List<SkillTreeHolder>();

        public override void SetUp()
        {
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                SkillTreeHolder skillTreePrefab = GameManager.Instance.PlayersManager.GetPlayer(i).Upgrades.GetSkillTree();
                SkillTreeHolder newSkillTree = Instantiate(skillTreePrefab, _skillTreesParent);
                _skillTrees.Add(newSkillTree);
                newSkillTree.SetPlayerIndex(i);
                newSkillTree.SetUp();
            }
        }

        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);
            _skillTrees.ForEach(s => s.gameObject.SetActive(false));
            _skillTrees[playerIndex].gameObject.SetActive(true);
            _skillTrees[playerIndex].Open(playerIndex);
        }
    }
}
