using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class MainCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private float _height = 0;
        private bool _interpolatePosition = true;
        private List<FollowTarget> _followPoints = new List<FollowTarget>();
        //private List<FollowTarget> _backUpFollowPoints = new List<FollowTarget>();
        private int _playersCount;
        [SerializeField][Range(0f, 20f)] private float _mainTargetFollowSpeed = 4f;
        [SerializeField] private float _totalSpeedMultiplier = 1f;
        [SerializeField] private CameraLimits _originalLimits;
        [SerializeField] private CameraLimits _currentLimits;
        [SerializeField] private CameraLimits _offset;
        private const float SPEED_TRANSITION_DURATION = 1f;
        [SerializeField] private AnimationCurve _moveToTargetCurve;
        private bool _followPlayers = false;

        [System.Serializable]
        public class FollowTarget
        {
            public float SpeedMultiplier = 1f;
            public Transform CrossHair;
            public Transform Target;
            public int Index = -1;
            public bool IsActive = false;
        }

        public void SetUp()
        {
            if (!this.enabled)
            {
                return;
            }

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                AddCameraFollowTarget(i);
            }

            // set the initial position of the camera
            Vector3 startPosition = Vector3.zero;
            _followPoints.ForEach(f => startPosition += f.Target.position);
            _target.position = startPosition / _followPoints.Count;
            _followPlayers = true;
        }

        private void Update()
        {
            _followPoints.ForEach(f => UpdateFollowPointPosition(f));

            if (_playersCount < 1 || !_followPlayers)
            {
                return;
            }

            FollowMainTarget();
        }

        private void AddCameraFollowTarget(int playerIndex)
        {
            if (_followPoints.Exists(f => f.Index == playerIndex))
            {
                _followPoints.Find(f => f.Index == playerIndex);
                return;
            }

            Transform target = new GameObject($"Player{playerIndex}CameraTarget").transform;

            FollowTarget newFollow = new FollowTarget()
            {
                CrossHair = GameManager.Instance.PlayersManager.GetPlayer(playerIndex).Crosshair.GetCrosshairTransform(),
                Target = target,
                Index = playerIndex,
                IsActive = true
            };

            newFollow.Target.position = GameManager.Instance.PlayersManager.GetPlayer(playerIndex).transform.position;
            //_backUpFollowPoints.Add(newFollow);
            _followPoints.Add(newFollow);
            _playersCount = _followPoints.Count;
        }

        private void UpdateFollowPointPosition(FollowTarget target)
        {
            if(!target.IsActive)
            {
                return;
            }
            
            Vector3 newPosition;

            if (_interpolatePosition)
            {
                float lerpSpeed = _mainTargetFollowSpeed;
                _followPoints.ForEach(p => lerpSpeed *= p.SpeedMultiplier);
                lerpSpeed /= _playersCount;
                lerpSpeed *= Time.deltaTime;
                newPosition = Vector3.Lerp(target.Target.position, target.CrossHair.position, lerpSpeed);
            }
            else
            {
                newPosition = target.CrossHair.position;
            }

            newPosition.x = Mathf.Clamp(newPosition.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
            newPosition.z = Mathf.Clamp(newPosition.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);
            newPosition.y = _height;

            target.Target.position = newPosition;
        }

        private void FollowMainTarget()
        {
            List<FollowTarget> activeFollowees = _followPoints.FindAll(p => p.IsActive);
            Vector3 position = Vector3.zero;
            activeFollowees.FindAll(f => f.IsActive).ForEach(f => position += f.Target.position);
            position /= _playersCount;

            if (_interpolatePosition)
            {
                float lerpSpeed = _mainTargetFollowSpeed;
                activeFollowees.ForEach(p => lerpSpeed *= p.SpeedMultiplier);
                lerpSpeed *= Time.deltaTime;
                position = Vector3.Lerp(_target.position, position, lerpSpeed);
            }

            _target.position = position;
        }

        public void SetSpeedMultiplier(float multiplier, int targetIndex)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeSpeedMultiplierValueProcess(1f + multiplier * _totalSpeedMultiplier, targetIndex));
        }

        private IEnumerator ChangeSpeedMultiplierValueProcess(float endValue, int index)
        {
            float timer = 0f;
            float startValue = _followPoints[index].SpeedMultiplier;

            while (timer < SPEED_TRANSITION_DURATION)
            {
                timer += Time.deltaTime;
                _followPoints[index].SpeedMultiplier = Mathf.Lerp(startValue, endValue, timer / SPEED_TRANSITION_DURATION);
                yield return null;
            }
        }

        public void ResetSpeedMultiplier(int targetIndex)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeSpeedMultiplierValueProcess(1f, targetIndex));
        }

        public void SetLimits(CameraLimits limits)
        {
            _originalLimits.SetValues(limits);
            _currentLimits.ScaleUpValues(_originalLimits, _offset, 1f);
        }

        // Used for test scenes
        public void ResetLimits()
        {
            _currentLimits.SetValues(_originalLimits);
        }

        public void EnableInterpolation(bool enable)
        {
            _interpolatePosition = enable;
        }

        /// <summary>
        /// Removes the player with the provided index from the list of players getting followed by the camera.
        /// </summary>
        /// <param name="playerIndex"></param>
        public void RemoveCameraFollow(int playerIndex)
        {
            _followPoints.Find(f => f.Index == playerIndex).IsActive = false;
            _playersCount = _followPoints.FindAll(f => f.IsActive).Count;
        }

        /// <summary>
        /// Adds the player with the provided index from the list of players getting followed by the camera.
        /// </summary>
        /// <param name="playerIndex"></param>
        public void AddCameraFollower(int playerIndex)
        {
            //_followPoints.Add(_backUpFollowPoints.Find(f => f.Index == playerIndex));
            _followPoints.Find(f => f.Index == playerIndex).IsActive = true;
            _playersCount = _followPoints.FindAll(f => f.IsActive).Count;
        }

        public void SetOffsetMultiplier(float value)
        {
            _currentLimits.ScaleUpValues(_originalLimits, _offset, value);
        }

        public void MoveToPoint(Transform point, float duration)
        {
            StartCoroutine(MoveToPointProcess(point, duration));
        }

        private IEnumerator MoveToPointProcess(Transform point, float duration)
        {
            float timeElapsed = 0f;
            // stop the players following process
            _followPlayers = false;
            // cache the start point
            Vector3 startPoint = _target.position;

            startPoint.x = Mathf.Clamp(startPoint.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
            startPoint.z = Mathf.Clamp(startPoint.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);

            while (timeElapsed <= duration)
            {
                timeElapsed += Time.deltaTime;
                float t = _moveToTargetCurve.Evaluate(timeElapsed / duration);

                Vector3 pointPosition = point.position;
                pointPosition.x = Mathf.Clamp(pointPosition.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
                pointPosition.z = Mathf.Clamp(pointPosition.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);
                point.position = pointPosition;
                
                Vector3 newPosition = Vector3.Lerp(startPoint, pointPosition, t);
                
                _target.position = newPosition;
                yield return null;
            }
        }

        public void MoveBackToPlayers(float duration)
        {
            StartCoroutine(MoveToPlayersProcess(duration));
        }

        private IEnumerator MoveToPlayersProcess(float duration)
        {
            float timeElapsed = 0f;
            _followPlayers = false;
            Vector3 startPoint = _target.position;
            List<Transform> players = GameManager.Instance.PlayersManager.GetPlayers();

            while (timeElapsed <= duration)
            {
                timeElapsed += Time.deltaTime;
                float t = _moveToTargetCurve.Evaluate(timeElapsed / duration);

                Vector3 pointPosition = Vector3.zero;

                players.ForEach(p => pointPosition += p.position);
                pointPosition /= 2;

                pointPosition.x = Mathf.Clamp(pointPosition.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
                pointPosition.z = Mathf.Clamp(pointPosition.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);

                Vector3 newPosition = Vector3.Lerp(startPoint, pointPosition, t);

                _target.position = newPosition;
                yield return null;
            }
        }

        public void EnableFollowingPlayer()
        {
            _followPlayers = true;
        }

        public void StopCameraFollowProcess()
        {
            _followPlayers = false;
            StopAllCoroutines();
        }
    }
}
