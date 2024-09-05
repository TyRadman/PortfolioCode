using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Tutorial
{
    using UnitControllers;

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "Jumps", menuName = MENU_PATH + "Jumps")]
    public class JumpAchievement : TutorialAchievement
    {
        [SerializeField] private int _requiredJumps = 3;
        private int _successfulJumpsCount = 0;
        private PlayerComponents _currentPlayer;
        [SerializeField] private ProjectileDetector _detector;
        private ProjectileDetector _currentDetector;

        public override bool IsAchieved()
        {
            return _successfulJumpsCount >= _requiredJumps;
        }

        public override void SetUp(WaypointMarker marker)
        {
            base.SetUp(marker);
            _successfulJumpsCount = 0;
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(0);
            SubscribeOnHitEvent();
            _currentPlayer.OnPlayerRevived += SubscribeOnHitEvent;
            //_currentPlayer.Jump.OnJumped += OnPlayerJumped;

            _currentDetector = Instantiate(_detector, _currentPlayer.transform.position, Quaternion.identity);
            _currentDetector.OnProjectileDetected += OnPlayerJumped;
        }

        private void SubscribeOnHitEvent()
        {
            _currentPlayer.Health.OnHit += OnPlayerHit;
        }

        private void OnPlayerHit()
        {
            _successfulJumpsCount = 0;
        }

        private void OnPlayerJumped()
        {
            _successfulJumpsCount++;

            if(IsAchieved())
            {
                OnAchievementCompleted?.Invoke();
                _currentDetector.OnProjectileDetected -= OnPlayerJumped;
                _currentPlayer.OnPlayerRevived -= SubscribeOnHitEvent;
                Destroy(_currentDetector);
            }
        }
    }
}
