using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "Area Of Effect Impact", menuName = "Shot Configurations/Impact/Area Of Effect Impact")]
    public class AreaOfEffectImpact : OnImpact
    {
        [SerializeField] private float _areaRadius = 2f;

        public float AreaRadius => _areaRadius; 

        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            Collider[] targets = Physics.OverlapSphere(hitPoint, _areaRadius, mask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        if (damagable.Invincible) continue;

                        damagable.TakeDamage(damage, Vector3.zero, bullet.GetShooter(), bullet.transform.position); //not sure if we need the direction yet. Better have and not need it than need it and not have it :)
                    }
                }
            }

            bullet.DisableBullet();
        }
    }
}
