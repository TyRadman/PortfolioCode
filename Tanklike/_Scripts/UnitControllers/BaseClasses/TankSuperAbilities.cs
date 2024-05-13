using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankSuperAbilities : MonoBehaviour, IController
    {
        public System.Action OnSuperPerformed;

        [SerializeField] protected List<SuperAbilityHolder> _superAbilities = new List<SuperAbilityHolder>();
        [SerializeField] protected TankComponents _components;
        protected AbilityConstraint _constraints;
        protected SuperAbilityHolder _tankSuperAbility;
        protected bool _canUseAbility = true;

        public bool IsActive { get; protected set; }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            
        }

        public virtual void UseAbility()
        {
            // use the skill
            _tankSuperAbility.Ability.OnActivateAbility();
        }

        public virtual void AddSkill(SuperAbilityHolder ability)
        {
            // temporary. We need to look into whether we need scriptable objects for this, or stick to in-game instances
            _tankSuperAbility = Instantiate(ability);
            _tankSuperAbility.Ability = Instantiate(_tankSuperAbility.Ability);
            _superAbilities.Add(_tankSuperAbility);
            _tankSuperAbility.Ability.SetUp(_components.transform);
        }

        public void EnableAbility(bool canUseAbility)
        {
            _canUseAbility = canUseAbility;
        }

        public void SetConstraints(AbilityConstraint constraints)
        {
            _constraints = constraints;
        }

        public void SetSuperAbility(Ability ability)
        {
            // if the ability doesn't exist in the super abilities list, then add it
            if (!_superAbilities.Exists(a => a == ability))
            {
                Debug.LogWarning("Ability to set doesn't exist. Have you instantiated the ability somewhere else?");
            }

            _tankSuperAbility = _superAbilities.Find(a => a.Ability.SkillTag == ability.SkillTag);
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            IsActive = false;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
