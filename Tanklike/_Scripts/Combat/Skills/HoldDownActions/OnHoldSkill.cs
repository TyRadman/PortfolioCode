using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public abstract class OnHoldSkill : Skill
    {
        [Header("Hold down variables")]
        [field: SerializeField] public float HoldDuration;
        protected TankComponents Components;
        [field: SerializeField] public AbilityConstraint Constraints { get; private set; }
        //protected WaitForSeconds _constraintWait;
        [SerializeField] protected float _duration;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);
            Components = tankTransform.GetComponent<TankComponents>();
            SetConstraintDuration();
        }

        public virtual void OnHoldAction(TankComponents tank)
        {
            Components.Constraints.ApplyConstraints(true, Constraints);
            //Components.StartCoroutine(RemoveConstraints());
        }

        protected void RemoveConstraints()
        {
            Components.Constraints.ApplyConstraints(false, Constraints);
        }

        protected virtual void SetConstraintDuration()
        {

        }
    }
}
