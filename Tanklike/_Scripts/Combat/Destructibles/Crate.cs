using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Combat.Destructible
{
    public class Crate : DestructibleDropper
    {
        [Header("Special Values")]
        [SerializeField] private float _explosionForce = 30f;
        [SerializeField] private float _explosionRadius = 2f;
        [Header("References")]
        [SerializeField] private List<Rigidbody> _parts;
        [Header("Animations")]
        [SerializeField] private Animation _anim;
        [SerializeField] private AnimationClip _shrikingClip;
        [SerializeField] private AnimationClip _enableRigidbodyClip;

        protected override void OnDeath(TankComponents tank)
        {
            base.OnDeath(tank);

            // report the destruction of the object to the report manager
            int playerIndex = -1;

            if (tank is PlayerComponents)
            {
                playerIndex = ((PlayerComponents)tank).PlayerIndex;
            }

            GameManager.Instance.ReportManager.ReportDestroyingObject(Tag, playerIndex);
            // spawn drops
            GameManager.Instance.CollectableManager.SpawnCollectablesOfType(CollecatblesToSpawn, transform.position, Tag);
            _anim.clip = _enableRigidbodyClip;
            _anim.Play();
            Invoke(nameof(ShrinkParts), 3f);
        }

        public void Explode()
        {
            Vector3 randomOffset = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            _parts.ForEach(p => p.AddExplosionForce(_explosionForce, transform.position + randomOffset, _explosionRadius));
        }

        private void ShrinkParts()
        {
            _anim.clip = _shrikingClip;
            _anim.Play();
        }
    }
}