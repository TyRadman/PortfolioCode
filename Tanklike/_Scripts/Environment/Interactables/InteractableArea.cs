using System;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Environment
{
    [RequireComponent(typeof(Collider))]
    public class InteractableArea : MonoBehaviour
    {
        [SerializeField] protected Transform _displayInteractionTextParent;
        [SerializeField] protected string _interactionText;
        protected PlayerInteractions _currentInteractor;
        [SerializeField] protected bool _isActive;
        [SerializeField] private AbilityConstraint _onEnterConstraints;
        [field: SerializeField] public Action OnInteractorExit { get; set; }
        private Collider _collider;
        protected bool _hasInteractor = false;
        protected int _currentPlayerIndex;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public virtual void SetUp()
        {

        }

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

            _hasInteractor = true;
            PlayerComponents player = other.GetComponent<PlayerComponents>();

            if(player == null)
            {
                Debug.LogError($"No player component at {gameObject.name}");
                return;
            }

            // stop the hold action
            player.OnHold.ForceStopHoldAction();
            _currentInteractor = player.PlayerInteractions;
            _currentInteractor.OnInteractionAreaEnter(
                this, _displayInteractionTextParent, _interactionText, _onEnterConstraints);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnAreaLeft(other);
            }
        }

        protected virtual void OnAreaLeft(Collider other)
        {
            if (_currentInteractor == other.GetComponent<PlayerInteractions>())
            {
                _hasInteractor = false;
                OnInteractorExit?.Invoke();

                StopInteraction();

                RefreshCollider();
            }
        }

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
            _currentPlayerIndex = playerIndex;
        }

        public AbilityConstraint GetConstraints()
        {
            return _onEnterConstraints;
        }
    }
}
