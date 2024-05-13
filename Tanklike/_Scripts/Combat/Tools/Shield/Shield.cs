using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class Shield : MonoBehaviour, IHittable
    {
        private const float FADING_DURATION = 0.3f;
        [Header("References")]
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private MeshRenderer _shieldMesh;
        private Material _shieldMaterial;
        [Header("Damage Taking")]
        private Material _shieldRippleMaterial;
        [SerializeField] private ParticleSystem _rippleParticles;
        [Header("Other")]
        [SerializeField] private float _playerRotationFollowSpeed = 0.3f;
        private bool _isActive = false;
        private int _previousTankLayer;

        private void Awake()
        {
            _shieldRippleMaterial = _rippleParticles.GetComponent<ParticleSystemRenderer>().material;
            _shieldMaterial = _shieldMesh.material;
            _collider.enabled = false;
        }

        #region Activation
        public void ActivateShield(bool activate, Transform tank)
        {
            _collider.enabled = activate;
            StartCoroutine(ShowShield(activate ? 0f : 1f, activate ? 1f : 0f, activate));

            // if the shield is activated then we set its parent to null and make it lerp its rotation to the player's rotation
            if (activate)
            {
                transform.parent = null;
                StartCoroutine(ShieldFollowProcess(tank));
                // set the tank's collider tag and layer to neutral so that it doesn't take any damage
                _previousTankLayer = tank.gameObject.layer;
                tank.gameObject.layer = GameManager.Instance.Constants.Alignments.Find(a => a.Alignment == TankAlignment.NEUTRAL).LayerNumber;
            }
            // otherwise, we make it a child of the tank again
            else
            {
                transform.parent = tank;
                tank.gameObject.layer = _previousTankLayer;
            }
        }

        private IEnumerator ShowShield(float minAlpha, float maxAlpha, bool activate)
        {
            float time = 0f;
            _isActive = activate;

            while(time < FADING_DURATION)
            {
                time += Time.deltaTime;
                SetShieldAlpha(Mathf.Lerp(minAlpha, maxAlpha, time / FADING_DURATION));
                yield return null;
            }
        }

        private IEnumerator ShieldFollowProcess(Transform target)
        {
            while (_isActive)
            {
                transform.SetPositionAndRotation(target.position, Quaternion.Slerp(transform.rotation, target.rotation, _playerRotationFollowSpeed));
                yield return null;
            }
        }

        public void SetShieldAlpha(float value)
        {
            _shieldMaterial.SetFloat("_ShieldAlpha", value);
        }
        #endregion

        public void SetShieldUser(TankAlignment side)
        {
            // set up the alignment of the shield 
            var sideInfo = GameManager.Instance.Constants.Alignments.Find(s => s.Alignment == side);
            gameObject.tag = sideInfo.Tag;
            gameObject.layer = sideInfo.LayerNumber;
        }

        public void ShowDamage(Vector3 contactPoint)
        {
            if(_rippleParticles.isPlaying) _rippleParticles.Stop();

            _shieldRippleMaterial.SetVector("_SphereCenter", contactPoint);
            _rippleParticles.Play();
        }

        public void SetSize(float size)
        {
            transform.localScale = Vector3.one * size;
            //_collider.radius = size;
        }

        public void OnHit(Vector3 point)
        {
            ShowDamage(point);
        }
    }
}
