using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using UnitControllers;

    /// <summary>
    /// The class that holds a sequence of upgrades of a skill.
    /// </summary>
    [CreateAssetMenu(fileName = "UCC_NAME", menuName = Directories.SKILL_TREE + "Upgrades Conditional Container")]
    public class SkillsConditionalContainer : SkillUpgrade
    {
        [System.Serializable]
        public class ConditionalUpgrade
        {
            [field: SerializeField] public SkillUpgrade Upgrade { get; set; }
            [field: SerializeField] public bool IsAvailable { get; set; } = false;

            /// <summary>
            /// The upgrades that must be unlocked to make this upgrade available.
            /// </summary>
            [field: SerializeField] public List<SkillUpgrade> UpgradesToUnlock { get; set; } = new List<SkillUpgrade>();

            public void SetUp(PlayerComponents player)
            {
                IsAvailable = false;
            }

            public bool IsAvailableToUnlock()
            {
                // if any of the upgrades to unlock is available, then this upgrade is not available
                for (int i = 0; i < UpgradesToUnlock.Count; i++)
                {
                    Debug.Log($"Checking upgrade {UpgradesToUnlock[i].name} and its availability is: {UpgradesToUnlock[i].IsAvailable}");

                    if (UpgradesToUnlock[i].IsAvailable)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [field: SerializeField] public List<ConditionalUpgrade> ConditionalUpgrades { get; private set; }

        public override void SetUp(PlayerComponents player)
        {
            ConditionalUpgrades.ForEach(u => u.Upgrade.SetUp(player));
        }

        internal List<SkillUpgrade> GetAvailableUpgrades()
        {
            List<SkillUpgrade> upgrades = new List<SkillUpgrade>();

            for (int i = 0; i < ConditionalUpgrades.Count; i++)
            {
                if(ConditionalUpgrades[i].IsAvailableToUnlock())
                {
                    upgrades.Add(ConditionalUpgrades[i].Upgrade);
                    Debug.Log($"Available Conditional upgrade: {ConditionalUpgrades[i].Upgrade.name}");
                }
            }

            return upgrades;
        }

        internal override SkillUpgrade Clone()
        {
            SkillsConditionalContainer clone = Instantiate(this);

            clone.ConditionalUpgrades = new List<ConditionalUpgrade>();

            for (int i = 0; i < ConditionalUpgrades.Count; i++)
            {
                ConditionalUpgrade upgrade = new ConditionalUpgrade()
                {
                    Upgrade = ConditionalUpgrades[i].Upgrade.Clone(),
                    IsAvailable = ConditionalUpgrades[i].IsAvailable,
                    UpgradesToUnlock = new List<SkillUpgrade>(),
                };

                clone.ConditionalUpgrades.Add(upgrade);
            }

            return clone;
        }
    }
}
