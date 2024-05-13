using UnityEngine;
using UnityEngine.Events;

namespace TankLike.UI.PauseMenu
{
    [System.Serializable]
    public class SelectableAction
    {
        [HideInInspector] public string Name;
#if UNITY_EDITOR
        [HideInInspector] [ReadOnly()] public Direction Direction;
#else
        [HideInInspector] public Direction Direction;
#endif
        public UnityEvent Action;
    }
}
