using System;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Environment
{
    public class InteractableArea : MonoBehaviour
    {
        [SerializeField] protected Transform _displayInteractionTextParent;
        [SerializeField] protected string _interactionText;
        protected PlayerInteractions _currentInteractor;
        [SerializeField] protected bool _isActive;
        [SerializeField] private AbilityConstraint _onEnterConstraints;
        [field: SerializeField] public Action OnInteractorExit { get; set; }

        protected virtual void OnTriggerEnter(Collider other)
        {
            // only add the player as an interactor if the interaction area doesn't have an interactor
            if (other.CompareTag("Player") && _currentInteractor == null)
            {
                PlayerComponents player = other.GetComponent<PlayerComponents>();
                // stop the hold action
                player.OnHold.ForceStopHoldAction();
                _currentInteractor = player.PlayerInteractions;
                _currentInteractor.OnInteractionAreaEnter(
                    this, _displayInteractionTextParent, _interactionText, _onEnterConstraints);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_currentInteractor == other.GetComponent<PlayerInteractions>())
                {
                    OnInteractorExit?.Invoke();
                    StopInteraction();
                }
            }
        }

        public virtual void StopInteraction()
        {
            if (_currentInteractor != null)
            {
                _currentInteractor.OnInteractionAreaExit(_onEnterConstraints);
                _currentInteractor = null;
            }


        }

        public virtual void EnableInteraction(bool value)
        {
            GetComponent<Collider>().enabled = value;
        }

        public virtual void Deactivate()
        {
            GetComponent<Collider>().enabled = false;
        }

        public virtual void Interact(int playerIndex)
        {

        }

        public AbilityConstraint GetConstraints()
        {
            return _onEnterConstraints;
        }
    }
}
