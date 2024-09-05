using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankWiggler : MonoBehaviour, IController
    {
        [Header("Body References")]
        [SerializeField] private Transform _tankHolder;
        [Header("Turret References")]
        [SerializeField] private Transform _turretRotationReference;
        private const float SWITCH_BETWEEN_WIGGLES_TIME = 0.1f;
        
        private bool _isWiggling = false;
        private Transform _body;
        private Transform _turret;
        private Quaternion _defaultRotation;

        public bool IsActive { get; private set; }

        public void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            _body = parts.GetBodyPartOfType(BodyPartType.Body).transform;
            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;

            _defaultRotation = _tankHolder.rotation;
        }

        public void WiggleBody(Wiggle wiggle)
        {
            if (!_isWiggling)
            {
                StartCoroutine(WigglingProcess(wiggle, _tankHolder, _body));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(SwitchWigglesProcess(wiggle, _tankHolder, _body));
            }
        }

        public void WiggleTurret(Wiggle wiggle)
        {
            if (!_isWiggling)
            {
                StartCoroutine(WigglingProcess(wiggle, _turret, _turretRotationReference));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(SwitchWigglesProcess(wiggle, _turret, _turretRotationReference));
            }
        }

        private IEnumerator WigglingProcess(Wiggle wiggle, Transform bodyToWiggle, Transform referenceBody)
        {
            _isWiggling = true;
            float time = 0f;
            Quaternion initialRotation = bodyToWiggle.rotation;
            AnimationCurve curve = wiggle.Curve;
            float direction = wiggle.Backward ? -1 : 1;
            float duration = wiggle.Duration;
            float angle = wiggle.MaxAngle;

            while (time < duration)
            {
                time += Time.deltaTime;
                float curveValue = curve.Evaluate(time / duration) * direction;
                float rotationAngle = curveValue * angle;
                // gets the right rotation of the body, rotates it (hypothetically) by the given angle, and returns the new rotation
                Quaternion rotationDelta = Quaternion.AngleAxis(rotationAngle, referenceBody.right);
                bodyToWiggle.rotation = initialRotation * rotationDelta;
                //print($"initial: {initialRotation.eulerAngles}. reference: {referenceBody.eulerAngles}");
                yield return null;
            }

            //_tankHolder.rotation = Quaternion.AngleAxis(0f, _tankBody.right);
            _isWiggling = false;
        }

        private IEnumerator SwitchWigglesProcess(Wiggle wiggle, Transform bodyToWiggle, Transform referenceBody)
        {
            float time = 0f;

            while(time < SWITCH_BETWEEN_WIGGLES_TIME)
            {
                time += Time.deltaTime;
                bodyToWiggle.rotation = Quaternion.Lerp(bodyToWiggle.rotation, _defaultRotation, time / SWITCH_BETWEEN_WIGGLES_TIME);
                yield return null;
            }

            StartCoroutine(WigglingProcess(wiggle, bodyToWiggle, referenceBody));
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
