using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    public class Skill : ScriptableObject
    {
        [HideInInspector] public bool _isUnlocked = false;
        [SerializeField] protected string _name;
        [SerializeField][TextArea(3, 10)] protected string _description;
        [SerializeField] private Sprite _icon;

        protected const string FILE_NAME_PREFIX = "Stat_";
        protected const string MENU_ROOT = Directories.SKILLS + "Stats/";

        public string Name => _name;
        public string Description => _description;

        public virtual void SetUp(TankComponents components)
        {
        }

        public Sprite GetIcon()
        {
            return _icon;
        }
    }
}
