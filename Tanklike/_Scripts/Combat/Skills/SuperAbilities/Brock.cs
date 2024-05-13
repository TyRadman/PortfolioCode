using TankLike.UnitControllers;
using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = PREFIX + "Brock", menuName = Directories.ABILITIES + "Brock Ability")]
    public class Brock : Ability
    {
        [Header("Special Values")]
        [SerializeField] private int _damagePerBullet;
        [SerializeField] private AbilityConstraint _postHoldConstraints;
        [SerializeField] private AmmunationData _bulletData;
        [SerializeField] private int _bulletsNumber = 3;
        [SerializeField] private float _verticalDistance = 15f;
        [SerializeField] private float _horizontalMaxOffset = 2f;
        [SerializeField] private float _maxForwardDitance = 10f;
        [SerializeField] private float _radius;
        private float _timeBetweenShots;
        [SerializeField] private float _startingPoint = 2.5f;
        private TankComponents _tankComponents;
        [SerializeField] private List<AnimationCurve> _verticalCurves;
        [SerializeField] private List<AnimationCurve> _horizontalCurves;
        [SerializeField] private AnimationCurve _forwardCurve;
        [Header("Durations")]
        [SerializeField] private float _airMovementDuration = 0.3f;
        [SerializeField] private float _landingDuration = 0.3f;
        [SerializeField] private float _inAirDelayDuration = 0.3f;
        [Header("Indicator")]
        [SerializeField] private Indicator _indicatorPrefab;
        [SerializeField] private IndicatorCurve _indicatorCurvePrefab;
        private IndicatorCurve _indicatorCurve;
        [SerializeField] private float _indicatorSpeedMultiplier = 0.5f;
        [SerializeField] private Vector3 _indicatorAimOffset;
        private Indicator _indicator;
        protected Pool<Bullet> _bulletPool;
        private Transform[] _shootingPoints;
        private PlayerCrosshairController _crosshair;
        [SerializeField] private Vector2 _indicatorAimRadiusRange;

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);

            // create the indicator
            _indicator = Instantiate(_indicatorPrefab);
            _indicatorCurve = Instantiate(_indicatorCurvePrefab);
            _indicatorCurve.EnableLine(false);
            SetUpIndicator();
            _tankComponents = tankTransform.GetComponent<TankComponents>();
            _crosshair = ((PlayerComponents)_tankComponents).Crosshair;
            _timeBetweenShots = ((_duration - _airMovementDuration - _landingDuration - _inAirDelayDuration) / _bulletsNumber) / 2;

            // get shooting points. If there are brock shooting points in the additional tank info class, then set them as shooting points, otherwise, set the main shooting point
            if (_tankComponents.AdditionalInfo.BrockShootingPoints.Length > 0)
            {
                _shootingPoints = _tankComponents.AdditionalInfo.BrockShootingPoints;
            }
            else
            {
                _shootingPoints = _tankComponents.Shooter.ShootingPoints.ToArray(); //need to be tested
            }
        }

        public override void OnActivateAbility()
        {
            base.OnActivateAbility();

            GameManager.Instance.StartCoroutine(BrockAbilityRoutine());
        }

        private IEnumerator BrockAbilityRoutine()
        {
            _tankComponents.Constraints.ApplyConstraints(false, Constraints);
            _tankComponents.Constraints.ApplyConstraints(true, _postHoldConstraints);
            // reset indicator stuff
            _indicator.transform.parent = null;
            _crosshair.ResetAimRange();
            _crosshair.EnableCrosshair(true);
            _crosshair.ResetCrosshairSpeedMultiplier();
            List<Bullet> bullets = new List<Bullet>();
            _indicatorCurve.EnableLine(false);
            _indicator.OnIndicatorFinished();

            for (int i = 0; i < _bulletsNumber; i++)
            {
                // create the bullet
                Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(_bulletData.GUID);
                bullet.SetValues(0f, _damagePerBullet, 0f);
                bullet.transform.SetPositionAndRotation(_shootingPoints[i % _shootingPoints.Length].position, _shootingPoints[i % _shootingPoints.Length].rotation);
                bullet.gameObject.SetActive(true);
                bullets.Add(bullet);
                Vector2 randomCircle = _radius * 0.5f * Random.insideUnitCircle;
                Vector3 point = _indicator.transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);// - _verticalDistance * 1.1f * Vector3.up; ;
                // enable the bullet's collider, mesh, and trail
                bullet.EnableBullet();
                bullet.SetActive(true);

                //bullet.MoveToPoint(point, false, _airMovementDuration);
                GameManager.Instance.StartCoroutine(MoveBulletToPoint(bullet.transform, point, _airMovementDuration)); ;

                // set reference to the bullet data
                bullet.SetUpBulletdata(_bulletData);
                // give the bullet a target tag
                bullet.SetTargetLayerMask(Helper.GetOpposingTag(_tankComponents.gameObject.tag));
                // assign the shooter of the bullet
                bullet.SetShooter(_tankComponents);

                yield return new WaitForSeconds(_timeBetweenShots);
            }

            ResetAbility();
            yield return new WaitForSeconds(_airMovementDuration);
            OnAbilityFinished();
        }

        private IEnumerator MoveBulletToPoint(Transform bullet, Vector3 point, float duration)
        {
            Vector3 startPos = bullet.position;
            float time = 0f;
            // catch the direction of the bullet's movement
            Quaternion targetRotation = Quaternion.LookRotation(point - bullet.position);
            AnimationCurve verticalCurve = _verticalCurves.RandomItem();
            AnimationCurve horizontalCurve = _horizontalCurves.RandomItem();

            // Smoothly interpolate the rotation over time.
            while (time < duration && bullet.gameObject.activeSelf)
            {
                float t = time / duration;
                Vector3 newPosition = Vector3.Lerp(startPos, point, _forwardCurve.Evaluate(t));
                newPosition += _horizontalMaxOffset * horizontalCurve.Evaluate(t) * bullet.right;
                newPosition += _verticalDistance * verticalCurve.Evaluate(t) * bullet.up;
                bullet.position = newPosition;
                time += Time.deltaTime;
                // interpolate the bullet's direction towards its target
                bullet.rotation = Quaternion.Slerp(bullet.rotation, targetRotation, Bullet.ROTATION_SPEED * Time.deltaTime);
                yield return null;
            }

            time = 0f;

            // to ensure it lands on the ground
            while (time < 5f)
            {
                bullet.position -= Vector3.up * Time.deltaTime * 10f;
                time += Time.deltaTime;
                yield return null;
            }
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
        }

        #region Hold Down Methods
        public override void OnAbilityHoldStart()
        {
            base.OnAbilityHoldStart();

            _tankComponents.Constraints.ApplyConstraints(true, Constraints);

            _crosshair.EnableCrosshair(false);
            
            _indicatorCurve.EnableLine(true);
            _indicator.gameObject.SetActive(true);
            _indicator.OnIndicatorStarted();
            _indicator.transform.parent = _crosshair.GetCrosshair();
            _indicator.transform.localPosition = Vector3.zero + _indicatorAimOffset;
            
            _crosshair.SetAimRange(_indicatorAimRadiusRange);
            _crosshair.SetCrosshairSpeedMultiplier(_indicatorSpeedMultiplier);
        }

        public override void OnAbilityFinished()
        {
            base.OnAbilityFinished(); 
        }

        public override void OnAbilityHoldUpdate()
        {
            base.OnAbilityHoldUpdate();
            _indicatorCurve.UpdateCurve(_indicator.transform.position, _tankComponents.transform.position);
        }

        public override void OnAbilityCancelled()
        {
            base.OnAbilityCancelled();
            //ResetAbility();
            _indicator.OnIndicatorFinished();

            // reset indicator stuff
            _indicator.transform.parent = null;
            _crosshair.ResetAimRange();
            _crosshair.EnableCrosshair(true);
            _crosshair.ResetCrosshairSpeedMultiplier();
            _indicatorCurve.EnableLine(false);

            _tankComponents.Constraints.ApplyConstraints(false, Constraints);

        }
        #endregion

        private void ResetAbility()
        {
            _tankComponents.Constraints.ApplyConstraints(false, _postHoldConstraints);
        }

        private void SetUpIndicator()
        {
            _indicator.gameObject.SetActive(false);
            _indicator.transform.localScale = new Vector3(_radius, 1f, _radius);
        }
    }
}
