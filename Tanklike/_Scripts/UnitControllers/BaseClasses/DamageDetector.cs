using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    public class DamageDetector : MonoBehaviour, IDamageable
    {
        #region Not Used
        public bool IsInvincible => _health.IsInvincible;
        public Transform Transform => _health.Transform;

        public void Die()
        {

        }

        public DamagePopUpAnchor PopUpAnchor { set; get; }
        #endregion

        [SerializeField] private TankHealth _health;

        public void TakeDamage(int damage, Vector3 direction, TankComponents shooter, Vector3 bulletPosition)
        {
            _health.TakeDamage(damage, direction, shooter, bulletPosition);
        }

#if UNITY_EDITOR
        public int GetLayerMask()
        {
            if(_health == null)
            {
                return -1;
            }

            TankComponents com = _health.GetComponent<TankComponents>();
        
            if(com is PlayerComponents)
            {
                return Constants.PlayerDamagableLayer;
            }
            else
            {
                return Constants.EnemyDamagableLayer;
            }
        }

        public bool HasHealthReference()
        {
            return _health != null;
        }

        public void SetHealth(TankHealth health)
        {
            _health = health;
        }
#endif
    }
}
