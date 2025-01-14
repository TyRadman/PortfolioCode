using System;
using UnityEngine;

namespace TankLike.Environment
{
    using UnitControllers;

    [RequireComponent(typeof(Collider))]
    public class InteractableArea : MonoBehaviour
    {
        [field: SerializeField] public Action OnInteractorExit { get; set; }
        
        [SerializeField] protected Transform _displayInteractionTextParent;
        [SerializeField] protected string _interactionText;
        [SerializeField] private AbilityConstraint _onEnterConstraints;
        [SerializeField] protected bool _isActive;
        [SerializeField] private Collider _collider;
        
        protected PlayerInteractions _currentInteractor;
        protected bool _hasInteractor = false;
        protected int _currentPlayerIndex;

        public virtual void SetUp()
        {

        }

        #region On Enter
        protected virtual void OnTriggerEnter(Collider other)
        {
            // only add the player as an interactor if the interaction area doesn't have an interactor
            if (other.CompareTag("Player") && _currentInteractor == null)
            {
                OnAreaEntered(other);
            }
        }

        protected virtual void OnAreaEntered(Collider other)
        {
            if(_hasInteractor)
            {
                return;
            }

            PlayerComponents player = other.GetComponent<PlayerComponents>();

            if (player == null)
            {
                return;
            }

            _hasInteractor = true;

            // stop the hold action
            //player.OnHold.ForceStopHoldAction();
            _currentInteractor = player.PlayerInteractions;
            _currentInteractor.OnInteractionAreaEnter(this, _displayInteractionTextParent, _interactionText, _onEnterConstraints);

            // display input
            string inputName = InputManager.Controls.Player.Jump.name;
            player.InGameUIController.DisplayInput(inputName);
        }
#endregion

        #region On Exit
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnAreaLeft(other);
            }
        }

        protected virtual void OnAreaLeft(Collider other)
        {
            PlayerComponents player = other.GetComponent<PlayerComponents>();

            if (player == null)
            {
                return;
            }

            if (_currentInteractor == player.PlayerInteractions)
            {
                _hasInteractor = false;
                OnInteractorExit?.Invoke();

                StopInteraction();

                RefreshCollider();

                player.InGameUIController.HideInput();
            }
        }
        #endregion

        private void RefreshCollider()
        {
            _collider.enabled = false;
            Invoke(nameof(EnableCollider), Time.deltaTime);
        }

        private void EnableCollider()
        {
            _collider.enabled = true;
        }

        public virtual void StopInteraction()
        {
            if (_currentInteractor != null)
            {
                _currentInteractor.OnInteractionAreaExit(_onEnterConstraints);
                _currentInteractor.GetComponent<PlayerComponents>().InGameUIController.HideInput();
                _currentInteractor = null;
            }
        }

        public virtual void EnableInteraction(bool value)
        {
            _collider.enabled = value;
        }

        public virtual void Deactivate()
        {
            _collider.enabled = false;
        }

        public virtual void Interact(int playerIndex)
        {
            _currentPlayerIndex = playerIndex;
        }

        public AbilityConstraint GetConstraints()
        {
            return _onEnterConstraints;
        }
    }
}
