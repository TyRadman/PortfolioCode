using System.Collections;
using System.Collections.Generic;
using TankLike.Misc;
using TankLike.ScreenFreeze;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public abstract class Ability : ScriptableObject
    {
        /// <summary>
        /// The prefix of the asset
        /// </summary>
        public const string PREFIX = "Ability_";

        [Header("Values")]
        [SerializeField][Range(0.5f, 20)] protected float _duration = 0.5f;
        [SerializeField] protected float _coolDown = 3f;
        [SerializeField] private bool _canHoldAbility;
        [SerializeField] private ScreenFreezeData _freezeData;

        protected Transform _tank;
        protected TankComponents _components;

        public virtual void SetUp(TankComponents components)
        {
            _tank = components.transform;
            _components = components;
        }

        /// <summary>
        /// Starts the ability
        /// </summary>
        public virtual void PerformAbility()
        {
            // TODO: Remove from base class and implement it on each derivitive
            if (_freezeData != null)
            {
                GameManager.Instance.ScreenFreezer.FreezeScreen(_freezeData);
            }
        }
        
        /// <summary>
        /// Called when the ability hold-down starts
        /// </summary>
        public virtual void OnAbilityHoldStart()
        {

        }

        /// <summary>
        /// Called during the coroutine of the super ability hold down.
        /// </summary>
        public virtual void OnAbilityHoldUpdate()
        {

        }

        /// <summary>
        /// Called when the ability finishes performing.
        /// </summary>
        public virtual void OnAbilityFinished()
        {

        }

        /// <summary>
        /// Called when the ability is interrupted.
        /// </summary>
        public virtual void OnAbilityInterrupted()
        {

        }

        /// <summary>
        /// Returns the duration of the ability.
        /// </summary>
        /// <returns></returns>
        public virtual float GetDuration()
        {
            return _duration;
        }

        /// <summary>
        /// Sets up the indicator by setting values that the ability has.
        /// </summary>
        /// <param name="indicator"></param>
        public virtual void SetUpIndicatorSpecialValues(BaseIndicator indicator)
        {

        }

        /// <summary>
        /// Disposes the ability and all its dependencies.
        /// </summary>
        public virtual void Dispose()
        {

        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
