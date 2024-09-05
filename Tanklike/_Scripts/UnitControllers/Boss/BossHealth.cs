using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class BossHealth : EnemyHealth
    {
        [SerializeField] protected float _explosionEffectDelay = 0.3f;

        private ThreeCannonBossAnimations _animations;

        private Coroutine _deathEffectCoroutine;
        public System.Action<int, int> OnTakeDamage;
        private BossComponents _bossComponents;
        // TODO: should be the parameter passed for OnDeath
        public System.Action<BossComponents> OnBossDeath;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _bossComponents = (BossComponents)components;
            _animations = (ThreeCannonBossAnimations)((BossComponents)components).Animations;

            GameManager.Instance.HUDController.BossHUD.SetupHealthBar(_maxHealth);

            SetUpSubscriptions();
        }

        private void SetUpSubscriptions()
        {
            OnTakeDamage = GameManager.Instance.HUDController.BossHUD.UpdateHealthBar;

            OnBossDeath += GameManager.Instance.ReportManager.ReportBossKill;
            OnBossDeath += GameManager.Instance.BossesManager.RemoveBoss;
            OnBossDeath += GameManager.Instance.EnemiesManager.RemoveEnemy;
            OnBossDeath += a => GameManager.Instance.HUDController.BossHUD.Enable(false);
            OnDeath += GameManager.Instance.ResultsUIController.DisplayVictoryScreen;
        }

        public override void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            if (!_canTakeDamage)
            {
                return;
            }

            base.TakeDamage(damage, direction, shooter, bulletPosition);

            OnTakeDamage?.Invoke(_currentHealth, _maxHealth);
        }

        public override void Die()
        {
            _canTakeDamage = false;
            _animations.TriggerDeathAnimation();
            OnDeath?.Invoke(_components);

            OnBossDeath?.Invoke(_bossComponents);
            
            RemoveSubscriptions();
        }

        private void RemoveSubscriptions()
        {
            OnBossDeath = null;
            OnTakeDamage = null;
            OnDeath = null;
        }

        public void ExplodeParts()
        {
            // Return to pool if we're using pooling
            Destroy(gameObject);

            _components.TankBodyParts.HandlePartsExplosion((BossData)_stats);
            _components.VisualEffects.PlayDeathEffects();

            // Switch back to the level BG music
            GameManager.Instance.BossesManager.SwitchBackBGMusic();
        }
    }
}
