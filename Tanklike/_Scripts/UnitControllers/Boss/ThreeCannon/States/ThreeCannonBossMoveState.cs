using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_Move", menuName = MENU_PATH + "Three Cannon/Move State")]
    public class ThreeCannonBossMoveState : BossMoveState
    {
        [SerializeField] private Vector2 _movementDistanceRange;
        [SerializeField, Range(0f, 1f)] private float _moveToTargetChance = 0.5f;
        private Vector3 _targetPoint;
        private bool _targetPointFound;
        private int _pointFinderCounter;
        private Vector3 _mapCenter;
        private float _walkableAreaRadius;

        private float _groundPoundCheckTime = 0.5f;
        private float _groundPoundCheckTimer;
        private bool _movementStarted;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);
            _movement.OnTargetReached += OnTargetReachedHandler;
            _attackController.OnGroundPoundTriggered += OnGroundPoundTriggeredHandler;

            _mapCenter = bossComponents.RoomCenter;
            _walkableAreaRadius = _movement.WalkableAreaRadius;
        }

        public override void OnEnter()
        {
            Debug.Log("MOVE STATE");
            _isActive = true;

            _attackController.EnableGroundPoundTrigger(true);

            _groundPoundCheckTimer = 0f;
            _movementStarted = false;
        }

        public override void OnUpdate()
        {
            if (_movementStarted)
            {
                return;
            }

            if(_groundPoundCheckTimer >= _groundPoundCheckTime)
            {
                _movementStarted = true;
                _movement.StartMoveAnimation(true);

                // If there is already an active target that was not reached, don't pick a new random point
                if (_movement.TargetIsAvailable)
                {
                    _movement.StartMovement();
                    return;
                }

                _pointFinderCounter = 0;
                _targetPointFound = false;

                while (!_targetPointFound && _pointFinderCounter < 50)
                {
                    // Get the chance to move towards the target player
                    float chance = Random.Range(0, 1f);
                    Vector3 point = Vector3.zero;

                    if (chance <= _moveToTargetChance)
                    {
                        // Get target player as target point
                        var player = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
                        Vector3 dir = player.position - _movement.transform.position;
                        dir.Normalize();

                        float dist = Vector3.Distance(player.position, _movement.transform.position);

                        if (dist > _movementDistanceRange.y)
                        {
                            dist = _movementDistanceRange.y;
                        }

                        point = _movement.transform.position + dir * dist;
                    }
                    else
                    {
                        // Get random point and move to it
                        float angle = Random.Range(0.0f, Mathf.PI * 2);
                        Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                        dir *= Random.Range(_movementDistanceRange.x, _movementDistanceRange.y);
                        dir.y = 0.5f;
                        point = _movement.transform.position + dir;
                    }

                    if (Helper.IsPointInsideSphere(point, _mapCenter, _walkableAreaRadius))
                    {
                        MoveToTarget(point);
                    }

                    _pointFinderCounter++;
                }

                if (!_targetPointFound)
                {
                    _stateMachine.ChangeState(BossStateType.Attack);
                }
            }
            else
            {
                _groundPoundCheckTimer += Time.deltaTime;
            }
        }

        public override void OnExit()
        {
            _isActive = false;
            _movement.StartMoveAnimation(false);
        }

        public override void OnDispose()
        {
            _movement.OnTargetReached -= OnTargetReachedHandler;
            _attackController.OnGroundPoundTriggered -= OnGroundPoundTriggeredHandler;
        }

        private bool MoveToTarget(Vector3 target)
        {
            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(target, out hit, 10.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                _targetPoint = hit.position;
                _targetPointFound = true;
                _movement.SetTargetPosition(_targetPoint);
                return true;
            }

            return false;
        }

        private void OnTargetReachedHandler()
        {
            if (!_isActive)
            {
                return;
            }

            _stateMachine.ChangeState(BossStateType.Attack);
        }

        private void OnGroundPoundTriggeredHandler()
        {
            if (!_isActive)
            {
                return;
            }

            _movement.StopMovement();
            _stateMachine.ChangeState(BossStateType.GroundPound);
        }
    }
}
