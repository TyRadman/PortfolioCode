using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyDifficultyModifier : MonoBehaviour, IController
    {
        [SerializeField] private List<DifficultyModifier> _modifiers;
        private EnemyComponents _components;
        public bool IsActive { get; set; }

        public void SetUp(EnemyComponents components, float difficulty)
        {
            _components = components;
            _modifiers.ForEach(m => m.ApplyModifier(_components, difficulty));
        }

        public void ApplyModifier(DifficultyModifier modifier, float difficulty = 0f)
        {
            modifier.ApplyModifier(_components, difficulty);
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Restart()
        {

        }

        public void Dispose()
        {

        }
    }
}
