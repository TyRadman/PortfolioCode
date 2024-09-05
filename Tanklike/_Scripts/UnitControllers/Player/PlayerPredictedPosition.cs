using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerPredictedPosition : MonoBehaviour, IController
    {
        [SerializeField] private Transform _image;
        [field: SerializeField] public bool IsActive { get; private set; }
        private Transform _playerTransform;
        private PlayerComponents _components;
        private PlayerMovement _movement;
        [SerializeField] private float _distance = 3f;
        [SerializeField] private float _interpolationSpeed = 0.1f;

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            UpdatePosition();
        }

        public void SetUp(TankComponents components)
        {
            _components = (PlayerComponents)components;
            _playerTransform = components.transform;
            _image.parent = null;
            _movement = _components.Movement as PlayerMovement;
        }

        private void UpdatePosition()
        {
            float multiplier = _components.Movement.CurrentSpeed;
            Vector3 direction = _movement.LastMovementInput;
            Vector3 newPosition = _playerTransform.position + direction * _distance * multiplier;
            newPosition = Vector3.Lerp(_image.position, newPosition, _interpolationSpeed * Time.deltaTime);
            newPosition.y = _playerTransform.position.y + 0.5f;
            _image.position = newPosition;
        }

        public Vector3 GetPositionAtDistance(float distance)
        {
            float multiplier = _components.Movement.CurrentSpeed;
            Vector3 direction = _movement.LastMovementInput;
            Vector3 newPosition = _playerTransform.position + direction * distance * multiplier;
            newPosition.y = _playerTransform.position.y + 0.5f;
            return newPosition;
        }

        public Transform GetImage()
        {
            return _image;
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

        public void Dispose()
        {

        }

        public void Restart()
        {
            IsActive = false;
        }
        #endregion
    }
}
