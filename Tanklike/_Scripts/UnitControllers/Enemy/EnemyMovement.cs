using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers
{
    public class EnemyMovement : TankMovement
    {
        public System.Action OnTargetReached;

        [Header("Testing")]
        [SerializeField] private Testing.AIMouseTarget _mouseTarget;
        private bool _debugMode;

        [Header("Navigation")]
        [SerializeField] private float _stoppingDistance = 0.1f;
        [SerializeField] private float _decelerationDistance = 1f;
        [SerializeField] private float _reverseDistance = 6f;
        [SerializeField] private float _obstacleDetectionDistance = 3f;
        [SerializeField] private LayerMask _obstacleLayers;
        [SerializeField] private float _pathUpdateTime = 1f;

        [Header("Sensors")]
        [SerializeField] private float _sideSensorOffset = 1f;

        protected bool _isMoving = false;
        protected bool _isSlowingDown = false;
        protected Vector3 _targetPosition;
        protected Vector3 _targetDirection;
        protected int _currentPathIndex = -1;
        protected float _pathUpdateTimer;
        protected NavMeshPath _path;
        protected List<Vector3> _points = new List<Vector3>();

        //Obstales detection
        private bool _hitFrontObstacle;
        private bool _hitBackObstacle;
        private Vector3 _obstacleNormal;
        private bool _directionIsDetermined;
        private int _movementDirection;
        private bool _faceTargetPoint;

        private bool _frontObstacle;
        private bool _frontRightObstacle;
        private bool _frontLeftObstacle;
        private bool _frontDiagnoalRightObstacle;
        private bool _frontDiagnoalLeftObstacle;

        private bool _wayToTargetBlocked;
        private bool _wayToPointBlocked;

        private float _distanceToTarget;
        private Vector3 _nextPoint;

        private bool _normalMovement = false;
        private bool _obstacleMovement = false;

        public bool TargetIsAvailable { get; private set; }

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if (_mouseTarget != null) _mouseTarget.OnTargetChanged += OnTargetChangedHandler;
        }

        public void SetDebugMode(bool value)
        {
            _debugMode = value;
            if(_mouseTarget != null)
                _mouseTarget.gameObject.SetActive(_debugMode);

            //Debug.Log("MOUSE TARGET -> " + _mouseTarget);
            //Debug.Log("DEBUG MODE -> " + value);
        }

        private void OnTargetChangedHandler()
        {
            if (!_debugMode) return;
            if (_isMoving) return;
            SetTargetPosition(_mouseTarget.transform.position);
        }

        private void Update()
        {
            if (!IsActive)
                return;

            _normalMovement = false;
            _obstacleMovement = false;

            if (_isMoving)
            {
                MoveToTarget();
                UpdatePathToTarget();
                DetectObstacles();
                for (int i = 0; i < _path.corners.Length - 1; i++)
                {
                    _path.corners[i].y = 1f;
                    _path.corners[i+1].y = 1f;

                    Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.blue);
                }
            }

            if(_faceTargetPoint)
            {
                FaceTargetPoint();
            }
            else
            {
                if ((!_hitFrontObstacle && !_frontDiagnoalLeftObstacle && !_frontDiagnoalRightObstacle && _isMoving) || !_isMoving)
                {
                    _normalMovement = true;
                    _obstacleMovement = false;

                    MoveCharacterController(_targetDirection);
                }
                else
                {
                    _normalMovement = false;
                    _obstacleMovement = true;

                    ObstacleMovement();
                }
            }     

            if (_normalMovement && _obstacleMovement)
                Debug.LogError("GOTCHA");
        }

        protected void FaceTargetPoint()
        {
            MoveCharacterController(_targetDirection);
            float dot = Vector3.Dot(_body.forward, _targetDirection);
            
            if (dot >= 0.99)
            {
                _faceTargetPoint = false;
                //Debug.Log("DONE FACING");
            }
        }

        protected void UpdatePathToTarget()
        {
            if(_pathUpdateTimer >= _pathUpdateTime)
            {
                RecalculatePath();
                _pathUpdateTimer = 0f;
            }
            _pathUpdateTimer += Time.deltaTime;
        }

        public void MoveToDestination(Vector3 destination)
        {
            MoveToPoint(_points[_currentPathIndex]);
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            //Debug.Log("TARGET -> " + targetPosition);
            _targetPosition = targetPosition;
            StartMovement();
        }

        public void StartMovement()
        {
            _pathUpdateTimer = 0f;

            _distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

            RecalculatePath();

            TargetIsAvailable = true;
            _directionIsDetermined = false;
            _isMoving = true;
        }

        protected void RecalculatePath()
        {
            _path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, _targetPosition, NavMesh.AllAreas, _path);

            if (_path.corners.Length > 0)
            {
                _currentPathIndex = 0;
                _nextPoint = _path.corners[0];
            }
            _points = new List<Vector3>(_path.corners);
        }

        public float GetPathLength(NavMeshPath path)
        {
            float length = 0f;

            if (path.corners.Length < 2)
            {
                return length;
            }

            for (int i = 1; i < path.corners.Length; i++)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return length;
        }

        public float GetCurrentSpeed()
        {
            return CurrentSpeed;
        }

        protected void MoveToTarget()
        {
            if (!_directionIsDetermined)
                DetermineMovementDirection();

            //temporary
            float distanceToPoint = Vector3.Distance(transform.position, _nextPoint);

            //Debug.Log("DISTANCE -> " + distanceToPoint);
            if (distanceToPoint > _stoppingDistance)
            {
                _targetDirection = _nextPoint - transform.position;
                _targetDirection.y = 0;
                _targetDirection.Normalize();

                float dotToPoint = Vector3.Dot(transform.forward, _targetDirection);
                //Debug.Log("DOT TO POINT -> " + dotToPoint + "DISTANCE -> " + distanceToPoint);
                if(dotToPoint >= -0.1f && dotToPoint <= 0.1f && distanceToPoint <= 2f && _currentPathIndex > 1)
                {
                    _forwardAmount = 0;
                    _turnAmount = 0;
                    _isMoving = false;
                    _components.TankWiggler.WiggleBody(_forwardWiggle);
                    _directionIsDetermined = false;
                    _currentPathIndex = 0;
                    OnTargetReached?.Invoke();
                    TargetIsAvailable = false;
                    return;
                }

                // if the last point is near the wall, consider point reached if the movement type is obstacle movement and the distance is less than the obstacle detection distance
                if (_obstacleMovement && distanceToPoint < _obstacleDetectionDistance && _currentPathIndex > 1)
                {
                    Debug.Log("YO STOP");
                    _forwardAmount = 0;
                    _turnAmount = 0;
                    _isMoving = false;
                    _components.TankWiggler.WiggleBody(_forwardWiggle);
                    _directionIsDetermined = false;
                    _currentPathIndex = 0;
                    OnTargetReached?.Invoke();
                    TargetIsAvailable = false;
                    return;
                }

                if (_hitFrontObstacle)
                {
                    // if the last point is near the wall, consider point reached if the distance is less than the obstacle detection distance
                    if (distanceToPoint < _obstacleDetectionDistance && _currentPathIndex >= _points.Count - 1)
                    {
                        _forwardAmount = 0;
                        _turnAmount = 0;
                        _isMoving = false;
                        _components.TankWiggler.WiggleBody(_forwardWiggle);
                        _directionIsDetermined = false;
                        _currentPathIndex = 0;
                        OnTargetReached?.Invoke();
                        TargetIsAvailable = false;
                        return;
                    }

                    //Debug.Log("OBSTACLE STATE");
                    //if(_wayToPointBlocked)
                    //ObstacleMovement();
                    //Debug.Log("YO 1");

                    if (!_frontObstacle && !_frontLeftObstacle && !_frontRightObstacle)
                    {
                        _forwardAmount = 1;
                        _lastForwardAmount = 1;
                        _turnAmount = 0;
                    }
                    else
                    {
                        if (_frontObstacle && _frontRightObstacle && _frontLeftObstacle)
                        {
                            _turnAmount = 1;
                        }
                        else if(_frontObstacle && _frontLeftObstacle)
                        {
                            _turnAmount = 1;
                        }
                        else if(_frontObstacle && _frontRightObstacle)
                        {
                            _turnAmount = -1;
                        }
                        else if (_frontObstacle)
                        {
                            float hitDot = Vector3.Dot(_body.forward, _obstacleNormal);
                            if (hitDot > 0)
                            {
                                if(!_frontLeftObstacle)
                                    _turnAmount = -1;
                            }
                            else if (hitDot < 0)
                            {
                                if (!_frontRightObstacle)
                                    _turnAmount = 1;
                            }
                        }
                        else if (_frontLeftObstacle && !_frontRightObstacle && !_frontObstacle)
                        {
                            _forwardAmount = 0;
                            _turnAmount = 1;
                        }

                        else if (!_frontLeftObstacle && _frontRightObstacle && !_frontObstacle)
                        {
                            _forwardAmount = 0;
                            _turnAmount = -1;
                        }
                        else if(_frontLeftObstacle && _frontRightObstacle && !_frontObstacle)
                        {
                            _turnAmount = 0;
                            _forwardAmount = 1;
                            _lastForwardAmount = 1;

                        }
                    }
                }
                else
                {
                    //Debug.Log("MOVEMENT STATE");

                    //if (_wayToPointBlocked)
                    //{
                    //    _turnAmount = 0;
                    //    return;
                    //}
                    float dot = Vector3.Dot(_body.forward, _targetDirection);
                    if (dot > 0)
                    {
                        if (_forwardAmount != 1)
                        {
                            _components.TankWiggler.WiggleBody(_backwardWiggle);
                            //_animation.AnimateTurretMotion(1);
                        }

                        _forwardAmount = 1;
                        _lastForwardAmount = 1;
                    }
                    else
                    {
                        //target is behind

                        if (_forwardAmount != 1)
                        {
                            _components.TankWiggler.WiggleBody(_backwardWiggle);
                            //_animation.AnimateTurretMotion(1);
                        }
                        _forwardAmount = 1;
                        _lastForwardAmount = 1;

                        //if (_distanceToTarget > _reverseDistance)
                        //{
                        //    //if it's too far, then reverse
                        //    if (_forwardAmount != 1)
                        //    {
                        //        _animation.AnimateTurretMotion(1);
                        //    }
                        //    _forwardAmount = 1;
                        //    _lastForwardAmount = 1;
                        //}
                        //else
                        //{
                        //    if (_forwardAmount != -1)
                        //    {
                        //        _animation.AnimateTurretMotion(-1);
                        //    }
                        //    _forwardAmount = -1;
                        //    _lastForwardAmount = -1;
                        //}
                    }

                    //if((_frontDiagnoalLeftObstacle || _frontDiagnoalRightObstacle))
                    //{
                    //    _turnAmount = 0;
                    //    ObstacleMovement();
                    //}
                    float targetAngle = Vector3.SignedAngle(_body.forward, _targetDirection * _forwardAmount, Vector3.up);

                    if (targetAngle > 1f)
                    {
                        if (!_frontDiagnoalRightObstacle && _frontDiagnoalLeftObstacle)
                        {
                            _forwardAmount = 1;
                            _turnAmount = 1;
                            //ObstacleMovement();
                            //Debug.Log("YO 2");
                        }
                        else if(_frontDiagnoalLeftObstacle || _frontDiagnoalRightObstacle)
                        {
                            _turnAmount = 0;
                            //ObstacleMovement();
                            //Debug.Log("YO 3");
                        }

                    }
                    else if (targetAngle < -1f)
                    {
                        if (!_frontDiagnoalLeftObstacle && _frontDiagnoalRightObstacle)
                        {
                            _forwardAmount = 1;
                            _turnAmount = -1;
                            //ObstacleMovement();
                            //Debug.Log("YO 4");
                        }
                        else if (_frontDiagnoalLeftObstacle || _frontDiagnoalRightObstacle)
                        {
                            _turnAmount = 0;
                            //ObstacleMovement();
                            //Debug.Log("YO 5");
                        }
                    }
                    else
                    {
                        if ((_frontDiagnoalLeftObstacle || _frontDiagnoalRightObstacle))
                        {
                            _turnAmount = 0;
                            //ObstacleMovement();
                            //Debug.Log("YO 6");
                        }
                    }
                    //Debug.Log(targetAngle);
                    //if (targetAngle > 1f)
                    //{
                    //    if ((!_frontDiagnoalRightObstacle && _lastForwardAmount == 1) || (!_backDiagnoalLeftObstacle && _lastForwardAmount == -1))
                    //    {
                    //        _forwardAmount = 1;
                    //        _turnAmount = 1;
                    //        ObstacleMovement();
                    //    }

                    //}
                    //else if (targetAngle < -1f)
                    //{
                    //    if ((!_frontDiagnoalLeftObstacle && _lastForwardAmount == 1) || (!_backDiagnoalRightObstacle && _lastForwardAmount == -1))
                    //    {
                    //        _forwardAmount = 1;
                    //        _turnAmount = -1;
                    //        ObstacleMovement();
                    //    }
                    //}
                    //else
                    //{
                    //    _turnAmount = 0;
                    //    ObstacleMovement();
                    //}
                }
            }
            else
            {
                if (_currentPathIndex >= _path.corners.Length - 1)
                {
                    _forwardAmount = 0;
                    _turnAmount = 0;
                    _isMoving = false;
                    if (_components.TankWiggler != null)
                        _components.TankWiggler.WiggleBody(_forwardWiggle);
                    _directionIsDetermined = false;
                    _currentPathIndex = 0;
                    OnTargetReached?.Invoke();
                    TargetIsAvailable = false;
                }
                else
                {
                    _currentPathIndex++;
                    _nextPoint = _path.corners[_currentPathIndex];
                }
            }
        }

        private void DetermineMovementDirection()
        {
            if (_points.Count <= 1)
                return;

            //Debug.Log("Determine Derection");

            //if(!_wayToPointBlocked)
            //{
            _faceTargetPoint = true;
            //Debug.Log("FACE DIRECTION");
            //}

            _targetDirection = _points[_currentPathIndex + 1] - transform.position;
            _targetDirection.y = 0;
            _targetDirection.Normalize();

            float dot = Vector3.Dot(transform.forward, _targetDirection);
            if (dot > 0)
            {
                if (_forwardAmount != 1)
                {
                    if (_components.TankWiggler != null)
                        _components.TankWiggler.WiggleBody(_backwardWiggle);
                    //_animation.AnimateTurretMotion(1);
                }

                _forwardAmount = 1;
                _movementDirection = 1;
            }
            else
            {
                //target is behind
                if (_distanceToTarget > _reverseDistance)
                {
                    //if it's too far, then reverse
                    if (_forwardAmount != 1)
                    {
                        if (_components.TankWiggler != null)
                            _components.TankWiggler.WiggleBody(_backwardWiggle);
                        //_animation.AnimateTurretMotion(1);
                    }
                    _forwardAmount = 1;
                    _movementDirection = 1;

                }
                else
                {
                    if (_forwardAmount != 1)
                    {
                        if (_components.TankWiggler != null)
                            _components.TankWiggler.WiggleBody(_backwardWiggle);
                        //_animation.AnimateTurretMotion(1); //was -1
                    }
                    _forwardAmount = 1; //was -1
                    _movementDirection = 1; //was -1

                }
            }

            _directionIsDetermined = true;
        }

        private void DetectObstacles()
        {
            #region Front Detectors

            //front-center
            if (Physics.Raycast(transform.position, _body.forward, out RaycastHit frontHit, _obstacleDetectionDistance, _obstacleLayers))
            {
                _obstacleNormal = frontHit.normal;
                _frontObstacle = true;
                Debug.DrawRay(transform.position, _body.forward * _obstacleDetectionDistance, Color.red);
            }
            else
            {
                
                _frontObstacle = false;
                Debug.DrawRay(transform.position, _body.forward * _obstacleDetectionDistance, Color.green);
            }
            //front-right
            if (Physics.Raycast(transform.position + _body.right * _sideSensorOffset, _body.forward, _obstacleDetectionDistance, _obstacleLayers))
            {
                _frontRightObstacle = true;
                Debug.DrawRay(transform.position + _body.right * _sideSensorOffset, _body.forward * _obstacleDetectionDistance, Color.red);
            }
            else
            {

                _frontRightObstacle = false;
                Debug.DrawRay(transform.position + _body.right * _sideSensorOffset, _body.forward * _obstacleDetectionDistance, Color.green);
            } 
            //front-left
            if (Physics.Raycast(transform.position - _body.right * _sideSensorOffset, _body.forward, _obstacleDetectionDistance, _obstacleLayers))
            {
                _frontLeftObstacle = true;
                Debug.DrawRay(transform.position - _body.right * _sideSensorOffset, _body.forward * _obstacleDetectionDistance, Color.red);
            }
            else
            {

                _frontLeftObstacle = false;
                Debug.DrawRay(transform.position - _body.right * _sideSensorOffset, _body.forward * _obstacleDetectionDistance, Color.green);
            }

            ////DIAGONAL
            //front-diagonal-right
            if (Physics.Raycast(transform.position + _body.right * _sideSensorOffset, (_body.forward + _body.right * 0.5f).normalized, _obstacleDetectionDistance, _obstacleLayers))
            {
                _frontDiagnoalRightObstacle = true;
                Debug.DrawRay(transform.position + _body.right * _sideSensorOffset, (_body.forward + _body.right * 0.5f).normalized * _obstacleDetectionDistance, Color.red);
            }
            else
            {

                _frontDiagnoalRightObstacle = false;
                Debug.DrawRay(transform.position + _body.right * _sideSensorOffset, (_body.forward + _body.right * 0.5f).normalized * _obstacleDetectionDistance, Color.green);
            }

            //front-diagonal-left
            if (Physics.Raycast(transform.position - _body.right * _sideSensorOffset, (_body.forward - _body.right * 0.5f).normalized, _obstacleDetectionDistance, _obstacleLayers))
            {
                _frontDiagnoalLeftObstacle = true;
                Debug.DrawRay(transform.position - _body.right * _sideSensorOffset, (_body.forward - _body.right * 0.5f).normalized * _obstacleDetectionDistance, Color.red);
            }
            else
            {

                _frontDiagnoalLeftObstacle = false;
                Debug.DrawRay(transform.position - _body.right * _sideSensorOffset, (_body.forward - _body.right * 0.5f).normalized * _obstacleDetectionDistance, Color.green);
            }

            #endregion

            bool pointRight = false;
            bool pointLeft = false;
            //point_right
            var targetRightDir = (_nextPoint - (_body.position + _body.right));
            targetRightDir.y = 0.5f;
            targetRightDir.Normalize();
            if (Physics.Raycast(transform.position + _body.right, targetRightDir, out RaycastHit obstacle, _obstacleDetectionDistance, _obstacleLayers)) 
            {
                //Debug.Log("OBSTACLE -> " + obstacle.collider.name);
                //_obstacleNormal = frontHit.normal;
                pointRight = true;
                Debug.DrawRay(transform.position + _body.right, targetRightDir * Vector3.Distance(_body.position + _body.right, _nextPoint), Color.red);
            }
            else
            {

                pointRight = false;
                Debug.DrawRay(transform.position + _body.right, targetRightDir * Vector3.Distance(_body.position + _body.right, _nextPoint), Color.cyan);
            }

            //point_left
            var targetLeftDir = (_nextPoint - (_body.position - _body.right));
            targetLeftDir.y = 0.5f;
            targetLeftDir.Normalize();
            if (Physics.Raycast(transform.position - _body.right, targetLeftDir, _obstacleDetectionDistance, _obstacleLayers))
            {

                pointLeft = true;
                Debug.DrawRay(transform.position - _body.right, targetLeftDir * Vector3.Distance(_body.position - _body.right, _nextPoint), Color.red);
                
                //Debug.Log("OBSTACLE -> " + obstacle.collider.name);
                //_obstacleNormal = frontHit.normal;
               
            }
            else
            {

                pointLeft = false;
                Debug.DrawRay(transform.position - _body.right, targetLeftDir * Vector3.Distance(_body.position - _body.right, _nextPoint), Color.cyan);
            }

            bool targetRight = false;
            bool targetLeft = false;
            //target_right
            var pointRightDir = (_targetPosition - (_body.position + _body.right));
            pointRightDir.y = 0.5f;
            pointRightDir.Normalize();
            if (Physics.Raycast(transform.position + _body.right, pointRightDir, out RaycastHit targetObstacleRight, _obstacleDetectionDistance, _obstacleLayers))
            {
                //Debug.Log("OBSTACLE -> " + targetObstacleRight.collider.name);
                if(targetObstacleRight.collider.transform.root != transform)
                {
                    targetRight = true;
                    Debug.DrawRay(transform.position + _body.right, pointRightDir * Vector3.Distance(_body.position + _body.right, _targetPosition), Color.red);
                }
                //_obstacleNormal = frontHit.normal;

            }
            else
            {
                targetRight = false;
                Debug.DrawRay(transform.position + _body.right, pointRightDir * Vector3.Distance(_body.position + _body.right, _targetPosition), Color.yellow);
            }

            //target_left
            var pointLeftDir = (_targetPosition - (_body.position - _body.right));
            pointLeftDir.y = 0.5f;
            pointLeftDir.Normalize();
            if (Physics.Raycast(transform.position - _body.right, pointLeftDir, out RaycastHit targetObstacleLeft, _obstacleDetectionDistance, _obstacleLayers))
            {
                //Debug.Log("OBSTACLE -> " + obstacle.collider.name);
                //_obstacleNormal = frontHit.normal;
                if (targetObstacleLeft.collider.transform.root != transform)
                {
                    targetLeft = true;
                    Debug.DrawRay(transform.position - _body.right, pointLeftDir * Vector3.Distance(_body.position - _body.right, _targetPosition), Color.red);
                }
            }
            else
            {
                targetLeft = false;
                Debug.DrawRay(transform.position - _body.right, pointLeftDir * Vector3.Distance(_body.position - _body.right, _targetPosition), Color.yellow);
            }

            _wayToTargetBlocked = targetLeft || targetRight;
            _wayToPointBlocked = pointLeft || pointRight;
            if (_forwardAmount == 1)
            {
                if (_frontObstacle || _frontLeftObstacle || _frontRightObstacle)
                {
                    _hitFrontObstacle = true;
                    _hitBackObstacle = false;
                }
                else
                    _hitFrontObstacle = false;
            }
        }

        private void MoveToPoint(Vector3 point)
        {
            Debug.Log("Point");
            _targetPosition = point;
            _targetDirection = _targetPosition - transform.position;
            _targetDirection.y = 0;
            _targetDirection.Normalize();
            _isMoving = true;
            _forwardAmount = 1;
        }

        public override void StopMovement()
        {
            base.StopMovement();
            _isMoving = false;

            if (_targetDirection.magnitude > 0) _isSlowingDown = true;

            if (_components.TankWiggler != null)
                _components.TankWiggler.WiggleBody(_forwardWiggle);
            _forwardAmount = 0;
        }

        public void TurnBody(int turnAmount)
        {
            if (!_canMove) return;

            _body.Rotate(Vector3.up * turnAmount * _turnSpeed * Time.deltaTime);
        }

        public void TurnTurret(int turnAmount)
        {
            if (!_canMove) return;

            _turret.Rotate(Vector3.up * turnAmount * _rotationSpeed * Time.deltaTime);
        }

        public int GetRotationDirection(Transform target)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            float targetAngle = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);

            if (targetAngle > 1f)
            {
                return 1;
            }
            else if (targetAngle < -1f)
            {
                return -1;
            }

            return 0;
        }

        public int GetTurretRotationDirection(Transform target)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            float targetAngle = Vector3.SignedAngle(_turret.forward, dir.normalized, Vector3.up);

            if (targetAngle > 1f)
                return 1;
            else if (targetAngle < -1f)
                return -1;

            return 0;
        }

        public float GetDotToTarget(Transform target)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            return Vector3.Dot(transform.forward, dir.normalized);
        }

        public float GetTurretDotToTarget(Transform target)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            return Vector3.Dot(_turret.forward, dir.normalized);
        }

        public void AimTurretAtTarget(Transform target)
        {
            var rotationDirection = GetTurretRotationDirection(target);

            float dot = GetTurretDotToTarget(target);

            if (dot < 0.999f)
            {
                TurnTurret(rotationDirection);
            }
            else
            {
                TurnTurret(0);
            }
        }

        public void AimBodyAtTarget(Transform target)
        {
            var rotationDirection = GetRotationDirection(target);

            float dot = GetDotToTarget(target);

            if (dot < 0.999f)
            {
                TurnBody(rotationDirection);
            }
            else
            {
                TurnBody(0);
            }
        }

        private void ResetObstacleDetection()
        {
            _hitBackObstacle = false;
            _hitFrontObstacle = false;

            _frontObstacle = false;
            _frontRightObstacle = false;
            _frontLeftObstacle = false;
            _frontDiagnoalRightObstacle = false;
            _frontDiagnoalLeftObstacle = false;

            _wayToTargetBlocked = false;
            _wayToPointBlocked = false;

            _obstacleNormal = Vector3.zero;
        }

        #region IController
        public override void Activate()
        {
            base.Activate();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Restart()
        {
            base.Restart();

            _targetPosition = Vector3.zero;
            _targetDirection = Vector3.zero;
            _currentPathIndex = 0;
            _points.Clear();

            ResetObstacleDetection();

            _directionIsDetermined = false;
            _movementDirection = 0;
            _distanceToTarget = 0;
            _nextPoint = Vector3.zero;
        }
        #endregion
    }
}
