using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class SkillHolder : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField] [TextArea(3, 10)] protected string _description;
        [SerializeField] private Sprite _icon;

        public string Name => _name;
        public string Description => _description;

        public virtual Sprite GetIcon()
        {
            return _icon;
        }

        public virtual string GetName()
        {
            return _name;
        }

        public virtual string GetDescription()
        {
            return _description;
        }
    }
}
