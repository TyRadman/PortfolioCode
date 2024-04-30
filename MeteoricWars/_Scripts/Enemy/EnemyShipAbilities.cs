using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    /// <summary>
    /// The holder of the enemy abilities
    /// </summary>
    public class EnemyShipAbilities : MonoBehaviour
    {
        [HideInInspector] public EnemyComponents Components;
        private List<EnemyAbility> m_Abilities = new List<EnemyAbility>();
        private Transform m_AbilitiesHolder;
        private EnemyAbility m_SelectedAbility;
        // values that can be adjusted based on difficulty
        private Vector2 m_SpecialAttackFrequency;

        private void Awake()
        {
            Components = GetComponent<EnemyComponents>();
            Components.HasAbility = true;
            m_AbilitiesHolder = new GameObject("Abilities").transform;
            m_AbilitiesHolder.parent = transform;
            m_AbilitiesHolder.localPosition = Vector2.zero;
            m_AbilitiesHolder.eulerAngles = transform.eulerAngles;
        }

        /// <summary>
        /// Sets up and adds an ability to the ship
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="_ability"></param>
        /// <param name="_styleIndex"></param>
        public void SetUpValues(SpecialAttackValues _info, EnemyAbilityPrefab _ability, int _styleIndex = -1)
        {
            // setting the abilities
            EnemyAbility ability = Instantiate(_ability.Prefab, transform).GetComponent<EnemyAbility>();
            ability.transform.parent = transform;
            ability.transform.localPosition = Vector2.zero;
            ability.transform.eulerAngles = transform.eulerAngles;
            ability.AbilitiesHolder = this;
            m_Abilities.Add(ability);
            ability.gameObject.SetActive(true);
            ability.StyleIndex = _styleIndex;
            ability.AbilityFrequencyRange = _info.Frequency;
            ability.AwakeAbility();
            ability.SetUpAbility(1, ability.StyleIndex);
        }

        public void ClearAbilities()
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                Destroy(m_Abilities[i].gameObject);
            }

            m_Abilities.Clear();
        }

        /// <summary>
        /// Starts the ship's performance for the ability
        /// </summary>
        public void ActivateShip()
        {
            //m_SelectedAbility = m_Abilities[Random.Range(0, m_Abilities.Count)];
            m_SelectedAbility = m_Abilities[Random.Range(0, m_Abilities.Count)];
            Invoke(nameof(ActivateSpecialAttack), m_SelectedAbility.AbilityFrequencyRange.RandomValue());
        }

        /// <summary>
        /// Happens a couple of times during the ship's lifecycle
        /// </summary>
        private void ActivateSpecialAttack()
        {
            Components.Movement.StopMovement();
            Components.ShootingMethod.StopShooting();
            m_SelectedAbility.PerformAbility();
        }

        // these must be called by the end of the ability. The order depends on the ability
        #region After math calls
        public void ResumeMovement()
        {
            Components.Movement.PerformMovement(transform);
        }

        public void ResumeShooting()
        {
            Components.ShootingMethod.PerformShooting();
        }

        public void PerformAbilityAgain()
        {
            CancelInvoke();
            m_SelectedAbility = m_Abilities[Random.Range(0, m_Abilities.Count)];
            Invoke(nameof(ActivateSpecialAttack), m_SelectedAbility.AbilityFrequencyRange.RandomValue());
        }
        #endregion

        public void StopSpecialAttackQueue()
        {
            m_Abilities.ForEach(a => a.ForceStopAbility());
            CancelInvoke();
        }
    }
}