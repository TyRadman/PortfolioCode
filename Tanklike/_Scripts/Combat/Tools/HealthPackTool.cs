using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public class HealthPackTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private int _healthAmountPerPack = 30;
        private TankHealth _tankHealth;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _tankHealth = tankTransform.GetComponent<TankHealth>();
        }

        public override void UseTool()
        {
            base.UseTool();

            // we pass null for now, so we don't need to know who did the healing for a given tank
            _tankHealth.TakeDamage(-_healthAmountPerPack, Vector3.zero, null, Vector3.zero);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

    }
}
