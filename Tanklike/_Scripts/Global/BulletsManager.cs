using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike
{
    public class BulletsManager : MonoBehaviour
    {
        private List<Bullet> _activeBullets = new List<Bullet>();
        public void Setup()
        {
        }

        public void AddBullet(Bullet bullet)
        {
            if (!_activeBullets.Contains(bullet))
                _activeBullets.Add(bullet);
        }

        public void RemoveBullet(Bullet bullet)
        {
            if (_activeBullets.Contains(bullet))
                _activeBullets.Remove(bullet);
        }

        public void DeactivateBullets()
        {
            foreach (var bullet in _activeBullets)
            {
                bullet.TurnOff();
            }

            _activeBullets.Clear();
        }
    }
}
