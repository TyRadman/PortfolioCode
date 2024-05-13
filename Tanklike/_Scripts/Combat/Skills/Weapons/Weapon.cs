using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Utils;
using TankLike.Misc;
using static TankLike.IndicatorEffects;
using TankLike.Sound;

namespace TankLike.Combat
{
    public enum WeaponType
    {
        NORMAL = 0,
        LASER = 1,
        THROWER = 2,
        AOE = 3,
        ROCKET_LAUNCHER,
    }

    public abstract class Weapon : Skill
    {
        [field: SerializeField] public AmmunationData BulletData { get; private set; }
        [Header("Weapon Settings")]
        [SerializeField] protected WeaponType _weaponType;
        [field: SerializeField] public float CoolDownTime { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public Audio ShotAudio { get; private set; }
        [field: SerializeField] public IndicatorType IndicatorType { get; private set; }

        protected Transform _tankTransform;
        protected LayerMask _targetLayerMask;

        public virtual void OnShot(TankComponents shooter = null, Transform shootingPoint = null, float angle = 0)
        {
            if(ShotAudio != null)
            {
                GameManager.Instance.AudioManager.Play(ShotAudio);
            }
            else
            {
                Debug.LogError($"No ShotAudio found for the weapon {this.name}");
            }
        }

        public virtual void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {

        }

        public override void SetUp(Transform tankTransform)
        {
            base.SetUp(tankTransform);
            _tankTransform = tankTransform;
            // clone this SO and add it to the player
            Weapon normalShot = Instantiate(this);       
            // add the copied SO to the player
            tankTransform.GetComponent<PlayerShooter>().AddNormalShot(normalShot);
        }

        public void SetDamage(int damage)
        {
            Damage = damage;
        }

        public virtual void SetSpeed(float speed)
        {
        }

        public void SetTargetLayer(LayerMask targetLayers)
        {
            _targetLayerMask = targetLayers;
        }
    }
}
