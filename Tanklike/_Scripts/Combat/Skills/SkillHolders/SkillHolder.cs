using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class SkillHolder : ScriptableObject
    {
        public virtual Sprite GetIcon()
        {
            return null;
        }
    }
}
