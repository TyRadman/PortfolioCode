using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyDifficultyModifier : MonoBehaviour
    {
        [SerializeField] private List<DifficultyModifier> _modifiers;

        public void SetUp(EnemyComponents components, float difficulty)
        {
            _modifiers.ForEach(m => m.ApplyModifier(components, difficulty));
        }
    }
}
