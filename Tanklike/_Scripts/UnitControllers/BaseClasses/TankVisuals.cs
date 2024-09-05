using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class TankVisuals : MonoBehaviour, IController
    {
        [Header("Hit Flash")]
        [SerializeField] private Material _hitFlashMaterial;
        [SerializeField] private float _hitFlashTime;

        [SerializeField] private List<Renderer> _tankMeshes = new List<Renderer>();
        private const float TRANSITION_DURATION = 0.5f;
        private Material _originalMaterial; // maybe save the material of each mesh by using a dictionary instead of a list
        private Color _originalColor;
        private Coroutine _hitFlashCoroutine;
        private Coroutine _switchColorCoroutine;

        public bool IsActive { get; private set; }

        public void Setup(TankComponents components)
        {
            TankBodyParts parts = components.TankBodyParts;

            parts.Parts.ForEach(p => AddMeshes(p.Meshes));

            _originalMaterial = _tankMeshes[0].material;
            _originalColor = _tankMeshes[0].material.color;
            components.Health.OnHit += OnHitHandler;

        }

        public void SetTextureForMainMaterial(Texture2D texture)
        {
            _originalMaterial.SetTexture("_BaseMap", texture);
            SwitchMaterial(_originalMaterial);
        }

        public void SwitchMaterial(Material material)
        {
            _tankMeshes.ForEach(m => m.material = material);
        }

        public void RestoreOriginalMaterial()
        {
            _tankMeshes.ForEach(m => m.material = _originalMaterial);
        }

        public void RestoreOriginalColor()
        {
            _tankMeshes.ForEach(m => m.material.color = _originalColor);
        }

        public void AddMaterial(Material material)
        {
            foreach (var mesh in _tankMeshes)
            {
                Material[] sharedMaterials = mesh.sharedMaterials;
                Material[] newMats = new Material[sharedMaterials.Length + 1];
                newMats[0] = mesh.sharedMaterials[0];
                newMats[1] = material;
                mesh.sharedMaterials = newMats;
            }
        }

        public void RemoveMaterial()
        {
            foreach (var mesh in _tankMeshes)
            {
                Material[] sharedMaterials = mesh.sharedMaterials;
                Material[] newMats = new Material[sharedMaterials.Length - 1];
                newMats[0] = mesh.sharedMaterials[0];
                mesh.sharedMaterials = newMats;
            }
        }

        public void AddColorToMeshes(Color color)
        {
            //_tankMeshes.ForEach(m => m.material.color += color);
            if (_switchColorCoroutine != null)
                StopCoroutine(_switchColorCoroutine);
            _switchColorCoroutine = StartCoroutine(SwitchToColor(Color.white, color));
        }

        public void SubtractColorFromMeshes(Color color)
        {
            //_tankMeshes.ForEach(m => m.material.color -= color);
            if (_switchColorCoroutine != null)
                StopCoroutine(_switchColorCoroutine);
            _switchColorCoroutine = StartCoroutine(SwitchToColor(color, Color.white));
        }

        private IEnumerator SwitchToColor(Color startColor, Color endColor)
        {
            float time = 0f;

            while (time < TRANSITION_DURATION)
            {
                time += Time.deltaTime;
                float t = time / TRANSITION_DURATION;
                _tankMeshes.ForEach(m => m.material.color = Color.Lerp(startColor, endColor, t));
                yield return null;
            }
        }

        public void AddMeshes(List<Renderer> meshes)
        {
            _tankMeshes.AddRange(meshes);
        }

        public Material GetOriginalMaterial()
        {
            return _originalMaterial;
        }

        public void HideVisuals()
        {
            foreach (var mesh in _tankMeshes)
            {
                mesh.enabled = false;
            }
        }

        public void ShowVisuals()
        {
            foreach (var mesh in _tankMeshes)
            {
                mesh.enabled = true;
            }
        }

        private void OnHitHandler()
        {
            if (_hitFlashMaterial != null)
            {
                if (_hitFlashCoroutine != null)
                    StopCoroutine(_hitFlashCoroutine);
                _hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            foreach (var mesh in _tankMeshes)
            {
                //mesh.material = _hitFlashMaterial;
                mesh.material.SetColor("_BaseColor", Color.red);
            }

            yield return new WaitForSeconds(_hitFlashTime);

            foreach (var mesh in _tankMeshes)
            {
                //mesh.material = _originalMaterial;
                mesh.material.SetColor("_BaseColor", _originalColor);

            }
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;

            if (_hitFlashCoroutine != null)
                StopCoroutine(_hitFlashCoroutine);

            _tankMeshes.ForEach(m => m.material = _originalMaterial);

            if (_switchColorCoroutine != null)
                StopCoroutine(_switchColorCoroutine);

            _tankMeshes.ForEach(m => m.material.color = _originalColor);
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
