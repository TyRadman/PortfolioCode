using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyComponents : TankComponents
    {
        [field: SerializeField] public EnemyAIController EnemyController { get; private set; }
        [field: SerializeField] public EnemyItemDropper ItemDrop { get; private set; }
        [field: SerializeField] public EnemyDifficultyModifier DifficultyModifier { get; private set; }

        private void Awake()
        {
            SetUp();
        }

        public override void GetComponents()
        {
            base.GetComponents();
            EnemyController = GetComponent<EnemyAIController>();
            ItemDrop = GetComponent<EnemyItemDropper>();
        }

        public override void SetUp()
        {
            base.SetUp();
            EnemyController.SetReferences(this);
        }

        public override void Activate()
        {
            base.Activate();

            DifficultyModifier.SetUp(this, GameManager.Instance.EnemiesManager.Difficulty);
        }

        public override void Dispose()
        {
            base.Dispose();
            EnemyController.Dispose();
            ItemDrop.Dispose();
        }
    }
}
