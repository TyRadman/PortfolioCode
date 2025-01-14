using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Testing.Playground
{
    using Combat.SkillTree;

    public class PlaygroundAbilitySelectionPopup : MonoBehaviour
    {
        [SerializeField] private List<PlaygroundAbilityButton> _buttons;
        
        private PlaygroundAbilitySelectionUIController _abilitySelectionUIController;
        private PlaygroundAbilityHolderButton _skillHolderButton;
        public void SetUp(PlaygroundAbilitySelectionUIController abilitySelectionUIController)
        {
            _abilitySelectionUIController = abilitySelectionUIController;
        }

        public void Init(List<SkillProfile> skills, PlaygroundAbilityHolderButton skillHolderButton)
        {
            _skillHolderButton = skillHolderButton;

            _buttons.ForEach(b => b.gameObject.SetActive(false));

            for (int i = 0; i < skills.Count; i++)
            {
                _buttons[i].SetUp(_abilitySelectionUIController, skills[i].SkillHolder, _skillHolderButton);
                _buttons[i].gameObject.SetActive(true);
            }
        }
    }
}
