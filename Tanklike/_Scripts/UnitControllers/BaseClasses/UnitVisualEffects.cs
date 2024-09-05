using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using UnityEngine;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// This class handles spawning visual effects for the unit
    /// </summary>
    public class UnitVisualEffects : MonoBehaviour, IController
    {
        [Header("Death Explosion")]
        [SerializeField] protected Vector3 _explosionSize = Vector3.one;
        [SerializeField] protected Vector3 _explosionDecalSize = Vector3.one;
        [SerializeField] protected float _explosionDecalheight = 0.1f;

        public bool IsActive { get; private set; }

        public virtual void SetUp(TankComponents components)
        {
        }

        public void PlayDeathEffects()
        {
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.DeathExplosion;
            explosion.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            explosion.transform.localScale = _explosionSize;
            explosion.gameObject.SetActive(true);
            explosion.Play();
            
            var decal = GameManager.Instance.VisualEffectsManager.Explosions.ExplosionDecal;
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            decal.transform.SetPositionAndRotation(transform.position + Vector3.up * _explosionDecalheight, randomRotation);
            decal.transform.localScale = _explosionDecalSize;
            decal.gameObject.SetActive(true);
            decal.Play();
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            IsActive = false;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
