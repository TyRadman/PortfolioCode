using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerFuel : MonoBehaviour, IController
    {
        [Header("Settings")]
        [SerializeField] private float _maxFuel;
        [SerializeField] private float _refillSpeed;
        [SerializeField] private float _refillDelay = 1f;

        private PlayerComponents _components;
        private float _currentFuel;
        private Coroutine _refillCoroutine;
        private WaitForSeconds _refillWaitForSeconds;
        private bool _canConsumeFuel = true;

        public bool IsActive { get; private set; }

        public void Setup(PlayerComponents components)
        {
            _components = components;
            _currentFuel = _maxFuel;

            _refillWaitForSeconds = new WaitForSeconds(_refillDelay);
            UpdateFuelUI();
        }

        public bool HasEnoughFuel(float amount)
        {
            if(_currentFuel < amount)
            {
                return false;
            }

            return true;
        }

        public bool HasEnoughFuel()
        {
            return _currentFuel > 0f;
        }

        public void UseFuel(float amount)
        {
            if(!_canConsumeFuel)
            {
                return;
            }

            _currentFuel -= amount;
            _currentFuel = Mathf.Clamp(_currentFuel, 0f, _maxFuel);

            UpdateFuelUI();

            if (_refillCoroutine != null)
            {
                StopCoroutine(_refillCoroutine);
            }

            _refillCoroutine = StartCoroutine(RefillRoutine());
        }

        public void RefillFuel()
        {
            _currentFuel = _maxFuel;
            UpdateFuelUI();
        }

        private void UpdateFuelUI()
        {
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateFuelBar(_currentFuel, _maxFuel);
        }

        private IEnumerator RefillRoutine()
        {
            yield return _refillWaitForSeconds;

            while (_currentFuel < _maxFuel)
            {
                _currentFuel += _refillSpeed * Time.deltaTime;
                UpdateFuelUI();

                yield return null;
            }
        }

        public void EnableFuelConsumption(bool enable)
        {
            _canConsumeFuel = enable;
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
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
