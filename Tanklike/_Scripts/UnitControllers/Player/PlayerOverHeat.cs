using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UI;
using TankLike.UI.Notifications;
using TankLike.UI.InGame;
using TankLike.Sound;

namespace TankLike.UnitControllers
{
    public class PlayerOverHeat : MonoBehaviour, IController
    {
        protected float _maxShotAmount;
        protected float _currectShotAmount;
        [Tooltip("How many bars will recharge a second")]
        [SerializeField][Range(0.1f, 10f)] protected float _shotsBarsIncrement = 0f;
        public const float PRIOR_RECHARGING_WAIT_TIME = 1f;
        [SerializeField] private int _barsNumber = 3;
        [Header("References")]
        [SerializeField] private SegmentedBar _segmentedBar;
        private bool _canRechargeShots = true;
        private int _lastShotsCount = 0;
        [SerializeField] private Audio _rechargeAudio;
        private const float RECHARGE_SFX_TRIGGER_THRESHOLD = 0.2f;
        private PlayerComponents _components;

        public bool IsActive { get; private set; }

        public void SetUp(PlayerComponents components)
        {
            _components = components;
            _maxShotAmount = _barsNumber;
            _currectShotAmount = _maxShotAmount;
            _lastShotsCount = (int)_maxShotAmount;
            _segmentedBar.SetUp();
            _segmentedBar.SetCount(_barsNumber);
            _segmentedBar.SetTotalAmount(1f);
        }

        private void Update()
        {
            if (!_canRechargeShots)
            {
                return;
            }

            IncreaseShotsOverTime(_shotsBarsIncrement * Time.deltaTime);
        }

        public virtual void IncreaseShotsOverTime(float fuelAmount)
        {
            if (_shotsBarsIncrement <= 0 || _currectShotAmount >= _maxShotAmount)
            {
                return;
            }

            if (_currectShotAmount < _maxShotAmount)
            {
                AddShotBarAmount(fuelAmount);
            }
        }

        public virtual void AddShotBars(int amount)
        {
            _lastShotsCount += amount;
            _segmentedBar.AddAmountToSegments(amount);
            _currectShotAmount += amount;
            _currectShotAmount = Mathf.Clamp(_currectShotAmount, 0, _maxShotAmount);

            _canRechargeShots = false;
            // cancel any invokes that might have been called before
            CancelInvoke();
            // enable recharging after a while
            Invoke(nameof(EnableRecharging), PRIOR_RECHARGING_WAIT_TIME);
        }

        private void EnableRecharging()
        {
            _canRechargeShots = true;
        }

        public virtual void AddShotBarAmount(float amount)
        {
            _segmentedBar.AddAmountToSegments(amount);
            _currectShotAmount += amount;
            _currectShotAmount = Mathf.Clamp(_currectShotAmount, 0, _maxShotAmount);

            if(Mathf.FloorToInt(_currectShotAmount + RECHARGE_SFX_TRIGGER_THRESHOLD) > _lastShotsCount)
            {
                GameManager.Instance.AudioManager.Play(_rechargeAudio);
                _components.Crosshair.PlayReloadAnimation();
                _lastShotsCount++;
            }
        }

        public void FillBars()
        {
            GameManager.Instance.NotificationsManager.PushCollectionNotification(NotificationType.Ammo, 0, _components.PlayerIndex);
            _segmentedBar.AddAmountToSegments(_maxShotAmount - _currectShotAmount);
            _currectShotAmount = _maxShotAmount;
            _lastShotsCount = (int)_maxShotAmount;
        }

        public bool HasEnoughShots(int amount)
        {
            return amount <= _currectShotAmount;
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
