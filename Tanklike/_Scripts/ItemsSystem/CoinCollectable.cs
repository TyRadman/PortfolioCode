using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    public class CoinCollectable : Collectable
    {
        [SerializeField] private int _amount;
        [SerializeField] private Animation _animation;
        private const float MAX_ANIMATION_START_DELAY = 0.5f;

        public override void OnCollected(PlayerComponents player)
        {
            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, _amount, player.PlayerIndex);

            GameManager.Instance.PlayersManager.Coins.AddCoins(_amount);
            base.OnCollected(player);
        }

        protected override void OnBounceFinished()
        {
            base.OnBounceFinished();

            if(_animation != null)
            {
                Invoke(nameof(PlayAnimation), Random.Range(0f, MAX_ANIMATION_START_DELAY));
            }
        }

        private void PlayAnimation()
        {
            _animation.Play();
        }
    }
}
