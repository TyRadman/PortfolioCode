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

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _animations = (ThreeCannonBossAnimations)((BossComponents)components).Animations;

            GameManager.Instance.HUDController.BossHUD.SetupHealthBar(_maxHealth);
        }

        public override void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            if (!_canTakeDamage)
            {
                return;
            }

            base.TakeDamage(damage, direction, shooter, bulletPosition);
            GameManager.Instance.HUDController.BossHUD.UpdateHealthBar(_currentHealth, _maxHealth);
            //GameManager.Instance.CameraManager.ShakeCamera(CameraShakeType.HIT);
            // recharge ability if there's an event subscribed
        }

        public override void Die()
        {
            _canTakeDamage = false;
            _animations.TriggerDeathAnimation();
            GameManager.Instance.ReportManager.ReportBossKill((BossData)_stats, _playerIndex);
            GameManager.Instance.BossesManager.RemoveBoss((BossComponents)_components);
            GameManager.Instance.HUDController.BossHUD.Enable(false);
            OnDeath?.Invoke();
        }

        public void ExplodeParts()
        {
            // Return to pool if we're using pooling
            Destroy(gameObject);

            if (_explodeOnDeath)
            {
                var parts = GameManager.Instance.BossesManager.GetBossPartsByType(((BossData)_stats).BossType);
                parts.transform.position = transform.position;
                parts.transform.rotation = transform.rotation;
                parts.gameObject.SetActive(true);
                parts.StartExplosion(_explosionForce, _explosionRadius, _upwardsModifier, _turret.rotation, _body.rotation, bossPreShrink: true);
            }

            // explosion and camera effects (MUTUAL)
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.DeathExplosion;
            explosion.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            explosion.transform.localScale *= 3f;
            explosion.gameObject.SetActive(true);
            explosion.Play();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);

            var decal = GameManager.Instance.VisualEffectsManager.Explosions.ExplosionDecal;
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            decal.transform.SetPositionAndRotation(transform.position + Vector3.up * _explosionDecalheight, randomRotation);
            decal.transform.localScale = _explosionDecalSize;
            decal.gameObject.SetActive(true);
            decal.Play();
        }   
    }
}
