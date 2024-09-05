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

    public abstract class Weapon : Skill
    {
        public const string DIRECTORY = Directories.AMMUNITION + "Weapons/";
        [field: SerializeField] public AmmunationData BulletData { get; private set; }

        [field: SerializeField, Header("Weapon Settings")] public float CoolDownTime { get; private set; }
        [field: SerializeField] public int AmmoCapacity { get; private set; } = 5;
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public Audio ShotAudio { get; private set; }
        [field: SerializeField] public IndicatorType IndicatorType { get; private set; }

        protected Transform _tankTransform;
        protected TankComponents _components;

        [Tooltip("Set this only if the \'OnShot\' method doesn't pass a tank component.")]
        protected LayerMask _targetLayerMask;

        public virtual void OnShot(Transform shootingPoint = null, float angle = 0)
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

        public virtual void OnShot(Vector3 spawnPoint, Vector3 rotation, float angle = 0)
        {
            if (ShotAudio != null)
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

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _tankTransform = components.transform;
            _components = _tankTransform.GetComponent<TankComponents>();
            // TODO: Move it to the skill tree
            //tankTransform.GetComponent<PlayerShooter>().AddNormalShot(this);
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

        public virtual void DisposeWeapon()
        {
        }
    }
}
