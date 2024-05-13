using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class BossComponents : TankComponents
    {
        [field: SerializeField] public BossAIController AIController { get; private set; }
        [field: SerializeField] public BossAttackController AttackController { get; private set; }
        [field: SerializeField] public BossAnimations Animations { get; private set; }
        
        public Vector3 RoomCenter { get; private set; }
        public Vector3 RoomSize { get; private set; }

        
        //private void Awake()
        //{
        //    SetUp();
        //}

        public override void GetComponents()
        {
            base.GetComponents();
            AIController = GetComponent<BossAIController>();
            AttackController = GetComponent<BossAttackController>();
        }

        public override void SetUp()
        {
            RoomCenter = ((BossRoom)GameManager.Instance.RoomsManager.CurrentRoom).GetSpawnPoint().position;
            RoomSize = ((BossRoom)GameManager.Instance.RoomsManager.CurrentRoom).RoomSize;
            // Setups for all components
            TankBodyParts.SetUp();
            Movement.SetUp(this);
            Shooter.SetUp(this);
            Health.SetUp(this);
            Visuals.Setup(this);
            AIController.SetUp(this);
            AttackController.SetUp(this);
        }

        public override void OnDeath()
        {
            GameManager.Instance.ScreenFreezer.FreezeScreen(GameManager.Instance.Constants.EnemyDeathFreezeData);
            OnTankDeath?.Invoke();
        }

        public override void Activate()
        {
            Health.Activate();
            Visuals.Activate();
        }
    }
}
