using TankLike.UI.SkillTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Environment;
using TankLike.UI;
using TankLike.UI.Inventory;

namespace TankLike
{
    public class InteractableAreasManager : MonoBehaviour
    {
        [Header("Areas Screens")]
        [SerializeField] private WorkShopTabsNavigatable _workshop;
        [SerializeField] private ToolsNavigator _toolsShop;
        [SerializeField] private InteractableTextBox _textBoxPrefab;
        private List<InteractableTextBox> _boxes = new List<InteractableTextBox>();

        public void SetUp()
        {
            // create text boxes
            for (int i = 0; i < GameManager.Instance.PlayersManager.GetPlayersCount(); i++)
            {
                InteractableTextBox box = Instantiate(_textBoxPrefab, transform);
                box.gameObject.SetActive(false);
                _boxes.Add(box);
            }
        }

        public void ActivateTextBox(Transform buttonDisplayPosition, string interactionText, int playerIndex)
        {
            _boxes[playerIndex].gameObject.SetActive(true);
            _boxes[playerIndex].SetInteractionText(interactionText);
            _boxes[playerIndex].SetPosition(buttonDisplayPosition);
            _boxes[playerIndex].PlayOpenAnimation(true);
        }

        public void SetFeedbackText(string text, int playerIndex)
        {
            _boxes[playerIndex].SetFeedbackText(text);
        }

        public void DeactivateTextBox(int playerIndex)
        {
            _boxes[playerIndex].PlayOpenAnimation(false);
        }

        public void OpenShop(int playerIndex)
        {
            _toolsShop.Open(playerIndex);
        }

        public void OpenWorkshop(int playerIndex)
        {
            _workshop.Open(playerIndex);
        }
    }
}
