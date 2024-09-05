using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Utils;
using System;

namespace TankLike.ItemsSystem
{
    using UI.Notifications;

    [RequireComponent(typeof(Collider))]
    public abstract class Collectable : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public CollectableType Type { get; private set; }
        [Header("Drop Settings")]
        [SerializeField] private float _bounceHeight = 1.5f;
        [SerializeField] private AnimationCurve _bounceCurve;
        [Header("Attraction")]
        [SerializeField] private float _attractionRadius = 3f;
        [SerializeField] private CollectableAttractor _attractor;
        [Header("Other Values")]
        [SerializeField] private bool _bounceOnStart = true;
        private bool _canBeCollected;
        private const float GROUND_LEVEL = 0f;
        [SerializeField] private ConstantsManager _constants;
        public Action<IPoolable> OnReleaseToPool { get; private set; }
        [SerializeField] private bool _startOnEnable = true;

        [Header("Notifications")]
        [SerializeField] protected NotificationBarSettings_SO _notificationSettings;

        [Header("Pool")]
        [SerializeField] private bool _usePool = true;

        public bool CanBeCollected => _canBeCollected;
        private bool _isActive = false;
        private const float SHRINKING_DURATION = 0.2f;

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

            //DisplayNotification(player.PlayerIndex);

            _attractor.EnableAttractor(false);
            CancelInvoke();
            DisableCollectable();
            PlayParticles();

            if (_usePool)
            {
                OnReleaseToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void DisplayNotification(int playerIndex)
        {
            if(_notificationSettings == null)
            {
                return;
            }

            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, 1, playerIndex);
        }

        public virtual void DisableCollectable()
        {
            _canBeCollected = false;
            _attractor.EnableAttractor(false);
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

            float boundTime = GameManager.Instance.Constants.Collectables.BounceTime;

            while (timer < boundTime)
            {
                float offset = _bounceCurve.Evaluate(timer / boundTime) * _bounceHeight;
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
            StartCoroutine(ShrinkingProcess());
        }

        private IEnumerator ShrinkingProcess()
        {
            yield return new WaitForSeconds(GameManager.Instance.Constants.Collectables.DisplayDuration);

            float timeElapsed = 0f;
            Vector3 startScale = transform.localScale;

            while(timeElapsed < SHRINKING_DURATION)
            {
                timeElapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timeElapsed / SHRINKING_DURATION);
                yield return null;
            }

            if (_usePool)
            {
                OnReleaseToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
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
            OnReleaseToPool = OnRelease;
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
