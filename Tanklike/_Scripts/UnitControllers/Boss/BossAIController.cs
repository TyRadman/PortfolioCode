using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers.States;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public enum BossType
    {
        None = 0,
        ThreeCannon = 1,
    }

    public class BossAIController : MonoBehaviour, IController
    {
        public System.Action<BossAIController> OnBossDeath;

        [Header("Debug")]
        [SerializeField] protected bool _movementDebug;

        [Header("State Machine")]
        [SerializeField] protected BossStateType _initialState;
        [SerializeField] protected BossStateType _activationState;
        [SerializeField] protected List<BossState> _states;

        [Header("Settings")]
        [SerializeField] protected BossType _bossType;
        [SerializeField] protected bool _initialInactiveState;

        protected StateMachine<BossStateType> _stateMachine;
        protected BossComponents _components;
        protected BossHealth _health;
        protected BossMovementController _movementController;
        protected BossAttackController _attackController;

        protected Dictionary<BossStateType, IState> _statesDictionary = new Dictionary<BossStateType, IState>();

        public bool IsActive { get; private set; }

        public void SetUp(BossComponents components)
        {
            _components = components;
            _movementController = (BossMovementController)components.Movement;
            _attackController = (BossAttackController)components.AttackController;
            _health = (BossHealth)components.Health;

            InitStateMachine();

            _stateMachine.SetInitialState(_initialState);

            StartBossIntroduction();
        }

        protected virtual void InitStateMachine()
        {
            //Debug.Log("Init state machine");
            _stateMachine = new StateMachine<BossStateType>();
            _statesDictionary = new Dictionary<BossStateType, IState>();

            foreach (BossState state in _states)
            {
                BossState newState = Instantiate(state);
                newState.SetUp(_stateMachine, _components);
                _statesDictionary.Add(state.BossStateType, newState);
            }

            _stateMachine.Init(_statesDictionary);
        }

        private void Update()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Update();
            }
        }

        public void ActivateBoss()
        {
            Debug.Log("ACTIVATE BOSS");
            _stateMachine.ChangeState(_activationState);
            _components.Movement.Activate();
        }

        public State GetStateByType(BossStateType type)
        {
            return (State)_statesDictionary[type];
        }

        public void StartBossIntroduction()
        {
            _stateMachine.ChangeState(BossStateType.Introduction);
        }

        public void FinishBossIntroduction()
        {
            Activate();
        }

        #region IController
        public void Activate()
        {
            Debug.Log("ACTIVATE");
            IsActive = true;

            _components.Activate();
            Invoke(nameof(ActivateBoss), 0.5f);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            _stateMachine.ChangeState(BossStateType.Death);
            IsActive = false;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
