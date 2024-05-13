using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "W_AOE_Default", menuName = "TankLike/Skills/Weapons/AOE")]
    public class AOEWeapon : Weapon
    {
        [field: SerializeField] public float ExplosionRadius;
        public override void OnShot(TankComponents shooter = null, Transform shootingPoint = null, float angle = 0)
        {
            base.OnShot(shooter);

            Vector3 explosionCenter = shooter.transform.position;
            if (shootingPoint != null)
                explosionCenter = shootingPoint.position;

            Collider[] targets = Physics.OverlapSphere(explosionCenter, ExplosionRadius, _targetLayerMask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        damagable.TakeDamage(Damage, Vector3.zero, shooter, shooter.transform.position);
                    }
                }
            }
        }

        public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            base.OnShot(spawnBulletAction, shootingPoint, angle);
        }

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);
        }

        public void SetExplosionRadius(float radius)
        {
            ExplosionRadius = radius;
        }
    }
}
