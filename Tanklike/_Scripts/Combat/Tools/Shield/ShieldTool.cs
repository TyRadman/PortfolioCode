using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public class ShieldTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private Shield _shieldPrefab;
        private Shield _createdShield;
        private Transform _tankTransform;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _tankTransform = tankTransform.transform;
            // create the shield
            _createdShield = Instantiate(_shieldPrefab, tankTransform.transform);
            _createdShield.transform.localPosition = Vector3.zero;
            _createdShield.SetShieldAlpha(0f);
            // set the alignment of the shield
            _createdShield.SetShieldUser(TankAlignment.PLAYER);
            // set up the shield's size
            _createdShield.SetSize(tankTransform.GetComponent<TankAdditionalInfo>().ShieldScale);
        }

        public override void UseTool()
        {
            base.UseTool();
            _createdShield.ActivateShield(true, _tankTransform);
            Invoke(nameof(DeactivateShield), _duration);
        }

        private void DeactivateShield()
        {
            _createdShield.ActivateShield(false, _tankTransform);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

    }
}
