using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankTurretController : MonoBehaviour, IController
    {
        [Header("Turret")]
        [SerializeField] protected float _rotationSpeed = 50f;

        protected const float ROTATION_CORRECTION_THRESHOLD = 0.1f;

        protected bool _canRotate = true;

        protected Transform _turret;
        protected Transform Body;

        public bool IsActive { get; protected set; }

        public void SetUp(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
        }

        public virtual void HandleTurretRotation(Transform crosshair)
        {
            Vector3 direction = (crosshair.position - _turret.position).normalized;
            Vector3 tankForward = _turret.forward.normalized;
            Vector3 crossDot = Vector3.Cross(tankForward, direction);
            float rotationAmount;
            float cross = crossDot.y;

            if (cross > ROTATION_CORRECTION_THRESHOLD)
            {
                rotationAmount = 1f;
            }
            else if (cross < -ROTATION_CORRECTION_THRESHOLD)
            {
                rotationAmount = -1f;
            }
            else
            {
                if (cross > 0)
                {
                    rotationAmount = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(ROTATION_CORRECTION_THRESHOLD, 0f, cross));
                }
                else if (cross < 0)
                {
                    rotationAmount = Mathf.Lerp(-1f, 0f, Mathf.InverseLerp(-ROTATION_CORRECTION_THRESHOLD, 0f, cross));
                }
                else
                {
                    rotationAmount = 0f;
                }
            }

            _turret.Rotate(_rotationSpeed * rotationAmount * Time.deltaTime * Vector3.up);
        }

        protected void RotateTank(float rotationSpeed)
        {
            if (!_canRotate)
            {
                return;
            }

            transform.RotateAround(transform.position, transform.up, Time.deltaTime * rotationSpeed);
        }

        protected void RotateTurret(float rotationSpeed)
        {
            if (!_canRotate)
            {
                return;
            }

            _turret.RotateAround(transform.position, transform.up, Time.deltaTime * rotationSpeed);
        }

        public void EnableRotation(bool canRotate)
        {
            _canRotate = canRotate;
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            IsActive = false;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
