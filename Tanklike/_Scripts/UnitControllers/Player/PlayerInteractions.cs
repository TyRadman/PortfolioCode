using TankLike.Environment;
using TankLike.Sound;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerInteractions : MonoBehaviour, IController, IInput
    {
        private PlayerComponents _components;
        private InteractableArea _interactable;
        [SerializeField] private Audio _onSwitchAudio;

        public bool IsActive { get; private set; }

        public void SetUp(PlayerComponents components)
        {
            _components = components;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap interactionMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            interactionMap.FindAction(c.Player.Jump.name).performed += Interact;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap interactionMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            interactionMap.FindAction(c.Player.Jump.name).performed -= Interact;
        }
        #endregion

        public void OnInteractionAreaEnter(InteractableArea interactable, Transform buttonDisplayPosition, string interactionText, AbilityConstraint constraints)
        {
            _interactable = interactable;
            // enable the interaction input
            SetUpInput(_components.PlayerIndex);
            // disable shooting input by apply constraints
            _components.Constraints.ApplyConstraints(true, constraints);
            //GameManager.Instance.InputManager.EnableInteractionInput(true);
            GameManager.Instance.InteractableAreasManager.ActivateTextBox(buttonDisplayPosition, interactionText);
        }

        public void OnInteractionAreaExit(AbilityConstraint constraints)
        {
            _interactable = null;
            // disable input
            DisposeInput(_components.PlayerIndex);
            // enable shooting and other constraints that have been applied
            _components.Constraints.ApplyConstraints(false, constraints);
            //GameManager.Instance.InputManager.EnablePlayerInput();
            GameManager.Instance.InteractableAreasManager.DeactivateTextBox(_components.PlayerIndex);
        }

        public void Interact(InputAction.CallbackContext _)
        {
            _interactable.Interact(_components.PlayerIndex);
            GameManager.Instance.AudioManager.Play(_onSwitchAudio);
        }

        #region IController

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            // manually exit the interactable area if the player is inside one
            if (_interactable != null)
            {
                _interactable.StopInteraction();
            }

            IsActive = false;
        }

        public void Dispose()
        {
            DisposeInput(_components.PlayerIndex);
        }
        #endregion
    }
}
