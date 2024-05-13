using System.Collections;
using System.Collections.Generic;
using TankLike.Misc;
using TankLike.ScreenFreeze;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public abstract class Ability : Skill
    {
        /// <summary>
        /// The prefix of the asset
        /// </summary>
        public const string PREFIX = "Ability_";


        [field: SerializeField] public AbilityConstraint Constraints { get; private set; }
        [Header("Values")]
        public SkillTag SkillTag;
        [SerializeField][Range(0.5f, 20)] protected float _duration = 0.5f;
        [SerializeField] protected float _coolDown = 3f;
        [SerializeField] private bool _canHoldAbility;
        [SerializeField] private ScreenFreezeData _freezeData;
        protected Transform _tank;
        protected TankComponents _components;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            _tank = tankTransform;
            _components = tankTransform.GetComponent<TankComponents>();
            TankComponents tank = tankTransform.GetComponent<TankComponents>();
            // set the ability's constraints to the tank
            tank.SuperAbility.SetConstraints(Constraints);
        }

        public virtual void SetUpAbility(float holdAmount)
        {

        }

        /// <summary>
        /// Starts the skill
        /// </summary>
        public virtual void OnActivateAbility()
        {
            if (_freezeData != null)
            {
                GameManager.Instance.ScreenFreezer.FreezeScreen(_freezeData);
            }
        }
        
        public virtual void OnAbilityHoldStart()
        {

        }

        /// <summary>
        /// Called during the coroutine of the super ability hold down
        /// </summary>
        public virtual void OnAbilityHoldUpdate()
        {

        }

        public virtual void OnAbilityFinished()
        {

        }

        public virtual void OnAbilityCancelled()
        {

        }

        /// <summary>
        /// Called once the skill has finished 
        /// </summary>
        public virtual void DeactivateAbility()
        {

        }

        public float GetCoolDownTime()
        {
            return _coolDown;
        }

        public float GetDuration()
        {
            return _duration;
        }

        public virtual void SetDuration()
        {

        }

        public void ApplyConstraints(bool apply)
        {
            _components.Constraints.ApplyConstraints(apply, Constraints);
        }
    }
}

public enum SkillTag
{
    ShotGun = 0, Brock = 1, CustomShot = 2
}
