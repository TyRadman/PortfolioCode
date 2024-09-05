using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;
using TankLike.UnitControllers;
using TankLike.Utils;
using TankLike.Environment.LevelGeneration;
using TankLike.Cam;

namespace TankLike.Combat.Destructible
{
    public class Stone : DestructibleDropper, IBoostDestructible
    {
        [Header("Special Values")]
        [SerializeField] private Renderer _meshRenderer;
        [SerializeField] private float _shatterDuration;
        [SerializeField] private float _collectableChance;
        [Header("References")]
        [SerializeField] private Collider _collider;
        [SerializeField] private Collider _trigger;
        [SerializeField] private ParticleSystem _shatterEffect;

        public void Destruct()
        {
            StartCoroutine(ShatterRoutine());
        }

        protected override void OnDeath(TankComponents tank)
        {
            base.OnDeath(tank);

            StartCoroutine(ShatterRoutine());
        }

        private IEnumerator ShatterRoutine()
        {
            _collider.enabled = false;
            _trigger.enabled = false;

            GameManager.Instance.CollectableManager.SpawnCollectablesOfType(CollecatblesToSpawn, transform.position, Tag);

            if (_meshRenderer != null) _meshRenderer.gameObject.SetActive(false);

            _shatterEffect.Play();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
            yield return new WaitForSeconds(_shatterDuration);

            Destroy(gameObject);
        }
    }
}
