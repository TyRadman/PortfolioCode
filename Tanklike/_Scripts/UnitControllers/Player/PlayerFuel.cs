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

        private PlayerComponents _components;
        private float _currentFuel;
        private List<IController> _currentConsumers;
        private float _canRefill;

        public bool IsActive { get; private set; }

        public void Setup(PlayerComponents components)
        {
            _components = components;
            _currentFuel = _maxFuel;

            _currentConsumers = new List<IController>();

            UpdateFuelUI();
        }

        private void Update()
        {
            if(!IsActive)
            {
                return;
            }

            if(_currentFuel < _maxFuel && _currentConsumers.Count == 0)
            {
                _currentFuel += _refillSpeed * Time.deltaTime;
                UpdateFuelUI();
            }
        }

        public bool HasEnoughFuel(float amount)
        {
            if(_currentFuel < amount)
            {
                return false;
            }

            return true;
        }

        public void UseFuel(float amount)
        {
            _currentFuel -= amount;
            _currentFuel = Mathf.Clamp(_currentFuel, 0f, _maxFuel);

            UpdateFuelUI();
        }

        private void UpdateFuelUI()
        {
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateFuelBar(_currentFuel, _maxFuel);
        }

        public void AddConsumer(IController consumer)
        {
            //Debug.Log("Adding fuel consumer " + consumer);

            if (!_currentConsumers.Contains(consumer))
            {
                _currentConsumers.Add(consumer);
            }
        }

        public void RemoveConsumer(IController consumer)
        {
            //Debug.Log("Removing fuel consumer " + consumer);

            if (_currentConsumers.Contains(consumer))
            {
                _currentConsumers.Remove(consumer);
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
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
