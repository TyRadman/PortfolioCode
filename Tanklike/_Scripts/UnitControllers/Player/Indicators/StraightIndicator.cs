using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;

    public class StraightIndicator : BaseIndicator
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _startClip;
        [SerializeField] private AnimationClip _endClip;
        [SerializeField] private Transform _lineBody;
        [SerializeField] private LayerMask _obstructionLayerMask;
        private float _distance = 5f;
        [SerializeField] private float _lerpSpeed = 0.05f;
        [SerializeField] private float _endThreshold = 0.5f;
        [SerializeField] private GameObject _graphics;
        private Vector2 _aimRange;

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);
            _graphics.SetActive(false);
        }

        public void SetUpValues(Vector2 aimRange)
        {
            _aimRange = aimRange;
            _distance = _aimRange.y;
        }

        public override void StartIndicator()
        {
            base.StartIndicator();

            CancelInvoke();

            _crosshair.SetAimRange(_aimRange);
            //_crosshair.EnableCrosshair(false);

            PlayAnimation(_startClip);
            transform.parent = _playerTransform;
            transform.localPosition = Vector3.zero + Vector3.up * Constants.GroundHeight;
            Vector3 position = transform.position;
            position.y = Constants.GroundHeight;
            transform.position = position;
        }

        public override void UpdateIndicator()
        {
            base.UpdateIndicator();

            Vector3 direction = _crosshair.GetCrosshairTransform().position - _playerTransform.position;
            direction.y = 0f;
            transform.forward = direction;

            if (Physics.Raycast(_playerTransform.position, direction, out RaycastHit hit, _distance, _obstructionLayerMask))
            {
                Vector3 scale = transform.localScale;
                scale.z = Mathf.Lerp(scale.z, Vector3.Distance(_playerTransform.position, hit.point) - _endThreshold, _lerpSpeed);
                transform.localScale = scale;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.z = Mathf.Lerp(scale.z, _distance, _lerpSpeed);
                transform.localScale = scale;
            }
        }

        public override void EndIndicator()
        {
            base.EndIndicator();
            PlayAnimation(_endClip);
            Invoke(nameof(SetParentToNull), _endClip.length);

            _crosshair.ResetAimRange();
            //_crosshair.EnableCrosshair(true);
            //_crosshair.ResetCrosshairSpeedMultiplier();
        }

        private void SetParentToNull()
        {
            transform.parent = null;
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
    }
}
