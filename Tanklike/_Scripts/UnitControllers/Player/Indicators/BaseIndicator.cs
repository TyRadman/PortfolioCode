using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    public abstract class BaseIndicator : MonoBehaviour
    {
        protected PlayerComponents _player;
        protected PlayerCrosshairController _crosshair;
        protected Transform _playerTransform;
        protected Transform _crosshairTransform;

        public virtual void SetUp(PlayerComponents player)
        {
            _player = player;
            _crosshair = player.Crosshair;
            _playerTransform = player.transform;
            _crosshairTransform = _crosshair.GetCrosshairTransform();
        }

        public virtual void StartIndicator()
        {

        }

        public virtual void UpdateIndicator()
        {

        }

        public virtual void EndIndicator()
        {

        }

        public virtual void Enable()
        {

        }

        public virtual void Disable()
        {

        }
    }
}
