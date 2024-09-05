using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    public class AirIndicator : BaseIndicator, IPoolable
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _startAnimationClip;
        [SerializeField] private AnimationClip _finishAnimationClip;
        public Action<IPoolable> OnReleaseToPool { get; private set; }
        private Vector2 _indicatorAimRadiusRange;
        private float _indicatorSpeedMultiplier = 0.5f;
        [SerializeField] private Transform _indicatorEnd;
        [Header("Curve")]
        [SerializeField] private LineRenderer _line;
        [SerializeField] private float _height;
        [SerializeField] private int _levelOfDetail = 10;
        [SerializeField] private AnimationCurve _curve;

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);
        }

        public void SetUpValues(float radius, Vector2 aimRange, float aimSpeed)
        {
            _indicatorEnd.transform.localScale = new Vector3(radius * 2, 1f, radius * 2);
            _indicatorAimRadiusRange = aimRange;
            _indicatorSpeedMultiplier = aimSpeed;
        }

        public override void StartIndicator()
        {
            base.StartIndicator();

            CancelInvoke();
            
            _indicatorEnd.parent = _crosshair.GetCrosshairTransform();
            _indicatorEnd.localPosition = Vector3.zero;
            Vector3 position = _indicatorEnd.position;
            position.y = Constants.GroundHeight;
            _indicatorEnd.position = position;

            PlayAnimation(_startAnimationClip);

            _crosshair.EnableCrosshair(false);
            _crosshair.SetAimRange(_indicatorAimRadiusRange);
            _crosshair.SetCrosshairSpeedMultiplier(_indicatorSpeedMultiplier);
            _crosshair.EnableCrosshair(false);
            _line.enabled = true;
        }

        public override void UpdateIndicator()
        {
            base.UpdateIndicator();

            UpdateCurve();
        }

        private void UpdateCurve()
        {
            _line.positionCount = _levelOfDetail;
            Vector3 startPoint = _indicatorEnd.position;
            Vector3 endPoint = _player.transform.position;

            for (int i = 0; i < _levelOfDetail; i++)
            {
                float t = (float)i / ((float)_levelOfDetail - 1f);
                float x = Mathf.Lerp(startPoint.x, endPoint.x, t);
                float z = Mathf.Lerp(startPoint.z, endPoint.z, t);
                Vector3 position = new Vector3(x, _curve.Evaluate(t) * _height, z);
                _line.SetPosition(i, position);
            }
        }

        public override void EndIndicator()
        {
            base.EndIndicator();
            PlayAnimation(_finishAnimationClip);
            _indicatorEnd.transform.parent = null;
            _crosshair.ResetAimRange();
            _crosshair.EnableCrosshair(true);
            _crosshair.ResetCrosshairSpeedMultiplier();
            _line.enabled = false;
        }

        private void PlayAnimation(AnimationClip clip)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            _animation.clip = clip;
            _animation.Play();
        }

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

        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            GameManager.Instance.SetParentToSpawnables(gameObject);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
