using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Utils;

namespace TankLike.ItemsSystem
{
    [RequireComponent(typeof(Collider))]
    public abstract class Collectable : MonoBehaviour, IPoolable
    {
        [Header("Drop Settings")]
        [SerializeField] private float _bounceHeight = 1.5f;
        [SerializeField] private AnimationCurve _bounceCurve;
        [SerializeField] private float _bounceTime = 0.8f;
        [Header("Attraction")]
        [SerializeField] private float _attractionRadius = 3f;
        [SerializeField] private CollectableAttractor _attractor;
        [Header("Other Values")]
        [SerializeField] private float _displayDuration = 3f;
        [SerializeField] private bool _bounceOnStart = true;
        public CollectableTag Tag;
        private bool _canBeCollected;
        private const float GROUND_LEVEL = 0f;
        [SerializeField] private ConstantsManager _constants;

        protected System.Action<IPoolable> OnReleaseToPool;
        [SerializeField] private bool _startOnEnable = true;
        public bool CanBeCollected => _canBeCollected;
        private bool _isActive = false;

        private void OnEnable()
        {
            if (!_startOnEnable)
            {
                return;
            }
         
            SpawnCollectable();
            _attractor.SetUpAttractor(_attractionRadius, this);
        }

        public void StartCollectable()
        {
            _isActive = true;
            SpawnCollectable();
            _attractor.SetUpAttractor(_attractionRadius, this);
        }

        public virtual void OnCollected(PlayerComponents player)
        {
            _isActive = false;
            // disable attracting
            _attractor.EnableAttractor(false);
            CancelInvoke();
            DisableCollectable();
            PlayParticles();
            OnRelease();
        }

        public virtual void DisableCollectable()
        {
            _canBeCollected = false;
            _attractor.EnableAttractor(false);
            //OnReleaseToPool?.Invoke(this);
        }

        [ContextMenu("Bounce")]
        public virtual void SpawnCollectable()
        {
            if (_bounceOnStart)
            {
                _canBeCollected = false;
                _attractor.EnableAttractor(false);
                StartCoroutine(BounceProcess(_constants.Collectables.BounceCurves.RandomItem()));
            }
            else
            {
                _attractor.EnableAttractor(true);
                _canBeCollected = true;
            }
        }

        private IEnumerator BounceProcess(AnimationCurve curve)
        {
            float timer = 0;

            while (timer < _bounceTime)
            {
                float offset = _bounceCurve.Evaluate(timer / _bounceTime) * _bounceHeight;
                transform.position = new Vector3(transform.position.x, GROUND_LEVEL + offset, transform.position.z);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector3(transform.position.x, GROUND_LEVEL + _bounceCurve.Evaluate(1f) * _bounceHeight, transform.position.z);
            _attractor.EnableAttractor(true);
            _canBeCollected = true;
            OnBounceFinished();
        }

        protected virtual void OnBounceFinished()
        {

        }

        public void PlayParticles()
        {
            var vfx = GameManager.Instance.VisualEffectsManager.Misc.OnCollectedPoof;
            vfx.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }

        #region Pool
        public void Init(System.Action<IPoolable> OnRelease)
        {

        }

        public void TurnOff()
        {

        }

        public void OnRequest()
        {
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
