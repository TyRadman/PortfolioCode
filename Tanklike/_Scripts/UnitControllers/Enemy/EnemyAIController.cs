using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TankLike.UnitControllers.States;

namespace TankLike.UnitControllers
{
    public class EnemyAIController : MonoBehaviour, IController, IPoolable
    {
        public Action<EnemyAIController> OnEnemyDeath { get; set; }

        [Header("Debug")]
        [SerializeField] protected bool _movementDebug;

        [Header("State Machine")]
        [SerializeField] protected EnemyStateType _initialState;
        [SerializeField] protected EnemyStateType _activationState;
        [SerializeField] protected List<State> _states;

        [Header("Settings")]
        [SerializeField] protected EnemyType _enemyType;
        [SerializeField] protected bool _initialInactiveState;
        [SerializeField] protected CollisionEventPublisher _aggroTrigger;

        protected StateMachine<EnemyStateType> _stateMachine;
        protected EnemyMovement _movement;
        protected EnemyShooter _shooter;
        protected EnemyHealth _health;
        protected EnemyTurretController _turretController;

        public Action<IPoolable> OnReleaseToPool { get; private set; }
        protected Dictionary<EnemyStateType, IState> _statesDictionary = new Dictionary<EnemyStateType, IState>();

        public bool IsActive { get; set; }
        public EnemyComponents Components { get; private set; }

        public virtual void SetReferences(EnemyComponents components)
        {
            Components = components;
            _movement = (EnemyMovement)components.Movement;
            _shooter = (EnemyShooter)components.Shooter;
            _health = (EnemyHealth)components.Health;
            _turretController = (EnemyTurretController)components.TurretController;

            _movement.SetDebugMode(_movementDebug);
            
            if (_turretController != null)
            {
                _turretController.SetDebugMode(_movementDebug);
            }
            
            if (!_movementDebug)
            {
                InitStateMachine();
                _stateMachine.SetInitialState(_initialState);

                if (_initialState != EnemyStateType.IDLE)
                    Activate();
            }
            else
            {
                _movement.Activate();
            }
        }

        protected virtual void InitStateMachine()
        {
            _stateMachine = new StateMachine<EnemyStateType>();
            _statesDictionary = new Dictionary<EnemyStateType, IState>();

            foreach (var state in _states)
            {
                var newState = Instantiate(state);
                newState.SetUp(_stateMachine, Components);
                _statesDictionary.Add(state.EnemyStateType, newState);
            }

            _stateMachine.Init(_statesDictionary);
        }

        public State GetStateByType(EnemyStateType type)
        {
            return (State)_statesDictionary[type];
        }

        private void Update()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Update();
            }
        }

        public void ActivateEnemy()
        {
            _stateMachine.ChangeState(_activationState);
            _movement.Activate();
        }

        private void OnPlayerDetected(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Invoke(nameof(ActivateEnemy), 0.5f);
                _aggroTrigger.enabled = false;
            }
        }

        #region IController
        public void Activate()
        {
            Components.Activate();

            if (!_initialInactiveState)
            {
                Invoke(nameof(ActivateEnemy), 0.5f);
            }
            else
            {
                _aggroTrigger.OnTriggerEnterEvent += OnPlayerDetected;
            }
        }

        public void Deactivate()
        {

        }

        public void Restart()
        {
            OnEnemyDeath?.Invoke(this);
            OnReleaseToPool?.Invoke(this);

            if (_initialInactiveState)
            {
                _aggroTrigger.OnTriggerEnterEvent -= OnPlayerDetected;
                _aggroTrigger.enabled = true;
            }
        }

        public void Dispose()
        {
            if(_stateMachine != null)
                _stateMachine.Dispose();
        }
        #endregion

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {
            Components.TankBodyParts.InstantiateBodyParts();
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
