using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.SkillTree
{
    public class SkillTreeRandomSkillMenuController : MonoBehaviour
    {
        [SerializeField] private SkillTreeCell _firstCell;
        [SerializeField] private SkillTreeCell _secondCell;
        [SerializeField] private GameObject _content;

        public void Open()
        {
            _content.SetActive(true);
            _firstCell.HighLight(false);
            _secondCell.HighLight(false);
        }

        public void Close()
        {
            _content.SetActive(false);
        }

        public void SetSkills(SkillProfile firstSkill, SkillProfile secondCell)
        {
            _firstCell.SetSkillProfile(firstSkill);
            _secondCell.SetSkillProfile(secondCell);
        }

        public SkillTreeCell GetFirstCell()
        {
            return _firstCell;
        }

        public SkillTreeCell GetOtherCell(SkillTreeCell cell)
        {
            return cell == _firstCell ? _secondCell : _firstCell;
        }
    }
}
