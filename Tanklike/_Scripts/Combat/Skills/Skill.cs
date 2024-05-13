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

        public virtual void SetUp(Transform tankTransform)
        {

        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetDescription()
        {
            return _description;
        }
    }
}
