using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public class Ammunition : MonoBehaviour, IPoolable
    {
        [SerializeField] protected List<string> _targetTags = new List<string>();
        [SerializeField] protected LayerMask _targetLayerMask;

        [Header("Audio")]
        [SerializeField] protected Audio _impactAudio;

        protected const string MUTUAL_HITTABLE_TAG = "MutualHittable";
        protected const string WALL_TAG = "Wall";

        protected bool _isActive;
        [Tooltip("This is the damage a projectile inflicts on hit and the damage a laser beam inflicts every second")]
        protected int _damage = 3;
        [HideInInspector] public TankComponents Instigator;
        public Action<IPoolable> OnReleaseToPool { get; private set; }

        #region Pool
        public virtual void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public virtual void OnRequest()
        {

        }

        public virtual void OnRelease()
        {
            gameObject.SetActive(false);
            GameManager.Instance.SetParentToSpawnables(gameObject);
        }

        public virtual void TurnOff()
        {

        }

        public virtual void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
