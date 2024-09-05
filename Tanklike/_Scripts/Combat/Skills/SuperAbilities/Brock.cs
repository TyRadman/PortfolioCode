using TankLike.UnitControllers;
using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    [CreateAssetMenu(fileName = PREFIX + "Brock", menuName = Directories.ABILITIES + "Brock Ability")]
    public class Brock : Ability
    {
        [Header("Special Values")]
        [SerializeField] private List<AnimationCurve> _verticalCurves;
        [SerializeField] private List<AnimationCurve> _horizontalCurves;
        [SerializeField] private AmmunationData _bulletData;
        [SerializeField] private AnimationCurve _forwardCurve;
        [SerializeField] private float _verticalDistance = 15f;
        [SerializeField] private float _horizontalMaxOffset = 2f;
        [SerializeField] private float _radius;
        [SerializeField] private int _damagePerBullet;
        [SerializeField] private int _bulletsNumber = 3;
        [Header("Durations")]
        [SerializeField] private Vector2 _aimRange;
        [SerializeField] private float _airMovementDuration = 0.3f;
        [SerializeField] private float _landingDuration = 0.3f;
        [SerializeField] private float _inAirDelayDuration = 0.3f;
        [Tooltip("Speed multiplier. How fast should the cursor move in the super aim state in comparison to its normal speed.")]
        [SerializeField] private float _aimSpeed = 0.5f;
        private float _timeBetweenShots;
        private TankComponents _tankComponents;
        protected Pool<Bullet> _bulletPool;
        private Transform[] _shootingPoints;
        private PlayerCrosshairController _crosshair;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _tankComponents = components;
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

        public override void SetUpIndicatorSpecialValues(BaseIndicator indicator)
        {
            base.SetUpIndicatorSpecialValues(indicator);

            if (indicator is not AirIndicator)
            {
                Debug.LogError($"Ability {this.name} is expecting indicator of type {nameof(AirIndicator)}. Wrong indicator.");
                Debug.Break();
                return;
            }

            AirIndicator airIndicator = (AirIndicator)indicator;
            airIndicator.SetUpValues(_radius, _aimRange, _aimSpeed);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            GameManager.Instance.StartCoroutine(BrockAbilityRoutine());
        }

        private IEnumerator BrockAbilityRoutine()
        {
            List<Bullet> bullets = new List<Bullet>();
            Vector3 crosshairPosition = _crosshair.GetCrosshairTransform().position;

            for (int i = 0; i < _bulletsNumber; i++)
            {
                // create the bullet
                Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(_bulletData.GUID);
                bullet.SetValues(0f, _damagePerBullet, 0f);
                bullet.transform.SetPositionAndRotation(_shootingPoints[i % _shootingPoints.Length].position, _shootingPoints[i % _shootingPoints.Length].rotation);
                bullet.gameObject.SetActive(true);
                bullets.Add(bullet);
                Vector2 randomCircle = _radius * Random.insideUnitCircle;
                Vector3 point = crosshairPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
                // enable the bullet's collider, mesh, and trail
                bullet.EnableBullet();
                bullet.SetActive(true);

                GameManager.Instance.StartCoroutine(MoveBulletToPoint(bullet.transform, point, _airMovementDuration)); ;

                // set reference to the bullet data
                bullet.SetUpBulletdata(_bulletData);
                // give the bullet a target tag
                bullet.SetTargetLayerMask(Helper.GetOpposingTag(_tankComponents.gameObject.tag));
                // assign the shooter of the bullet
                bullet.SetShooter(_tankComponents);

                yield return new WaitForSeconds(_timeBetweenShots);
            }

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
    }
}
