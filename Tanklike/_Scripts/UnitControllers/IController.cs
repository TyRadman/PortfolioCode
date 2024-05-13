using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public interface IController
    {
        public bool IsActive { get; }
        /// <summary>
        /// Make the unit playable.
        /// </summary>
        public void Activate();
        /// <summary>
        /// Temporarily stop the unit from being playable.
        /// </summary>
        public void Deactivate();
        /// <summary>
        /// Disables the unit and resets all its components' values.
        /// </summary>
        public void Restart();
        /// <summary>
        /// For when the game is closed? Clean up your mess before you leave the room young man!
        /// </summary>
        public void Dispose();
    }
}
