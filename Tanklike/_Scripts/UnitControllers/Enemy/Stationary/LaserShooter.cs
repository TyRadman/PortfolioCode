using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Combat;
using TankLike.Misc;
using TankLike.Sound;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class LaserShooter : EnemyShooter
    {
        [SerializeField] private LayerMask _wallLayers;

        public override void DefaultShot(Transform shootingPoint = null, float angle = 0)
        {
            if (_currentWeapon == null) return;

            OnShoot?.Invoke();

            foreach (Transform point in _shootingPoints)
            {
                _currentWeapon.OnShot(_components, point, angle);
            }
            //SpawnBullet();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
        }

        protected override IEnumerator TelegraphRoutine()
        {
            float duration = 0f;
            List<IPoolable> activeEffects = new List<IPoolable>();
            List<Indicator> activeIndicators = new List<Indicator>();

            float timer = 0f;

            foreach (Transform point in _shootingPoints)
            {
                ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Telegraphs.EnemyTelegraph;
                vfx.transform.SetLocalPositionAndRotation(point.position, point.rotation);
                vfx.transform.position += vfx.transform.forward * _telegraphOffset;
                vfx.transform.parent = point;
                vfx.gameObject.SetActive(true);
                vfx.Play(vfx.Particles.main.duration / _telegraphDuration);
                _activePoolables.Add(vfx);
                activeEffects.Add(vfx);

              

                if(_currentWeapon.IndicatorType != IndicatorEffects.IndicatorType.None)
                {
                    Indicator indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(_currentWeapon.IndicatorType);
                    indicator.gameObject.SetActive(true);
                    var pos = point.position;
                    pos.y = 0.52f;
                    indicator.transform.position = pos;
                    indicator.transform.rotation = point.rotation;         
                    indicator.transform.parent = point;
                    indicator.Play();
                    _activePoolables.Add(indicator);
                    activeIndicators.Add(indicator);
                }
               
                if (duration == 0f) duration = vfx.Particles.main.duration;
            }

            // Update indicators
            while (timer < _telegraphDuration)
            {
                for (int i = 0; i < _shootingPoints.Count; i++)
                {
                    var point = _shootingPoints[i];
                    Ray ray = new Ray(point.position, point.forward);
                    bool cast = Physics.SphereCast(ray, ((LaserWeapon)_currentWeapon).Thickness, out RaycastHit hit, 50, _wallLayers);
                    Vector3 hitPoint = point.position + point.forward * ((LaserWeapon)_currentWeapon).MaxLength;

                    if (cast && (1 << hit.transform.gameObject.layer & _wallLayers) == 1 << hit.transform.gameObject.layer)
                    {
                        hitPoint = hit.point;
                    }

                    float dist = Vector3.Distance(point.position, hitPoint);
                    Vector3 indicatorSize = new Vector3(((LaserWeapon)_currentWeapon).Thickness, 0f, dist);

                    activeIndicators[i].transform.localScale = indicatorSize;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }

            OnTelegraphFinished?.Invoke();
            foreach (var effect in activeEffects)
            {
                effect.TurnOff();

                if (_activePoolables.Contains(effect))
                {
                    _activePoolables.Remove(effect);
                }
            }

            foreach (var indicator in activeIndicators)
            {
                indicator.TurnOff();

                if (_activePoolables.Contains(indicator))
                {
                    _activePoolables.Remove(indicator);
                }
            }

            activeEffects.Clear();
            activeIndicators.Clear();
        }
    }
}
