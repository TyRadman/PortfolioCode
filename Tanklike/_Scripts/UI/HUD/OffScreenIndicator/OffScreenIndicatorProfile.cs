using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    /// <summary>
    /// A profile for the offscreen indicators manager. It holds the player's transform, the icon and other additional informaiton.
    /// </summary>
    [System.Serializable]
    public class OffScreenIndicatorProfile
    {
        public OffScreenIcon Icon;
        public bool FollowPlayer = false;
        public bool IsShown = false;
        public Transform PlayerTransform;
    }
}
