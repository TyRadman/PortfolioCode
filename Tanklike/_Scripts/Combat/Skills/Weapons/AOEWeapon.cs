using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_AOE_Default", menuName = DIRECTORY + "Area Of Effect")]
    public class AOEWeapon : Weapon
    {
        [field: SerializeField] public float ExplosionRadius;
        public override void OnShot(Transform shootingPoint = null, float angle = 0)
        {
            base.OnShot();

            Vector3 explosionCenter = _components.transform.position;

            if (shootingPoint != null)
            {
                explosionCenter = shootingPoint.position;
            }

            Collider[] targets = Physics.OverlapSphere(explosionCenter, ExplosionRadius, _targetLayerMask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        damagable.TakeDamage(Damage, Vector3.zero, _components, _components.transform.position);
                    }
                }
            }
        }

        public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            base.OnShot(spawnBulletAction, shootingPoint, angle);
        }

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
        }

        public void SetExplosionRadius(float radius)
        {
            ExplosionRadius = radius;
        }
    }
}
