using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Misc;
using TankLike.Utils;
using UnityEngine;
using static TankLike.PlayersManager;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_RocketLauncherAttack", menuName = MENU_PATH + "Three Cannon/Rocket Launcher Attack State")]
    public class ThreeCannonBossRocketLauncherState : BossAttackState
    {
        [Header("Rocket Launcher Attack")]
        [SerializeField] private NormalShot _rocketLauncherWeapon;
        [SerializeField] private int _rocketLauncherProjectilesPerAttack;
        [SerializeField] private float _rocketLauncherTimeBetweenProjectiles = 0.25f;
        [SerializeField] private float _rocketImpactRadius = 5;
        [SerializeField] private int _rocketLauncherRandomProjectilesCount = 5;
        [SerializeField] private float _rocketLauncherRandomProjectilesGap = 4;
        [SerializeField] private float _rocketLauncherProjectileInAirDuration;
        [SerializeField] private float _rocketLauncherProjectileMaxHeightReach = 10;
        [SerializeField] private float _rocketLauncherProjectileIndicatorHeight = 0.16f;
        [SerializeField] protected LayerMask _rocketLauncherTargetLayers;

        private Vector3 _targetPosition;
        private Transform _targetTransform;
        private PlayerTransforms _currentTarget;
        private Coroutine _attackCoroutine;
        private bool _followTargetTransform;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);
            _movement.OnTargetFaced += OnTargetFacedHandler;
        }

        public override void OnEnter()
        {
            Debug.Log("ROCKET LAUNCHER ATTACK STATE");
            _isActive = true;

            _followTargetTransform = false;

            // Chance to attack a random target vs farthest target
            float rand = Random.Range(0f, 1f);

            if (rand <= _randomTargetChance)
            {
                var alivePlayers = GameManager.Instance.PlayersManager.GetPlayerTransforms();
                var targetPlayerTransform = alivePlayers[Random.Range(0, alivePlayers.Count)];
                _attackController.SetTarget(targetPlayerTransform);
                _currentTarget = targetPlayerTransform;
                _targetTransform = targetPlayerTransform.PlayerTransform;
                _targetPosition = targetPlayerTransform.PlayerTransform.position;
            }
            else
            {
                var targetPlayerTransform = GameManager.Instance.PlayersManager.GetFarthestPlayer(_movement.transform.position);
                _attackController.SetTarget(targetPlayerTransform);
                _currentTarget = targetPlayerTransform;
                _targetTransform = targetPlayerTransform.PlayerTransform;
                _targetPosition = targetPlayerTransform.PlayerTransform.position;
            }

            _movement.ResetTargetIsFaced();
            //Debug.Log(_target.gameObject.name);
        }

        public override void OnUpdate()
        {
            if (_followTargetTransform)
            {
                _movement.FaceTarget(_targetTransform);
            }
            else
            {
                _movement.FaceTarget(_targetPosition);
            }
        }

        public override void OnExit()
        {
            _isActive = false;
            _followTargetTransform = false;

            if (_attackCoroutine != null)
                _attackController.StopCoroutine(_attackCoroutine);
        }

        public override void OnDispose()
        {
        }

        private void OnTargetFacedHandler()
        {
            if (!_isActive)
                return;

            _followTargetTransform = true;

            //_attackController.Attack(ThreeCannonAttackType.RocketLauncher, OnAttackFinished);
            _attackCoroutine = _attackController.StartCoroutine(RocketLauncherAttackRoutine());
        }

        private void OnAttackFinished()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(BossStateType.Move);
        }

        #region Attack Methods
        private IEnumerator RocketLauncherAttackRoutine()
        {
            List<Indicator> targetIndicators = new List<Indicator>();

            //CalculatePositions(_rocketImpactRadius, targetPoints);

            for (int i = 0; i < _rocketLauncherProjectilesPerAttack; i++)
            {
                //Play animation
               ((ThreeCannonBossAnimations)_animations).TriggerRocketLauncherAnimation();

                var newPoint = _currentTarget.ImageTransform.position;
                newPoint.y = 0.16f; // dirty, have a variable for it

                LaunchRocket(newPoint);

                if (i % _rocketLauncherRandomProjectilesGap == 0 && i > 0)
                {
                    _attackController.StartCoroutine(ShootRocketsAtRandomPointsRoutine(_rocketLauncherRandomProjectilesCount));
                }

                yield return new WaitForSeconds(_rocketLauncherTimeBetweenProjectiles);
            }

            OnAttackFinished();
        }

        private void LaunchRocket(Vector3 targetPoint)
        {
            Transform shootingPoint = _attackController.RocketLauncherShootingPoint;

            //Muzzle flash effect
            ParticleSystemHandler muzzleEffect = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(_rocketLauncherWeapon.BulletData.GUID);
            muzzleEffect.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();

            //Spawn bullets
            Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(_rocketLauncherWeapon.BulletData.GUID);
            bullet.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);
            bullet.gameObject.SetActive(true);
            // enable the bullet's collider, mesh, and trail
            bullet.EnableBullet();
            bullet.SetActive(true);
            bullet.SetTargetLayerMask(_rocketLauncherTargetLayers);
            bullet.SetValues(_rocketLauncherWeapon.BulletSpeed, _rocketLauncherWeapon.Damage, _rocketLauncherWeapon.MaxDistance);
            // set reference to the bullet data
            bullet.SetUpBulletdata(_rocketLauncherWeapon.BulletData);
            // assign the shooter of the bullet
            bullet.SetShooter(_components);
            // move bullet along a spline
            bullet.MoveToPointAlongSpline(targetPoint, _rocketLauncherProjectileMaxHeightReach, _rocketLauncherProjectileInAirDuration);
            GameManager.Instance.AudioManager.Play(_rocketLauncherWeapon.ShotAudio);

            //Spawn indicators
            Vector3 indicatorSize = new Vector3(((AreaOfEffectImpact)bullet.Impact).AreaRadius * 2, 1, ((AreaOfEffectImpact)bullet.Impact).AreaRadius * 2);
            var indicatorType = _rocketLauncherWeapon.IndicatorType;

            Indicator indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(indicatorType);
            indicator.gameObject.SetActive(true);
            indicator.transform.position = targetPoint;
            indicator.transform.rotation = Quaternion.identity;
            indicator.transform.localScale = indicatorSize;
            indicator.Play(_rocketLauncherProjectileInAirDuration);

            bullet.SetTargetIndicator(indicator);
        }


        private IEnumerator ShootRocketsAtRandomPointsRoutine(int pointsCount)
        {
            Vector3[] targetPoints = new Vector3[pointsCount];

            Indicator[] targetIndicators = new Indicator[pointsCount];

            CalculatePositions(_rocketImpactRadius, targetPoints);

            for (int i = 0; i < targetPoints.Length; i++)
            {
                LaunchRocket(targetPoints[i]);
                yield return new WaitForSeconds(0.05f);
            }

            //yield return new WaitForSeconds(2f);

            //for (int i = 0; i < targetIndicators.Length; i++)
            //{
            //    targetIndicators[i].TurnOff();
            //}
        }

        private void CalculatePositions(/*Vector3 mainPoint, float missilesSpreadRange,*/ float impactRange, Vector3[] points)
        {
            var totalPoints = points.Length;
            var pos = _attackController.transform.position;

            if (totalPoints == 0)
                return;

            for (int i = 0; i < totalPoints; i++)
            {
                var count = 0;
                while (true)
                {
                    var newPoint = Helper.GetRandomPointInsideSphere(_currentTarget.ImageTransform.position, 10);
                    newPoint.y = _rocketLauncherProjectileIndicatorHeight;

                    points[i] = newPoint;

                    if (/*new Vector3(newPoint.x - pos.x, 0f, newPoint.z - pos.z).sqrMagnitude >= minFireDistance &&*/ !PointIsOverlapping(newPoint, i, impactRange, points))
                    {
                        points[i] = newPoint;
                        break;
                    }
                    count++;

                    if (count > _rocketLauncherProjectilesPerAttack)
                    {
                        Debug.Log("BAD POINT ADDED!");
                        points[i] = newPoint;
                        break;
                    }
                }
            }
        }

        bool PointIsOverlapping(Vector3 point, int maxIndex, float explosionRange, Vector3[] points)
        {
            for (int i = 1; i < maxIndex; i++)
            {
                if (Vector3.SqrMagnitude(point - points[i]) < Mathf.Pow(explosionRange, 2f))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
